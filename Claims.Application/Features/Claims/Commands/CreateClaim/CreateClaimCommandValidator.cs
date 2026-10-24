
using FluentValidation;

namespace Claims.Application.Features.Claims.Commands.CreateClaim;

public class CreateClaimCommandValidator
    : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.Claim.ClaimantName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Claim.ClaimantEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Claim.Market)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Claim.IncidentDescription)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Claim.IncidentDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow);

        RuleFor(x => x.Claim.EstimatedAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Claim.ClaimType)
            .IsInEnum();
    }
}
