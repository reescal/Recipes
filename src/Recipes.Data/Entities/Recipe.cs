using Recipes.Shared.Models;

namespace Recipes.Data.Entities;

public class Recipe
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Preparation { get; set; }
    public string Type { get; set; }
    public List<string> Tags { get; set; }
    public List<IngredientRow> Ingredients { get; set; }
    public List<RecipeMaterial> Materials { get; set; }
}

public class IngredientRow
{
    public Guid IngredientId { get; set; }
    public Quantity Quantity { get; set; }
    public string Preparation { get; set; }
    public bool IsOptional { get; set; }
}

public class RecipeMaterial
{
    public Guid MaterialId { get; set; }
}