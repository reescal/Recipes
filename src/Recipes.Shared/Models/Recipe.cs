using Recipes.Shared.Interfaces;

namespace Recipes.Shared.Models;

public class Recipe : RecipeCreate, IEntity
{
    public Guid Id { get; set; }
}

public class RecipeCreate
{
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public HashSet<RecipeProperties> Properties { get; set; }
    public List<IngredientRow> Ingredients { get; set; }
}

public class RecipeResponse : IEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public HashSet<RecipeProperties> Properties { get; set; }
    public List<IngredientRowResponse> Ingredients { get; set; }
}

public class RecipeProperties : IEntityProperties
{
    public int LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Tags { get; set; }
}

public class IngredientRow
{
    public Guid IngredientId { get; set; }
    public Quantity Quantity { get; set; }
    public string Preparation { get; set; }
    public bool IsOptional { get; set; }
}

public class IngredientRowResponse
{
    public Ingredient Ingredient { get; set; }
    public Quantity Quantity { get; set; }
    public string Preparation { get; set; }
    public bool IsOptional { get; set; }
}

public class Quantity
{
    public double Value { get; set; }
    public string Unit { get; set; }
}