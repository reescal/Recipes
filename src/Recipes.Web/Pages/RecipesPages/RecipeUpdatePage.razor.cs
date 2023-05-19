using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Recipes.Features.Recipes.Update;
using Recipes.Features.Recipes.GetById;
using Recipes.Web.Services;
using Recipes.Data.Entities;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Web.Pages.RecipesPages;

public partial class RecipeUpdatePage
{
    [Inject]
    public IRecipesService _recipesService { get; set; }
    [Inject]
    public IMessageService _message { get; set; }
    [Inject]
    public NavigationManager _navManager { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    private Form<RecipeUpdateRequest> _form;

    private RecipeUpdateRequest recipeUpdate = new RecipeUpdateRequest();

    private IEnumerable<RecipeGetResponse> recipes { get; set; } = new List<RecipeGetResponse>();
    private RecipeGetResponse recipe { get; set; } = new();

    private bool submitDisabled;

    protected override async Task OnInitializedAsync() => recipes = await _recipesService.Get();
    protected override async Task OnParametersSetAsync()
    {
        recipe = await _recipesService.Get(Id);
        if (recipe == null)
        {
            await _message.Error(NotFound(nameof(Ingredient), Id));
            _navManager.NavigateTo("/Recipes");
        }
        recipeUpdate = new RecipeUpdateRequest
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Type = recipe.Type,
            Description = recipe.Description,
            Image = recipe.Image,
            Video = recipe.Video,
            Yield = recipe.Yield,
            Time = recipe.Time,
            Ingredients = recipe.Ingredients.Select(i => new IngredientRow
            {
                IngredientId = i.Ingredient.Id,
                Preparation = i.Preparation,
                IsOptional = i.IsOptional,
                Quantity = i.Quantity
            }).ToList(),
            Materials = recipe.Materials.Select(m => new RecipeMaterial
            {
                MaterialId = m.Id
            }).ToList()
        };
    }

    private HashSet<string> RecipeTypes => recipes.Select(i => i.Type).ToHashSet();

    private async Task OnFinish(EditContext editContext)
    {
        submitDisabled = true;
        var result = await _recipesService.Update(recipeUpdate);
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
