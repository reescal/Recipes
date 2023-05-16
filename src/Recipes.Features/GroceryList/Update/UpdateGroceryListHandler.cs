using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;

namespace Recipes.Features.GroceryList.Update;
public class UpdateGroceryListHandler : IRequestHandler<UpdateGroceryListRequest, Unit>
{
    private readonly DocsContext _docsContext;

    public UpdateGroceryListHandler(IDbContextFactory<DocsContext> docsContext)
    {
        _docsContext = docsContext.CreateDbContext();
    }

    public async Task<Unit> Handle(UpdateGroceryListRequest request, CancellationToken cancellationToken)
    {
        var validGroceries = request.Grocery.Where(x => _docsContext.Ingredients.Find(x.IngredientId) != null).ToHashSet();

        var groceryList = await _docsContext.CreateGroceryListIfNotExist();
        groceryList.Grocery = validGroceries;
        await _docsContext.SaveChangesAsync(cancellationToken);

        return new Unit();
    }
}
