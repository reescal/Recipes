using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Materials.GetById;
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

        var ingredientIds = recipe.Ingredients.Select(x => x.IngredientId);
        var ingredients = _docsContext.Ingredients.Where(x => ingredientIds.Contains(x.Id)).AsEnumerable();

        var materialIds = recipe.Materials.Select(x => x.MaterialId);
        var materials = _docsContext.Materials.Where(x => materialIds.Contains(x.Id)).ToList();

        var response = _mapper.Map<RecipeGetResponse>(recipe);

        response.Ingredients.ForEach(i => i.Ingredient = _mapper.Map<IngredientGetResponse>(ingredients.First(x => x.Id == i.Ingredient.Id)));
        response.Materials.ForEach(m => m = _mapper.Map<MaterialGetResponse>(materials.First(x => x.Id == m.Id)));

        return response;
    }
}
