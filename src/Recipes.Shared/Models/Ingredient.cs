using Recipes.Shared.Interfaces;

namespace Recipes.Shared.Models;

public class Ingredient : IngredientCreate, IEntity
{
    public Guid Id { get; set; }
}

public class IngredientCreate
{
    public string Image { get; set; }
    public HashSet<IngredientProperties> Properties { get; set; }
}

public class IngredientProperties : IEntityProperties
{
    public int LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}