using Recipes.Features.Materials.Create;
using Recipes.Features.Materials.GetById;
using Recipes.Features.Materials.Update;
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

    private IEnumerable<MaterialGetResponse> Materials;

    public async Task<IEnumerable<MaterialGetResponse>> Get()
    {
        Materials ??= await _httpClient.GetFromJsonAsync<IEnumerable<MaterialGetResponse>>(api);
        return Materials;
    }

    public async Task<MaterialGetResponse> Get(Guid id) => await _httpClient.GetFromJsonAsync<MaterialGetResponse>(api + id);

    public async Task<string> Add(MaterialCreateRequest material)
    {
        var response = await _httpClient.PostAsJsonAsync(api, material);
        var result = await response.Content.ReadAsStringAsync();
        if(response.IsSuccessStatusCode)
            Materials = Materials.Append(new MaterialGetResponse() { Id = Guid.Parse(result), Image = material.Image, Properties = material.Properties });
        return result;
    }

    public async Task<string> Update(MaterialUpdateRequest material)
    {
        var response = await _httpClient.PutAsJsonAsync(api, material);
        var result = await response.Content.ReadAsStringAsync();
        if(response.IsSuccessStatusCode)
        {
            Materials = Materials.Where(x => x.Id != material.Id);
            Materials = Materials.Append(new MaterialGetResponse() { Id = Guid.Parse(result), Image = material.Image, Properties = material.Properties });
        }
        return result;
    }
}

public interface IMaterialsService
{
    Task<IEnumerable<MaterialGetResponse>> Get();
    Task<MaterialGetResponse> Get(Guid id);
    Task<string> Add(MaterialCreateRequest ingredient);
    Task<string> Update(MaterialUpdateRequest ingredient);
}