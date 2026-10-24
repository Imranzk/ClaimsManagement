
using Claims.Application.Common.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Commands.ProvideInformation;

public class ProvideInformationCommandHandler
    : IRequestHandler<ProvideInformationCommand, bool>
{
    private readonly IAppDbContext _context;

    public ProvideInformationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        ProvideInformationCommand request,
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

        if (claim.Status != ClaimStatus.InformationRequested)
        {
            throw new InvalidOperationException(
                "Additional information can only be provided when information has been requested.");
        }

        var additionalInformation = request.AdditionalInformation.Trim();
        var performedBy = request.PerformedBy.Trim();
        var now = DateTime.UtcNow;

        claim.AdditionalInformation = additionalInformation;
        claim.Status = ClaimStatus.InformationProvided;
        claim.UpdatedAtUtc = now;

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            Action = "InformationProvided",
            Details = additionalInformation,
            PerformedBy = performedBy,
            CreatedAtUtc = now
        };

        _context.ClaimAudits.Add(audit);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
