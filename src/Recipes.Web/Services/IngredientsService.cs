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

    public async Task<string> Add(IngredientCreateRequest ingredient)
    {
        var response = await _httpClient.PostAsJsonAsync(api, ingredient);
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
            Ingredients = Ingredients.Append(new IngredientGetResponse() { Id = Guid.Parse(result), Image = ingredient.Image, Properties = ingredient.Properties });
        return result;
    }

    public async Task<string> Update(IngredientUpdateRequest ingredient)
    {
        var response = await _httpClient.PutAsJsonAsync(api, ingredient);
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            Ingredients = Ingredients.Where(x => x.Id != ingredient.Id);
            Ingredients = Ingredients.Append(new IngredientGetResponse() { Id = Guid.Parse(result), Image = ingredient.Image, Properties = ingredient.Properties });
        }
        return result;
    }
}

public interface IIngredientsService
{
    Task<IEnumerable<IngredientGetResponse>> Get();
    Task<IngredientGetResponse> Get(Guid id);
    Task<string> Add(IngredientCreateRequest ingredient);
    Task<string> Update(IngredientUpdateRequest ingredient);
}