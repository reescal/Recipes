using MediatR;

namespace Recipes.Features.Materials.GetById;

public class MaterialGetRequest : IRequest<MaterialGetResponse>
{
    public Guid Id { get; set; }
}
