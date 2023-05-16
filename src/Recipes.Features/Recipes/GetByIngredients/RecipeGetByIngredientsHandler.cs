using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetByIngredients;

public class RecipeGetByIngredientsHandler : IRequestHandler<RecipeGetByIngredientsRequest, IEnumerable<RecipeGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipeGetByIngredientsHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public Task<IEnumerable<RecipeGetResponse>> Handle(RecipeGetByIngredientsRequest request, CancellationToken cancellationToken)
    {
        var recipes = _docsContext.Recipes.AsNoTracking().AsEnumerable();
        var recipeByIngredients = recipes.Where(x => x.Ingredients.Any(y => request.Ingredients.Contains(y.IngredientId)));

        var response = recipeByIngredients.IncludeIngredientsAndMaterials(_docsContext, _mapper);

        return Task.FromResult(response);
    }
}
