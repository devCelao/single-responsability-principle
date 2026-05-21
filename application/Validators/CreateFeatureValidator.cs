using application.Dtos;
using FluentValidation;

namespace application.Validators;

public class CreateFeatureValidator : AbstractValidator<CreateFeatureDto>
{
    public CreateFeatureValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Permission)
            .NotEmpty().WithMessage("Permission is required.")
            .MaximumLength(500).WithMessage("Permission cannot exceed 500 characters.");
    }
}
