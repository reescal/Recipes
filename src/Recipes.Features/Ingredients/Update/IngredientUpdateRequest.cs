using MediatR;
using Recipes.Features.Ingredients.GetById;
using Recipes.Shared.Models;

namespace Recipes.Features.Ingredients.Update;

public class IngredientUpdateRequest : IRequest<IngredientGetResponse>
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IndexHashSet<IngredientProperties> Properties { get; set; }
}
