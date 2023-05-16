using Recipes.Data;

namespace Recipes.Features.GroceryList;
public static class Helpers
{
    public static async Task<Data.Entities.GroceryList> CreateGroceryListIfNotExist(this DocsContext _docsContext)
    {
        var groceryList = _docsContext.GroceryList.FirstOrDefault();
        if (groceryList == null)
        {
            groceryList = new();
            _docsContext.GroceryList.Add(groceryList);
            await _docsContext.SaveChangesAsync();
        }

        return groceryList;
    }
}
