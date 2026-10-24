
using Claims.Application.Common.Interfaces;
using Claims.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public class GetClaimAuditQueryHandler
    : IRequestHandler<GetClaimAuditQuery, List<ClaimAuditDto>>
{
    private readonly IAppDbContext _context;

    public GetClaimAuditQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClaimAuditDto>> Handle(
        GetClaimAuditQuery request,
        CancellationToken cancellationToken)
    {
        var claimExists = await _context.Claims
            .AnyAsync(
                c => c.Id == request.ClaimId,
                cancellationToken);

        if (!claimExists)
        {
            throw new KeyNotFoundException("Claim not found.");
        }

        return await _context.ClaimAudits
            .AsNoTracking()
            .Where(a => a.ClaimId == request.ClaimId)
            .OrderBy(a => a.CreatedAtUtc)
            .Select(a => new ClaimAuditDto
            {
                Id = a.Id,
                ClaimId = a.ClaimId,
                Action = a.Action,
                Details = a.Details,
                PerformedBy = a.PerformedBy,
                CreatedAtUtc = a.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }
}
