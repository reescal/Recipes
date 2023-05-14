using MediatR;

namespace Recipes.Features.Ingredients.GetById;

public class IngredientGetRequest : IRequest<IngredientGetResponse>
{
    public Guid Id { get; set; }
}
