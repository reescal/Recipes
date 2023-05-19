using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Recipes.Features.Recipes.Create;
using Recipes.Features.Recipes.GetById;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.RecipesPages;

public partial class RecipeCreatePage
{
    [Inject]
    public IRecipesService _recipesService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    private Form<RecipeCreateRequest> _form;

    private RecipeCreateRequest recipe = new RecipeCreateRequest();

    private IEnumerable<RecipeGetResponse> recipes { get; set; } = new List<RecipeGetResponse>();

    private bool submitDisabled;

    protected override async Task OnInitializedAsync() => recipes = await _recipesService.Get();

    private HashSet<string> RecipeTypes => recipes.Select(i => i.Type).ToHashSet();

    private async Task OnFinish(EditContext editContext)
    {
        submitDisabled = true;
        var result = await _recipesService.Create(recipe);
        if (result.Valid)
        {
            await _message.Success(result.Message);
            _navManager.NavigateTo("/Recipes");
        }
        else
        {
            var errors = result.Message.Split('\n');
            var tasks = new List<Task>();
            foreach (var error in errors)
            {
                tasks.Add(_message.Error(error));
            }
            await Task.WhenAll(tasks);
        }
        submitDisabled = false;
    }
}
