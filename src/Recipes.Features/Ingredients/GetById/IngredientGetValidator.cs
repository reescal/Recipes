using FluentValidation;
using Recipes.Shared.Constants;

namespace Recipes.Features.Ingredients.GetById;
public class IngredientGetValidator : AbstractValidator<IngredientGetRequest>
{
    public IngredientGetValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationError.Required(nameof(IngredientGetRequest.Id)));
    }
}
