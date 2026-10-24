
using FluentValidation;

namespace Claims.Application.Features.Claims.Commands.ApproveClaim;

public class ApproveClaimCommandValidator
    : AbstractValidator<ApproveClaimCommand>
{
    public ApproveClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty()
            .WithMessage("Claim ID is required.");

        RuleFor(x => x.DecisionReason)
            .NotEmpty()
            .WithMessage("The approval reason is required.")
            .MaximumLength(2000)
            .WithMessage("The approval reason cannot exceed 2000 characters.");

        RuleFor(x => x.PerformedBy)
            .NotEmpty()
            .WithMessage("The person making the decision is required.")
            .MaximumLength(150)
            .WithMessage("The performer name cannot exceed 150 characters.");
    }
}
