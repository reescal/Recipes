using AntDesign;
using Microsoft.AspNetCore.Components;
using Recipes.Features.GroceryList.Get;
using Recipes.Features.Recipes.GetById;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.RecipesPages;

public partial class RecipesIndexPage
{
    [Inject]
    public IRecipesService _iRecipesService { get; set; }
    [Inject]
    public IGroceryListService _iGroceryListService { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    private string searchName;
    private IEnumerable<string> selectedTypes, selectedIngredients, selectedMaterials, selectedTags;

    private IEnumerable<RecipeGetResponse> recipes;

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

    protected override async Task OnInitializedAsync() => recipes = await _iRecipesService.Get();

    private IEnumerable<IGrouping<string, RecipeGetResponse>> filteredRecipes => recipes.OrderBy(x => x.Type).ThenBy(x => x.Name).GroupBy(x => x.Type);

    private HashSet<GroceryResponse> AsGroceries(RecipeGetResponse recipe) => recipe.Ingredients.Select(x => new GroceryResponse
    {
        Ingredient = x.Ingredient,
        Quantity = new()
        {
            Value = 1,
            Unit = "unit"
        }
    }).Distinct().ToHashSet();

    private IEnumerable<string> recipeNames => recipes.Select(x => x.Name);
    private IEnumerable<string> recipeTypes => recipes.Select(x => x.Type);
    private IEnumerable<string> recipeMaterials => recipes.SelectMany(r => r.Materials, (r, m) => m.Name);
    private IEnumerable<string> recipeIngredients => recipes.SelectMany(r => r.Ingredients, (r, i) => i.Ingredient.Name);
    private IEnumerable<string> recipeTags => recipes.SelectMany(r => r.Tags, (r, t) => t);
}
