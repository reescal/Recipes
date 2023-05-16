using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;

namespace Recipes.Features.GroceryList.AddGrocery;

public class AddGroceryHandler : IRequestHandler<AddGroceryRequest, Unit>
{
    private readonly DocsContext _docsContext;

    public AddGroceryHandler(IDbContextFactory<DocsContext> docsContext)
    {
        _docsContext = docsContext.CreateDbContext();
    }

    public async Task<Unit> Handle(AddGroceryRequest request, CancellationToken cancellationToken)
    {
        var validGroceries = request.Grocery.Where(x => _docsContext.Ingredients.Find(x.IngredientId) != null);
        if (!validGroceries.Any())
            return new Unit();

        var groceryList = await _docsContext.CreateGroceryListIfNotExist();

        foreach (var grocery in validGroceries)
        {
            var existingGrocery = groceryList.Grocery.First(x => x.IngredientId == grocery.IngredientId);
            if (existingGrocery != null)
                existingGrocery.Quantity.Value += grocery.Quantity.Value;
            else
                groceryList.Grocery.Add(grocery);
        }

        await _docsContext.SaveChangesAsync(cancellationToken);
        return new Unit();
    }
}