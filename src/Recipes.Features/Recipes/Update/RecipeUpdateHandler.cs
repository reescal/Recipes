using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Features.Recipes.GetById;
using Recipes.Shared;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Recipes.Update;
internal class RecipeUpdateHandler : IRequestHandler<RecipeUpdateRequest, RecipeGetResponse>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipeUpdateHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<RecipeGetResponse> Handle(RecipeUpdateRequest request, CancellationToken cancellationToken)
    {
        var recipe = await _docsContext.Recipes.FindAsync(request.Id)
                ?? throw new ApiException(System.Net.HttpStatusCode.NotFound, NotFound(nameof(Recipe), request.Id));

        recipe = _mapper.Map<Recipe>(request);

        await _docsContext.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<RecipeGetResponse>(recipe);
        return response;
    }
}
