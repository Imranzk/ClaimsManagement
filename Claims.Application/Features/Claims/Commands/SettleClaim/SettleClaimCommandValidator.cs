
using FluentValidation;

namespace Claims.Application.Features.Claims.Commands.SettleClaim;

public class SettleClaimCommandValidator
    : AbstractValidator<SettleClaimCommand>
{
    public SettleClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty()
            .WithMessage("Claim ID is required.");

        RuleFor(x => x.PerformedBy)
            .NotEmpty()
            .WithMessage("The person recording the settlement is required.")
            .MaximumLength(150)
            .WithMessage("The performer name cannot exceed 150 characters.");
    }
}
