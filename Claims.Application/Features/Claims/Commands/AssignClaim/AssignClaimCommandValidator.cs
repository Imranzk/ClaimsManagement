
using FluentValidation;

namespace Claims.Application.Features.Claims.Commands.AssignClaim;

public class AssignClaimCommandValidator : AbstractValidator<AssignClaimCommand>
{
    public AssignClaimCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty()
            .WithMessage("Claim ID is required.");

        RuleFor(x => x.AssignedOfficer)
            .NotEmpty()
            .WithMessage("Assigned officer is required.")
            .MaximumLength(150)
            .WithMessage("Assigned officer cannot exceed 150 characters.");

        RuleFor(x => x.PerformedBy)
            .NotEmpty()
            .WithMessage("The person performing the assignment is required.")
            .MaximumLength(150)
            .WithMessage("The performer name cannot exceed 150 characters.");
    }
}
