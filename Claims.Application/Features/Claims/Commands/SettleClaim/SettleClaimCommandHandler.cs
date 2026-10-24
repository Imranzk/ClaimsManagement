
using Claims.Application.Common.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Commands.SettleClaim;

public class SettleClaimCommandHandler
    : IRequestHandler<SettleClaimCommand, bool>
{
    private readonly IAppDbContext _context;

    public SettleClaimCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        SettleClaimCommand request,
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

        if (claim.Status != ClaimStatus.Approved)
        {
            throw new InvalidOperationException(
                "Only approved claims can be settled.");
        }

        var performedBy = request.PerformedBy.Trim();
        var now = DateTime.UtcNow;

        claim.Status = ClaimStatus.Settled;
        claim.UpdatedAtUtc = now;

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            Action = "ClaimSettled",
            Details = $"Claim settled for the estimated amount of {claim.EstimatedAmount:N2}.",
            PerformedBy = performedBy,
            CreatedAtUtc = now
        };

        _context.ClaimAudits.Add(audit);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
