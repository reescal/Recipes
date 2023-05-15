using MediatR;
using Recipes.Features.Materials.GetById;

namespace Recipes.Features.Materials.Update;

public class MaterialUpdateRequest : IRequest<MaterialGetResponse>
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}
