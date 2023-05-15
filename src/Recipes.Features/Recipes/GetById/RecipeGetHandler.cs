using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Shared;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Recipes.GetById;

public class RecipeGetHandler : IRequestHandler<RecipeGetRequest, RecipeGetResponse>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipeGetHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<RecipeGetResponse> Handle(RecipeGetRequest request, CancellationToken cancellationToken)
    {
        var recipe = await _docsContext.Recipes.FindAsync(request.Id)
                    ?? throw new ApiException(System.Net.HttpStatusCode.NotFound, NotFound(nameof(Recipe), request.Id));

        var response = recipe.IncludeIngredientsAndMaterials(_docsContext, _mapper);

        return response;
    }
}
