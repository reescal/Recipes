using MediatR;

namespace Recipes.Features.Recipes.GetById;

public class RecipeGetRequest : IRequest<RecipeGetResponse>
{
    public Guid Id { get; set; }
}
