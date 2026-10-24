
using MediatR;

namespace Claims.Application.Features.Claims.Commands.RequestInformation;

public record RequestInformationCommand(
    Guid ClaimId,
    string InformationRequest,
    string PerformedBy
) : IRequest<bool>;
