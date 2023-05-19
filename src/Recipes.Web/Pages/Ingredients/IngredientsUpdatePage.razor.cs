using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Ingredients.Update;
using Recipes.Web.Services;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Web.Pages.Ingredients;

public partial class IngredientsUpdatePage
{
    [Inject]
    public IIngredientsService _ingredientsService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    [Parameter]
    public Guid Id { get; set; }
    private Form<IngredientUpdateRequest> _form;

    private IngredientUpdateRequest ingredientUpdate = new IngredientUpdateRequest();

    private IEnumerable<IngredientGetResponse> ingredients { get; set; } = new List<IngredientGetResponse>();
    private IngredientGetResponse ingredient { get; set; } = new();

    private bool submitDisabled;


    protected override async Task OnInitializedAsync() => ingredients = await _ingredientsService.Get();

    protected override async Task OnParametersSetAsync()
    {
        ingredient = await _ingredientsService.Get(Id);
        if (ingredient == null)
        {
            await _message.Error(NotFound(nameof(Ingredient), Id));
            _navManager.NavigateTo("/Ingredients");
        }
        ingredientUpdate = new IngredientUpdateRequest
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Type = ingredient.Type,
            Description = ingredient.Description,
            Image = ingredient.Image
        };
    }

    private HashSet<string> IngredientTypes => ingredients.Select(i => i.Type).ToHashSet();

    private async Task OnFinish(EditContext editContext)
    {
        submitDisabled = true;
        var result = await _ingredientsService.Update(ingredientUpdate);
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
