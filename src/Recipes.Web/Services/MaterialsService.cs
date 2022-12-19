using Recipes.Shared.Models;
using System.Net.Http.Json;

namespace Recipes.Web.Services;

public class MaterialsService : IMaterialsService
{
    private readonly HttpClient _httpClient;
    private const string api = "api/Materials/";

    public MaterialsService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    private IEnumerable<Material> Materials;
    private HashSet<EntityTypes> MaterialTypes;

    public async Task<IEnumerable<Material>> Get()
    {
        Materials ??= await _httpClient.GetFromJsonAsync<IEnumerable<Material>>(api);
        return Materials;
    }

    public async Task<Material> Get(Guid id) => await _httpClient.GetFromJsonAsync<Material>(api + id);

    public async Task<HashSet<EntityTypes>> GetTypes()
    {
        MaterialTypes ??= await _httpClient.GetFromJsonAsync<HashSet<EntityTypes>>(api + "Types");
        return MaterialTypes;
    }

    public async Task<string> Add(MaterialCreate material)
    {
        var response = await _httpClient.PostAsJsonAsync(api, material);
        var result = await response.Content.ReadAsStringAsync();
        if(response.IsSuccessStatusCode)
        {
            Materials = Materials.Append(new Material() { Id = Guid.Parse(result), Image = material.Image, Properties = material.Properties });
            foreach(var i in material.Properties)
            {
                MaterialTypes.First(x => x.LangId == i.LangId).Types.Add(i.Type);
            }
        }
        return result;
    }

    public async Task<string> Update(Guid id, MaterialCreate material)
    {
        var response = await _httpClient.PutAsJsonAsync(api + id, material);
        var result = await response.Content.ReadAsStringAsync();
        if(response.IsSuccessStatusCode)
        {
            Materials = Materials.Where(x => x.Id != id);
            Materials = Materials.Append(new Material() { Id = Guid.Parse(result), Image = material.Image, Properties = material.Properties });
            foreach(var i in material.Properties)
            {
                MaterialTypes.First(x => x.LangId == i.LangId).Types.Add(i.Type);
            }
        }
        return result;
    }
}

public interface IMaterialsService
{
    Task<IEnumerable<Material>> Get();
    Task<Material> Get(Guid id);
    Task<HashSet<EntityTypes>> GetTypes();
    Task<string> Add(MaterialCreate ingredient);
    Task<string> Update(Guid id, MaterialCreate ingredient);
}