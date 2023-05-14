using FluentValidation;
using Recipes.Shared.Constants;

namespace Recipes.Features.Recipes.GetById;
public class RecipeGetValidator : AbstractValidator<RecipeGetRequest>
{
    public RecipeGetValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ValidationError.Required(nameof(RecipeGetRequest.Id)));
    }
}
