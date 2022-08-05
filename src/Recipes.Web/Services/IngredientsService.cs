namespace Recipes.Web.Services;

public class IngredientsService : IIngredientsService
{
    private readonly HttpClient _httpClient;

    public IngredientsService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public async Task<string> Get() => await _httpClient.GetStringAsync("api/Ingredients");
}

public interface IIngredientsService
{
    Task<string> Get();
}