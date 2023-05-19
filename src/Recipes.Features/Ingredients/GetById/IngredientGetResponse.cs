namespace Recipes.Features.Ingredients.GetById;
public class IngredientGetResponse
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string NutritionalInfo { get; set; }
}
