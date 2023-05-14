using Recipes.Shared.Models;

namespace Recipes.Data.Entities;

public class Material
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IndexHashSet<MaterialProperties> Properties { get; set; }
}