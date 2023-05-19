using AntDesign;
using Microsoft.AspNetCore.Components;
using Recipes.Features.GroceryList.Get;
using Recipes.Features.Ingredients.GetById;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.Ingredients;

public partial class IngredientsIndexPage
{
    [Inject]
    public IIngredientsService _ingredientsService { get; set; }
    [Inject]
    public IGroceryListService _groceryListService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    private IEnumerable<IngredientGetResponse> ingredients { get; set; } = new List<IngredientGetResponse>();

    private string searchTerm = string.Empty;

    private ListGridType grid = new()
    {
        Gutter = 16,
        Xs = 1,
        Sm = 2,
        Md = 4,
        Lg = 4,
        Xl = 6,
        Xxl = 3,
    };

    protected override async Task OnInitializedAsync() => ingredients = (await _ingredientsService.Get()).OrderBy(x => x.Type).ThenBy(x => x.Name);

    private IEnumerable<IGrouping<string, IngredientGetResponse>> filteredIngredients =>
        string.IsNullOrEmpty(searchTerm) ?
        ingredients.GroupBy(x => x.Type) :
        ingredients
            .Where(x => (x.Type + x.Name).Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))
            .GroupBy(x => x.Type);

    private async Task AddToGroceryList(IngredientGetResponse ingredient)
    {
        var result = await _groceryListService.AddGrocery(AsGrocery(ingredient));
        if (result)
            await _message.Success("Ingredient added succesfully to grocery list");
        else
            await _message.Error("Error adding ingredient to grocery list");
    }

    private HashSet<GroceryResponse> AsGrocery(IngredientGetResponse ingredient) => new HashSet<GroceryResponse>
    {
        new()
        {
            Ingredient = ingredient,
            Quantity = new()
            {
                Value = 1,
                Unit = "unit"
            }
        }
    };

    private class SimpleEntity
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
