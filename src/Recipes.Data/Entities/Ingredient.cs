namespace Recipes.Data.Entities;

public class Ingredient
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string NutritionalInfo { get; set; }
}