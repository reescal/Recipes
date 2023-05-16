using MediatR;
using Recipes.Features.Materials.GetById;

namespace Recipes.Features.Materials.GetAll;

public class MaterialsGetAllRequest : IRequest<IEnumerable<MaterialGetResponse>>
{
}
