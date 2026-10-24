using Claims.Application.Common.Interfaces;
using Claims.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public class GetClaimByClaimNumberQueryHandler
    : IRequestHandler<GetClaimByClaimNumberQuery, ClaimDto?>
{
    private readonly IAppDbContext _context;

    public GetClaimByClaimNumberQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimDto?> Handle(
        GetClaimByClaimNumberQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Claims
            .AsNoTracking()
            .Where(c => c.ClaimNumber == request.ClaimNumber)
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
