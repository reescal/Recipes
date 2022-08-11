using Recipes.Api.Interfaces;
using System;
using System.Collections.Generic;

namespace Recipes.Api.Models;

public class Material : MaterialCreate, IEntity
{
    public Guid Id { get; set; }
}

public class MaterialCreate
{
    public string Image { get; set; }
    public HashSet<MaterialProperties> Properties { get; set; }
}

public class MaterialProperties : IEntityProperties
{
    public int LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
