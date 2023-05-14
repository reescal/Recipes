using MediatR;
using Recipes.Shared.Models;

namespace Recipes.Features.Ingredients.Create;

public class IngredientCreateRequest : IRequest<Guid>
{
    public string Image { get; set; }
    public IndexHashSet<IngredientProperties> Properties { get; set; }
}
