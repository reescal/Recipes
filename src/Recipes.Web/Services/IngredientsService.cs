using System.Net.Http.Json;
using Recipes.Features.Ingredients.Create;
using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Ingredients.Update;
using Recipes.Features.Recipes.GetById;
using Recipes.Shared;

namespace Recipes.Web.Services;

public class IngredientsService : IIngredientsService
{
    private readonly HttpClient _httpClient;
    private const string api = "api/Ingredients/";
    private IEnumerable<IngredientGetResponse> Ingredients;

    public IngredientsService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public async Task<IEnumerable<IngredientGetResponse>> Get()
    {
        Ingredients ??= (await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<IngredientGetResponse>>>(api)).Result;
        return Ingredients;
    }

    public async Task<IngredientGetResponse> Get(Guid id)
    {
        var ingredient = Ingredients.FirstOrDefault(x => x.Id == id);
        if (ingredient != null)
            return ingredient;
        ingredient = (await _httpClient.GetFromJsonAsync<ApiResponse<IngredientGetResponse>>(api + id)).Result;
        Ingredients.Append(ingredient);
        return ingredient;
    }

    public async Task<(bool Valid, string Message)> Add(IngredientCreateRequest ingredient)
    {
        var response = await _httpClient.PostAsJsonAsync(api, ingredient);
        if (!response.IsSuccessStatusCode)
            return await response.Content.ErrorResponse();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        Ingredients = Ingredients.Append(new IngredientGetResponse() { Id = result.Result, Image = ingredient.Image, Name = ingredient.Name, Description = ingredient.Description, Type = ingredient.Type });
        return (true, "Ingredient created successfully.");
    }

    public async Task<(bool Valid, string Message)> Update(IngredientUpdateRequest ingredient)
    {
        var response = await _httpClient.PutAsJsonAsync(api, ingredient);
        if (!response.IsSuccessStatusCode)
            return await response.Content.ErrorResponse();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IngredientGetResponse>>();
        Ingredients = Ingredients.Where(x => x.Id != ingredient.Id).Append(result.Result);
        return (true, "Ingredient updated successfully.");
    }
}

public interface IIngredientsService
{
    Task<IEnumerable<IngredientGetResponse>> Get();
    Task<IngredientGetResponse> Get(Guid id);
    Task<(bool Valid, string Message)> Add(IngredientCreateRequest ingredient);
    Task<(bool Valid, string Message)> Update(IngredientUpdateRequest ingredient);
}