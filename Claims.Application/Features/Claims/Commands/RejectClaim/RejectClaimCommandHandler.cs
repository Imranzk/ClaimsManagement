
using Claims.Application.Common.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Commands.RejectClaim;

public class RejectClaimCommandHandler
    : IRequestHandler<RejectClaimCommand, bool>
{
    private readonly IAppDbContext _context;

    public RejectClaimCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        RejectClaimCommand request,
        CancellationToken cancellationToken)
    {
        var claim = await _context.Claims
            .FirstOrDefaultAsync(
                c => c.Id == request.ClaimId,
                cancellationToken);

        if (claim is null)
        {
            throw new KeyNotFoundException("Claim not found.");
        }

        if (claim.Status != ClaimStatus.UnderReview &&
            claim.Status != ClaimStatus.InformationProvided)
        {
            throw new InvalidOperationException(
                "Only claims under review or with additional information provided can be rejected.");
        }

        var decisionReason = request.DecisionReason.Trim();
        var performedBy = request.PerformedBy.Trim();
        var now = DateTime.UtcNow;

        claim.Status = ClaimStatus.Rejected;
        claim.DecisionReason = decisionReason;
        claim.UpdatedAtUtc = now;

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            Action = "ClaimRejected",
            Details = decisionReason,
            PerformedBy = performedBy,
            CreatedAtUtc = now
        };

        _context.ClaimAudits.Add(audit);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
