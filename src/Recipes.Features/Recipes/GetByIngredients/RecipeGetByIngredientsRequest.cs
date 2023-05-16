using MediatR;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetByIngredients;
public class RecipeGetByIngredientsRequest : IRequest<IEnumerable<RecipeGetResponse>>
{
    public IEnumerable<Guid> Ingredients { get; set; }
}
