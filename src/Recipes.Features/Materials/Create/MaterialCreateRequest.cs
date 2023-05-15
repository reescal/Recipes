using MediatR;

namespace Recipes.Features.Materials.Create;

public class MaterialCreateRequest : IRequest<Guid>
{
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}
