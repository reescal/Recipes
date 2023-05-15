using MediatR;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.Ingredients.Update;

public class IngredientUpdateRequest : IRequest<IngredientGetResponse>
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}
