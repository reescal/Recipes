using Recipes.Shared.Models;

namespace Recipes.Data.Entities;
public class GroceryList
{
    public Guid Id { get; set; }
    public HashSet<Grocery> Grocery { get; set; } = new HashSet<Grocery> { };
}

public class Grocery
{
    public Guid IngredientId { get; set; }
    public Quantity Quantity { get; set; }
}