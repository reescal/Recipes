using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Features.Materials.GetById;
using Recipes.Shared;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Materials.Update;
internal class MaterialUpdateHandler : IRequestHandler<MaterialUpdateRequest, MaterialGetResponse>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public MaterialUpdateHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<MaterialGetResponse> Handle(MaterialUpdateRequest request, CancellationToken cancellationToken)
    {
        var material = await _docsContext.Materials.FindAsync(request.Id)
                    ?? throw new ApiException(System.Net.HttpStatusCode.NotFound, NotFound(nameof(Material), request.Id));

        material = _mapper.Map<Material>(request);

        await _docsContext.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<MaterialGetResponse>(material);
        return response;
    }
}
