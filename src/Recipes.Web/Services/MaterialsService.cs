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

    public async Task<(bool Valid, string Message)> Add(MaterialCreateRequest material)
    {
        var response = await _httpClient.PostAsJsonAsync(api, material);
        if (!response.IsSuccessStatusCode)
            return (false, await response.Content.ReadAsStringAsync());
        var result = await response.Content.ReadFromJsonAsync<Guid>();
        Materials = Materials.Append(new MaterialGetResponse() { Id = result, Image = material.Image, Name = material.Name, Description = material.Description, Type = material.Type });
        return (true, "Material created successfully.");
    }

    public async Task<(bool Valid, string Message)> Update(MaterialUpdateRequest material)
    {
        var response = await _httpClient.PutAsJsonAsync(api, material);
        if (!response.IsSuccessStatusCode)
            return (false, await response.Content.ReadAsStringAsync());
        var result = await response.Content.ReadFromJsonAsync<MaterialGetResponse>();
        Materials = Materials.Where(x => x.Id != material.Id).Append(result);
        return (true, "Material updated successfully.");
    }
}

public interface IMaterialsService
{
    Task<IEnumerable<MaterialGetResponse>> Get();
    Task<MaterialGetResponse> Get(Guid id);
    Task<(bool Valid, string Message)> Add(MaterialCreateRequest ingredient);
    Task<(bool Valid, string Message)> Update(MaterialUpdateRequest ingredient);
}