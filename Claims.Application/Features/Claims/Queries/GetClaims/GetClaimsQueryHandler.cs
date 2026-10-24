
using Claims.Application.Common.Interfaces;
using Claims.Application.DTOs;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Queries.GetClaims;

public class GetClaimsQueryHandler
    : IRequestHandler<GetClaimsQuery, List<ClaimDto>>
{
    private readonly IAppDbContext _context;

    public GetClaimsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClaimDto>> Handle(
        GetClaimsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Claims.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<ClaimStatus>(
                    request.Status, true, out var status) ||
                !Enum.IsDefined(status))
            {
                throw new ArgumentException("Invalid claim status.");
            }

            query = query.Where(c => c.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(request.Market))
        {
            var market = request.Market.Trim();
            query = query.Where(c => c.Market == market);
        }

        if (!string.IsNullOrWhiteSpace(request.AssignedOfficer))
        {
            var officer = request.AssignedOfficer.Trim();
            query = query.Where(c => c.AssignedOfficer == officer);
        }

        return await query
            .OrderByDescending(c => c.CreatedAtUtc)
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
            .ToListAsync(cancellationToken);
    }
}
