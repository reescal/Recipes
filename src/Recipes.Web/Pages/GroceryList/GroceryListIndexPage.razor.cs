using Microsoft.AspNetCore.Components;
using Recipes.Features.GroceryList.Get;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.GroceryList;

public partial class GroceryListIndexPage
{
    [Inject]
    public IGroceryListService _groceryListService { get; set; }
    private GetGroceryListResponse groceryList = new();
    private Guid editId;

    protected override async Task OnInitializedAsync() => groceryList = await _groceryListService.Get();

    private void Delete(GroceryResponse grocery) => groceryList.Grocery.Remove(grocery);

    private void startEdit(Guid id) => editId = id;

    private void stopEdit() => editId = Guid.Empty;

    private async Task Update() => await _groceryListService.Update(groceryList.Grocery);
}