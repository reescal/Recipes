using Recipes.Features.Materials.Create;
using Recipes.Features.Materials.GetById;
using Recipes.Features.Materials.Update;
using Recipes.Shared;
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
        Materials ??= (await _httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<MaterialGetResponse>>>(api)).Result;
        return Materials;
    }

    public async Task<MaterialGetResponse> Get(Guid id)
    {
        var material = Materials.FirstOrDefault(x => x.Id == id);
        if (material != null)
            return material;
        material = (await _httpClient.GetFromJsonAsync<ApiResponse<MaterialGetResponse>>(api + id)).Result;
        Materials.Append(material);
        return material;
    }

    public async Task<(bool Valid, string Message)> Add(MaterialCreateRequest material)
    {
        var response = await _httpClient.PostAsJsonAsync(api, material);
        if (!response.IsSuccessStatusCode)
            return await response.Content.ErrorResponse();
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        Materials = Materials.Append(new MaterialGetResponse() { Id = result.Result, Image = material.Image, Name = material.Name, Description = material.Description });
        return (true, "Material created successfully.");
    }

    public async Task<(bool Valid, string Message)> Update(MaterialUpdateRequest material)
    {
        var response = await _httpClient.PutAsJsonAsync(api, material);
        if (!response.IsSuccessStatusCode)
            return await response.Content.ErrorResponse();
        var result = await response.Content.ReadFromJsonAsync< ApiResponse<MaterialGetResponse>>();
        Materials = Materials.Where(x => x.Id != material.Id).Append(result.Result);
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