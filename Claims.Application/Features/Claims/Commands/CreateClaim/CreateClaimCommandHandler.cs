
using Claims.Application.Common.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;

namespace Claims.Application.Features.Claims.Commands.CreateClaim;

public class CreateClaimCommandHandler
    : IRequestHandler<CreateClaimCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateClaimCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
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

        return claim.Id;
    }
}
