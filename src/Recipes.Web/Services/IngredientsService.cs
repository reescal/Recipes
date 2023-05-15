using System.Net.Http.Json;
using Recipes.Features.Ingredients.Create;
using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Ingredients.Update;

namespace Recipes.Web.Services;

public class IngredientsService : IIngredientsService
{
    private readonly HttpClient _httpClient;
    private const string api = "api/Ingredients/";

    public IngredientsService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    private IEnumerable<IngredientGetResponse> Ingredients;

    public async Task<IEnumerable<IngredientGetResponse>> Get()
    {
        Ingredients ??= await _httpClient.GetFromJsonAsync<IEnumerable<IngredientGetResponse>>(api);
        return Ingredients;
    }

    public async Task<IngredientGetResponse> Get(Guid id) => await _httpClient.GetFromJsonAsync<IngredientGetResponse>(api + id);

    public async Task<(bool Valid, string Message)> Add(IngredientCreateRequest ingredient)
    {
        var response = await _httpClient.PostAsJsonAsync(api, ingredient);
        if(!response.IsSuccessStatusCode)
            return (false, await response.Content.ReadAsStringAsync());
        var result = await response.Content.ReadFromJsonAsync<Guid>();
        Ingredients = Ingredients.Append(new IngredientGetResponse() { Id = result, Image = ingredient.Image, Name = ingredient.Name, Description = ingredient.Description, Type = ingredient.Type });
        return (true, "Ingredient created successfully.");
    }

    public async Task<(bool Valid, string Message)> Update(IngredientUpdateRequest ingredient)
    {
        var response = await _httpClient.PutAsJsonAsync(api, ingredient);
        if (!response.IsSuccessStatusCode)
            return (false, await response.Content.ReadAsStringAsync());
        var result = await response.Content.ReadFromJsonAsync<IngredientGetResponse>();
        Ingredients = Ingredients.Where(x => x.Id != ingredient.Id).Append(result);
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