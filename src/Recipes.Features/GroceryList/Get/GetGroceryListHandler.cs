using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.GroceryList.Get;
public class GetGroceryListHandler : IRequestHandler<GetGroceryListRequest, GetGroceryListResponse>
{
    private readonly DocsContext _docsContext;
    private readonly IMapper _mapper;

    public GetGroceryListHandler(IDbContextFactory<DocsContext> docsContext, IMapper mapper)
    {
        _docsContext = docsContext.CreateDbContext();
        _mapper = mapper;
    }

    public async Task<GetGroceryListResponse> Handle(GetGroceryListRequest request, CancellationToken cancellationToken)
    {
        var groceryList = _docsContext.GroceryList.FirstOrDefault() ?? await _docsContext.CreateGroceryListIfNotExist();
        var ingredientIds = groceryList.Grocery.Select(g => g.IngredientId);
        var ingredients = _docsContext.Ingredients.Where(i => ingredientIds.Contains(i.Id)).AsEnumerable();
        var groceryListResponse = _mapper.Map<GetGroceryListResponse>(groceryList);
        foreach (var grocery in groceryListResponse.Grocery)
        {
            grocery.Ingredient = _mapper.Map<IngredientGetResponse>(ingredients.First(x => x.Id == grocery.Ingredient.Id));
        }
        return groceryListResponse;
    }
}
