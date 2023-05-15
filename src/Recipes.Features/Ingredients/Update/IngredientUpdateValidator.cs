using FluentValidation;
using Recipes.Shared.Constants;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;

namespace Recipes.Features.Ingredients.Update;
public class IngredientUpdateValidator : AbstractValidator<IngredientUpdateRequest>
{
    public IngredientUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(IngredientUpdateRequest.Id)));
        RuleFor(p => p.Image).NotEmpty()
                            .WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri)
                                .When(p => p.Image != null)
                                .WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Name).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(IngredientUpdateRequest.Name)));
        RuleFor(p => p.Name).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooShort(nameof(IngredientUpdateRequest.Name)));
        RuleFor(p => p.Name).MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooLong(nameof(IngredientUpdateRequest.Name)));
        RuleFor(p => p.Description).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(IngredientUpdateRequest.Description)));
        RuleFor(p => p.Type).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(IngredientUpdateRequest.Type)));
        RuleFor(p => p.Type).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooShort(nameof(IngredientUpdateRequest.Type)));
        RuleFor(p => p.Type).MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooLong(nameof(IngredientUpdateRequest.Type)));
    }
}
