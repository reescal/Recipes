using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Materials.GetById;

namespace Recipes.Features.Materials.GetAll;

public class MaterialsGetAllHandler : IRequestHandler<MaterialsGetAllRequest, IEnumerable<MaterialGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public MaterialsGetAllHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public Task<IEnumerable<MaterialGetResponse>> Handle(MaterialsGetAllRequest request, CancellationToken cancellationToken)
    {
        var materials = _docsContext.Materials.AsEnumerable();

        var response = _mapper.Map<IEnumerable<MaterialGetResponse>>(materials);

        return Task.FromResult(response);
    }
}
