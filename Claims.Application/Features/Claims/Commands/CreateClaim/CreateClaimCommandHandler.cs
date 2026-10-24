
using Claims.Application.Common.Interfaces;
using Claims.Application.DTOs;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;

namespace Claims.Application.Features.Claims.Commands.CreateClaim;

public class CreateClaimCommandHandler
    : IRequestHandler<CreateClaimCommand, ClaimDto>
{
    private readonly IAppDbContext _context;

    public CreateClaimCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<ClaimDto> Handle(
        CreateClaimCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Claim;

        var claim = new Claim
        {
            ClaimNumber = $"CLM-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            ClaimantName = input.ClaimantName.Trim(),
            ClaimantEmail = input.ClaimantEmail.Trim(),
            Market = input.Market.Trim(),
            ClaimType = input.ClaimType,
            IncidentDate = input.IncidentDate,
            IncidentDescription = input.IncidentDescription.Trim(),
            EstimatedAmount = input.EstimatedAmount,
            Status = ClaimStatus.Submitted,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            Action = "ClaimSubmitted",
            Details = $"Claim {claim.ClaimNumber} was submitted.",
            PerformedBy = claim.ClaimantEmail,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Claims.Add(claim);
        _context.ClaimAudits.Add(audit);

        await _context.SaveChangesAsync(cancellationToken);

        return new ClaimDto
        {
            Id = claim.Id,
            ClaimNumber = claim.ClaimNumber,
            ClaimantName = claim.ClaimantName,
            ClaimantEmail = claim.ClaimantEmail,
            Market = claim.Market,
            ClaimType = claim.ClaimType,
            IncidentDate = claim.IncidentDate,
            IncidentDescription = claim.IncidentDescription,
            EstimatedAmount = claim.EstimatedAmount,
            Status = claim.Status,
            AssignedOfficer = claim.AssignedOfficer,
            AdditionalInformation = claim.AdditionalInformation,
            DecisionReason = claim.DecisionReason,
            CreatedAtUtc = claim.CreatedAtUtc,
            UpdatedAtUtc = claim.UpdatedAtUtc
        };
    }
}
