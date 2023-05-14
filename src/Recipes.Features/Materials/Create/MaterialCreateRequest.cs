using MediatR;
using Recipes.Shared.Models;

namespace Recipes.Features.Materials.Create;

public class MaterialCreateRequest : IRequest<Guid>
{
    public string Image { get; set; }
    public IndexHashSet<MaterialProperties> Properties { get; set; }
}
