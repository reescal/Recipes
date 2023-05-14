using MediatR;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetByIngredients;
public class RecipeGetByIngredientsRequest : IRequest<List<RecipeGetResponse>>
{
    public List<Guid> Ingredients { get; set; }
}
