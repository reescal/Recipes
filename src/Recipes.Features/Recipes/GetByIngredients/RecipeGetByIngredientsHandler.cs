using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetByIngredients;

public class RecipeGetByIngredientsHandler : IRequestHandler<RecipeGetByIngredientsRequest, List<RecipeGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipeGetByIngredientsHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public Task<List<RecipeGetResponse>> Handle(RecipeGetByIngredientsRequest request, CancellationToken cancellationToken)
    {
        var recipes = _docsContext.Recipes.AsNoTracking().AsEnumerable();
        var recipeByIngredients = recipes.Where(x => x.Ingredients.Any(y => request.Ingredients.Contains(y.IngredientId)));

        var response = _mapper.Map<List<RecipeGetResponse>>(recipeByIngredients);

        return Task.FromResult(response);
    }
}
