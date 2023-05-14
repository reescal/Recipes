using MediatR;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetAll;

public class RecipesGetAllRequest : IRequest<List<RecipeGetResponse>>
{
}
