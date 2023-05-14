using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Shared;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Ingredients.GetById;

public class IngredientGetHandler : IRequestHandler<IngredientGetRequest, IngredientGetResponse>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public IngredientGetHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<IngredientGetResponse> Handle(IngredientGetRequest request, CancellationToken cancellationToken)
    {
        var ingredient = await _docsContext.Ingredients.FindAsync(request.Id) 
                    ?? throw new ApiException(System.Net.HttpStatusCode.NotFound, NotFound(nameof(Ingredient), request.Id));

        var response = _mapper.Map<IngredientGetResponse>(ingredient);

        return response;
    }
}
