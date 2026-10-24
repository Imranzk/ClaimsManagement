
using FluentValidation;

namespace Claims.Application.Features.Claims.Commands.ProvideInformation;

public class ProvideInformationCommandValidator
    : AbstractValidator<ProvideInformationCommand>
{
    public ProvideInformationCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty()
            .WithMessage("Claim ID is required.");

        RuleFor(x => x.AdditionalInformation)
            .NotEmpty()
            .WithMessage("Additional information is required.")
            .MaximumLength(2000)
            .WithMessage("Additional information cannot exceed 2000 characters.");

        RuleFor(x => x.PerformedBy)
            .NotEmpty()
            .WithMessage("The person providing the information is required.")
            .MaximumLength(150)
            .WithMessage("The performer name cannot exceed 150 characters.");
    }
}
