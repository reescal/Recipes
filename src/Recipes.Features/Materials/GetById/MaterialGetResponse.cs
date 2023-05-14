using Recipes.Shared.Models;

namespace Recipes.Features.Materials.GetById;
public class MaterialGetResponse
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IndexHashSet<MaterialProperties> Properties { get; set; }
}
