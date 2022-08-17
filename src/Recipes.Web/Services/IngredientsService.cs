using System.Net.Http.Json;
using Recipes.Shared.Models;

namespace Recipes.Web.Services;

public class IngredientsService : IIngredientsService
{
    private readonly HttpClient _httpClient;

    public IngredientsService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public async Task<IEnumerable<SimpleEntity>> Get() => await _httpClient.GetFromJsonAsync<IEnumerable<SimpleEntity>>("api/Ingredients/Names");
    public async Task<Ingredient> Get(Guid id) => await _httpClient.GetFromJsonAsync<Ingredient>($"api/Ingredients/{id}");
}

public interface IIngredientsService
{
    Task<IEnumerable<SimpleEntity>> Get();
    Task<Ingredient> Get(Guid id);
}