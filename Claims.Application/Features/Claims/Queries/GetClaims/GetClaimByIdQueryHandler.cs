
using Claims.Application.Common.Interfaces;
using Claims.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public class GetClaimByIdQueryHandler
    : IRequestHandler<GetClaimByIdQuery, ClaimDto?>
{
    private readonly IAppDbContext _context;

    public GetClaimByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimDto?> Handle(
        GetClaimByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Claims
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new ClaimDto
            {
                Id = c.Id,
                ClaimNumber = c.ClaimNumber,
                ClaimantName = c.ClaimantName,
                ClaimantEmail = c.ClaimantEmail,
                Market = c.Market,
                ClaimType = c.ClaimType,
                IncidentDate = c.IncidentDate,
                IncidentDescription = c.IncidentDescription,
                EstimatedAmount = c.EstimatedAmount,
                Status = c.Status,
                AssignedOfficer = c.AssignedOfficer,
                AdditionalInformation = c.AdditionalInformation,
                DecisionReason = c.DecisionReason,
                CreatedAtUtc = c.CreatedAtUtc,
                UpdatedAtUtc = c.UpdatedAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
