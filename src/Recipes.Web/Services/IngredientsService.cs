using System.Net.Http.Json;
using Recipes.Shared.Models;

namespace Recipes.Web.Services;

public class IngredientsService : IIngredientsService
{
    private readonly HttpClient _httpClient;
    private const string api = "api/Ingredients/";

    public IngredientsService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    private IEnumerable<Ingredient> Ingredients;
    private HashSet<IngredientTypes> IngredientTypes;

    public async Task<IEnumerable<Ingredient>> Get()
    {
        Ingredients ??= await _httpClient.GetFromJsonAsync<IEnumerable<Ingredient>>(api);
        return Ingredients;
    }

    public async Task<Ingredient> Get(Guid id) => await _httpClient.GetFromJsonAsync<Ingredient>(api + id);
    public async Task<HashSet<IngredientTypes>> GetTypes()
    {
        IngredientTypes ??= await _httpClient.GetFromJsonAsync<HashSet<IngredientTypes>>(api + "Types");
        return IngredientTypes;
    }

    public async Task<string> Add(IngredientCreate ingredient)
    {
        var response = await _httpClient.PostAsJsonAsync(api, ingredient);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            Ingredients = Ingredients.Append(new Ingredient() { Id = Guid.Parse(result), Image = ingredient.Image, Properties = ingredient.Properties });
            foreach (var i in ingredient.Properties)
            {
                IngredientTypes.First(x => x.LangId == i.LangId).Types.Add(i.Type);
            }
        }
        return result;
    }

    public async Task<string> Update(Guid id, IngredientCreate ingredient)
    {
        var response = await _httpClient.PutAsJsonAsync(api + id, ingredient);
        string result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            Ingredients = Ingredients.Where(x => x.Id != id);
            Ingredients = Ingredients.Append(new Ingredient() { Id = Guid.Parse(result), Image = ingredient.Image, Properties = ingredient.Properties });
            foreach(var i in ingredient.Properties)
            {
                IngredientTypes.First(x => x.LangId == i.LangId).Types.Add(i.Type);
            }
        }
        return result;
    }
}

public interface IIngredientsService
{
    Task<IEnumerable<Ingredient>> Get();
    Task<Ingredient> Get(Guid id);
    Task<HashSet<IngredientTypes>> GetTypes();
    Task<string> Add(IngredientCreate ingredient);
    Task<string> Update(Guid id, IngredientCreate ingredient);
}