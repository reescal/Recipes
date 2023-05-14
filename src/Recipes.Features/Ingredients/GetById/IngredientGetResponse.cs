using Recipes.Shared.Models;

namespace Recipes.Features.Ingredients.GetById;
public class IngredientGetResponse
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IndexHashSet<IngredientProperties> Properties { get; set; }
}
