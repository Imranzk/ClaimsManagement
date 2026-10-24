
using Claims.Application.Common.Interfaces;
using Claims.Application.DTOs;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Dashboard;

public class GetDashboardQueryHandler
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IAppDbContext _context;

    public GetDashboardQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> Handle(
        GetDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var claims = _context.Claims.AsNoTracking();

        var dashboard = new DashboardDto
        {
            TotalClaims = await claims.CountAsync(cancellationToken),

            SubmittedClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.Submitted,
                cancellationToken),

            UnderReviewClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.UnderReview,
                cancellationToken),

            InformationRequestedClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.InformationRequested,
                cancellationToken),

            InformationProvidedClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.InformationProvided,
                cancellationToken),

            ApprovedClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.Approved,
                cancellationToken),

            RejectedClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.Rejected,
                cancellationToken),

            SettledClaims = await claims.CountAsync(
                c => c.Status == ClaimStatus.Settled,
                cancellationToken),

            OutstandingLiability = await claims
                .Where(c =>
                    c.Status != ClaimStatus.Settled &&
                    c.Status != ClaimStatus.Rejected)
                .SumAsync(
                    c => (decimal?)c.EstimatedAmount,
                    cancellationToken) ?? 0m
        };

        dashboard.OfficerWorkloads = await claims
            .Where(c =>
                c.AssignedOfficer != null &&
                c.AssignedOfficer != string.Empty &&
                c.Status != ClaimStatus.Settled &&
                c.Status != ClaimStatus.Rejected)
            .GroupBy(c => c.AssignedOfficer!)
            .Select(group => new OfficerWorkloadDto
            {
                OfficerName = group.Key,
                AssignedClaims = group.Count()
            })
            .OrderByDescending(x => x.AssignedClaims)
            .ToListAsync(cancellationToken);

        dashboard.RecentClaims = await claims
            .OrderByDescending(c => c.CreatedAtUtc)
            .Take(5)
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

        return dashboard;
    }
}