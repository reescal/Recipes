using FluentValidation;
using Recipes.Features.Ingredients.Create;
using Recipes.Shared.Constants;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;

namespace Recipes.Features.Ingredients.Update;
public class IngredientUpdateValidator : AbstractValidator<IngredientUpdateRequest>
{
    public IngredientUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationError.Required(nameof(IngredientUpdateRequest.Id)));
        RuleFor(p => p.Image).NotEmpty().WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri).When(p => p.Image != null).WithMessage(ValidationError.Invalid("image link"));
        RuleFor(x => x.Properties).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(IngredientUpdateRequest.Properties)));
        RuleForEach(x => x.Properties).SetValidator(new IngredientPropertiesValidator());
    }
}
