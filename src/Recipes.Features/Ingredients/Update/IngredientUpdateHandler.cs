using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.GetById;
using Recipes.Shared;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Ingredients.Update;
internal class IngredientUpdateHandler : IRequestHandler<IngredientUpdateRequest, IngredientGetResponse>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public IngredientUpdateHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<IngredientGetResponse> Handle(IngredientUpdateRequest request, CancellationToken cancellationToken)
    {
        var ingredient = await _docsContext.Ingredients.FindAsync(request.Id)
                    ?? throw new ApiException(System.Net.HttpStatusCode.NotFound, NotFound(nameof(Ingredient), request.Id));

        ingredient = _mapper.Map<Ingredient>(request);

        await _docsContext.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<IngredientGetResponse>(ingredient);
        return response;
    }
}
