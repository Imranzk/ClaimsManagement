
using MediatR;

namespace Claims.Application.Features.Claims.Commands.RejectClaim;

public record RejectClaimCommand(
    Guid ClaimId,
    string DecisionReason,
    string PerformedBy
) : IRequest<bool>;
