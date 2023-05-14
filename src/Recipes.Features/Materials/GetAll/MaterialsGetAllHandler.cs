using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Materials.GetById;

namespace Recipes.Features.Materials.GetAll;

public class MaterialsGetAllHandler : IRequestHandler<MaterialsGetAllRequest, List<MaterialGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public MaterialsGetAllHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<List<MaterialGetResponse>> Handle(MaterialsGetAllRequest request, CancellationToken cancellationToken)
    {
        var materials = await _docsContext.Materials.ToListAsync();

        var response = _mapper.Map<List<MaterialGetResponse>>(materials);

        return response;
    }
}
