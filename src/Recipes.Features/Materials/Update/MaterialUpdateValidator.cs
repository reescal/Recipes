using FluentValidation;
using Recipes.Features.Materials.Create;
using Recipes.Shared.Constants;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;

namespace Recipes.Features.Materials.Update;
public class MaterialUpdateValidator : AbstractValidator<MaterialUpdateRequest>
{
    public MaterialUpdateValidator()
    {
        RuleFor(p => p.Image).NotEmpty()
                            .WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri)
                            .When(p => !string.IsNullOrWhiteSpace(p.Image))
                            .WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Name).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(MaterialCreateRequest.Name)));
        RuleFor(p => p.Name).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooShort(nameof(MaterialCreateRequest.Name)));
        RuleFor(p => p.Name).MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooLong(nameof(MaterialCreateRequest.Name)));
        RuleFor(p => p.Description).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(MaterialCreateRequest.Description)));
        RuleFor(p => p.Type).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(MaterialCreateRequest.Type)));
        RuleFor(p => p.Type).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooShort(nameof(MaterialCreateRequest.Type)));
        RuleFor(p => p.Type).MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooLong(nameof(MaterialCreateRequest.Type)));
    }
}
