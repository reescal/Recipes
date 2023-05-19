using Recipes.Features.Recipes.Create;
using Recipes.Features.Recipes.GetById;
using Recipes.Features.Recipes.Update;
using Recipes.Shared;
using System.Net.Http.Json;

namespace Recipes.Web.Services;

public class RecipesService : IRecipesService
{
    private readonly HttpClient _httpClient;
    private const string api = "api/Recipes/";
    private IEnumerable<RecipeGetResponse> Recipes;

    public RecipesService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public async Task<(bool Valid, string Message)> Create(RecipeCreateRequest recipe)
    {
        var response = await _httpClient.PostAsJsonAsync(api, recipe);
        if (!response.IsSuccessStatusCode)
            return await response.Content.ErrorResponse();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        var recipeGetResponse = await Get(result.Result);
        Recipes = Recipes.Append(recipeGetResponse);
        return (true, "Recipe created successfully.");
    }
    public async Task<IEnumerable<RecipeGetResponse>> Get()
    {
        Recipes ??= (await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<RecipeGetResponse>>>(api)).Result;
        return Recipes;
    }
    public async Task<RecipeGetResponse> Get(Guid id)
    {
        var recipe = Recipes.FirstOrDefault(x => x.Id == id);
        if (recipe != null)
            return recipe;
        recipe = (await _httpClient.GetFromJsonAsync<ApiResponse<RecipeGetResponse>>(api + id)).Result;
        Recipes.Append(recipe);
        return recipe;
    }
    public async Task<IEnumerable<RecipeGetResponse>> GetByIngredients(IEnumerable<Guid> ingredientIds)
    {
        var recipes = await Get();
        return recipes.Where(x => x.Ingredients.Any(y => ingredientIds.Contains(y.Ingredient.Id)));
    }

    public async Task<(bool Valid, string Message)> Update(RecipeUpdateRequest recipe)
    {
        var response = await _httpClient.PutAsJsonAsync(api, recipe);
        if (!response.IsSuccessStatusCode)
            return await response.Content.ErrorResponse();
        var result = (await response.Content.ReadFromJsonAsync<ApiResponse<RecipeGetResponse>>()).Result;
        Recipes = Recipes.Where(x => x.Id != recipe.Id).Append(result);
        return (true, "Recipe updated successfully.");
    }
}

public interface IRecipesService
{
    Task<IEnumerable<RecipeGetResponse>> Get();
    Task<RecipeGetResponse> Get(Guid Id);
    Task<IEnumerable<RecipeGetResponse>> GetByIngredients(IEnumerable<Guid> ingredientIds);
    Task<(bool Valid, string Message)> Create(RecipeCreateRequest recipe);
    Task<(bool Valid, string Message)> Update(RecipeUpdateRequest recipe);
}