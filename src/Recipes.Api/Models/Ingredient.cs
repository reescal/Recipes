using Recipes.Api.Interfaces;
using System;
using System.Collections.Generic;

namespace Recipes.Api.Models;

public class Ingredient : IngredientCreate, IEntity
{
    public Guid Id { get; set; }
}

public class IngredientCreate
{
    public string Image { get; set; }
    public HashSet<IngredientProperties> Properties { get; set; }
}

public class IngredientProperties
{
    public int LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
