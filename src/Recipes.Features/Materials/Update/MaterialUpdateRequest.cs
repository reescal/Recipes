using MediatR;
using Recipes.Features.Materials.GetById;
using Recipes.Shared.Models;

namespace Recipes.Features.Materials.Update;

public class MaterialUpdateRequest : IRequest<MaterialGetResponse>
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IndexHashSet<MaterialProperties> Properties { get; set; }
}
