using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Materials.GetById;
using Recipes.Shared.Models;

namespace Recipes.Features.Recipes.GetById;
public class RecipeGetResponse
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
    public List<IngredientRowResponse> Ingredients { get; set; }
    public List<MaterialGetResponse> Materials { get; set; }
}

public class IngredientRowResponse
{
    public IngredientGetResponse Ingredient { get; set; }
    public Quantity Quantity { get; set; }
    public string Preparation { get; set; }
    public bool IsOptional { get; set; }
}
