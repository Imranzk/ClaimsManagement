
using FluentValidation;

namespace Claims.Application.Features.Claims.Commands.RequestInformation;

public class RequestInformationCommandValidator
    : AbstractValidator<RequestInformationCommand>
{
    public RequestInformationCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty()
            .WithMessage("Claim ID is required.");

        RuleFor(x => x.InformationRequest)
            .NotEmpty()
            .WithMessage("The information request is required.")
            .MaximumLength(2000)
            .WithMessage("The information request cannot exceed 2000 characters.");

        RuleFor(x => x.PerformedBy)
            .NotEmpty()
            .WithMessage("The person making the request is required.")
            .MaximumLength(150)
            .WithMessage("The performer name cannot exceed 150 characters.");
    }
}
