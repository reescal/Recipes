using Recipes.Shared.Models;

namespace Recipes.Data.Entities;

public class Ingredient
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IndexHashSet<IngredientProperties> Properties { get; set; }
}