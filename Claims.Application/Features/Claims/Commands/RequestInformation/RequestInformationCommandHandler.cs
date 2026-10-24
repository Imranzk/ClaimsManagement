
using Claims.Application.Common.Interfaces;
using Claims.Domain.Entities;
using Claims.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Claims.Application.Features.Claims.Commands.RequestInformation;

public class RequestInformationCommandHandler
    : IRequestHandler<RequestInformationCommand, bool>
{
    private readonly IAppDbContext _context;

    public RequestInformationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        RequestInformationCommand request,
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

        if (claim.Status != ClaimStatus.UnderReview)
        {
            throw new InvalidOperationException(
                "Information can only be requested for claims under review.");
        }

        var informationRequest = request.InformationRequest.Trim();
        var performedBy = request.PerformedBy.Trim();
        var now = DateTime.UtcNow;

        claim.AdditionalInformation = informationRequest;
        claim.Status = ClaimStatus.InformationRequested;
        claim.UpdatedAtUtc = now;

        var audit = new ClaimAudit
        {
            ClaimId = claim.Id,
            Action = "InformationRequested",
            Details = informationRequest,
            PerformedBy = performedBy,
            CreatedAtUtc = now
        };

        _context.ClaimAudits.Add(audit);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
