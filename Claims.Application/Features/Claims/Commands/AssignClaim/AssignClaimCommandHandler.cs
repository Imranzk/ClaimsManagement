
using Claims.Application.Common.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Commands.AssignClaim;

public class AssignClaimCommandHandler
    : IRequestHandler<AssignClaimCommand, bool>
{
    private readonly IAppDbContext _context;

    public AssignClaimCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        AssignClaimCommand request,
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

        if (claim.Status != ClaimStatus.Submitted)
        {
            throw new InvalidOperationException(
                "Only submitted claims can be assigned.");
        }

        var officer = request.AssignedOfficer.Trim();
        var performedBy = request.PerformedBy.Trim();

        claim.AssignedOfficer = officer;
        claim.Status = ClaimStatus.UnderReview;
        claim.UpdatedAtUtc = DateTime.UtcNow;

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            Action = "ClaimAssigned",
            Details = $"Claim assigned to {officer}.",
            PerformedBy = performedBy,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.ClaimAudits.Add(audit);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
