
using MediatR;

namespace Claims.Application.Features.Claims.Commands.ProvideInformation;

public record ProvideInformationCommand(
    Guid ClaimId,
    string AdditionalInformation,
    string PerformedBy
) : IRequest<bool>;
