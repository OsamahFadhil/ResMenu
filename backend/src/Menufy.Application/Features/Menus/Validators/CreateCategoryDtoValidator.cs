using FluentValidation;
using Menufy.Application.Features.Menus.DTOs;

namespace Menufy.Application.Features.Menus.Validators;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.ParentCategoryId)
            .Must(id => id == null || id != Guid.Empty)
            .WithMessage("Parent category id must be null or a valid identifier.");
    }
}

