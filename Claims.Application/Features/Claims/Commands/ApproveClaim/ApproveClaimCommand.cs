
using MediatR;

namespace Claims.Application.Features.Claims.Commands.ApproveClaim;

public record ApproveClaimCommand(
    Guid ClaimId,
    string DecisionReason,
    string PerformedBy
) : IRequest<bool>;
