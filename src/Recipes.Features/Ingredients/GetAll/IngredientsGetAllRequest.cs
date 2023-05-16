using MediatR;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.Ingredients.GetAll;

public class IngredientsGetAllRequest : IRequest<IEnumerable<IngredientGetResponse>>
{
}
