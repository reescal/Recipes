using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Recipes.Features.Ingredients.Create;
using Recipes.Features.Ingredients.GetById;
using Recipes.Web.Services;

namespace Recipes.Web.Pages.Ingredients;

public partial class IngredientsCreatePage
{
    [Inject]
    public IIngredientsService _ingredientsService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    private Form<IngredientCreateRequest> _form;

    private IngredientCreateRequest ingredient = new IngredientCreateRequest();

    private IEnumerable<IngredientGetResponse> ingredients { get; set; } = new List<IngredientGetResponse>();

    private bool submitDisabled;

    protected override async Task OnInitializedAsync() => ingredients = await _ingredientsService.Get();

    private HashSet<string> IngredientTypes => ingredients.Select(i => i.Type).ToHashSet();

    private async Task OnFinish(EditContext editContext)
    {
        submitDisabled = true;
        var result = await _ingredientsService.Add(ingredient);
        if (result.Valid)
        {
            await _message.Success(result.Message);
            _navManager.NavigateTo("/Ingredients");
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
