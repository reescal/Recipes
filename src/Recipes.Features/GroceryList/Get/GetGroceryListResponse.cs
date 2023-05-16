using Recipes.Features.Ingredients.GetById;
using Recipes.Shared.Models;

namespace Recipes.Features.GroceryList.Get;
public class GetGroceryListResponse
{
    public Guid Id { get; set; }
    public HashSet<GroceryResponse> Grocery { get; set; }
}

public class GroceryResponse
{
    public IngredientGetResponse Ingredient { get; set; }
    public Quantity Quantity { get; set; }
}
