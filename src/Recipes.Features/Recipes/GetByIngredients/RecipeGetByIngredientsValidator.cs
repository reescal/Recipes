using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Shared.Constants;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Recipes.GetByIngredients;

public class RecipeGetByIngredientsValidator : AbstractValidator<RecipeGetByIngredientsRequest>
{
    public RecipeGetByIngredientsValidator(IDbContextFactory<DocsContext> factory)
    {
        var _docsContext = factory.CreateDbContext();
        RuleFor(x => x.Ingredients).NotEmpty().WithMessage(ValidationError.Required(nameof(RecipeGetByIngredientsRequest.Ingredients)));
        RuleForEach(x => x.Ingredients)
            .NotEmpty()
            .WithMessage(ValidationError.Required("Ingredient id cannot be empty"));
        RuleForEach(x => x.Ingredients)
            .Must(x => _docsContext.Ingredients.Find(x) != null)
            .Unless(x => x.Ingredients.Any(i => i == Guid.Empty))
            .WithMessage(NotFound(nameof(Ingredient)));
    }
}
