
using MediatR;

namespace Claims.Application.Features.Claims.Commands.SettleClaim;

public record SettleClaimCommand(
    Guid ClaimId,
    string PerformedBy
) : IRequest<bool>;
