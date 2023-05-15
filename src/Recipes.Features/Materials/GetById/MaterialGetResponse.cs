using Recipes.Shared.Models;

namespace Recipes.Features.Materials.GetById;
public class MaterialGetResponse
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}
