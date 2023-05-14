using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Shared;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Materials.GetById;

public class MaterialGetHandler : IRequestHandler<MaterialGetRequest, MaterialGetResponse>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public MaterialGetHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<MaterialGetResponse> Handle(MaterialGetRequest request, CancellationToken cancellationToken)
    {
        var material = await _docsContext.Materials.FindAsync(request.Id)
                    ?? throw new ApiException(System.Net.HttpStatusCode.NotFound, NotFound(nameof(Material), request.Id));

        var response = _mapper.Map<MaterialGetResponse>(material);

        return response;
    }
}
