
using MediatR;

namespace Claims.Application.Features.Claims.Commands.AssignClaim;

public record AssignClaimCommand(
    Guid ClaimId,
    string AssignedOfficer,
    string PerformedBy
) : IRequest<bool>;
