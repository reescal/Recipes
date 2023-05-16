﻿using Recipes.Features.GroceryList.Get;
using System.Net.Http.Json;

namespace Recipes.Web.Services;

public class GroceryListService : IGroceryListService
{
    private readonly HttpClient _httpClient;
    private const string api = "api/GroceryLists/";
    private GetGroceryListResponse GroceryList;

    public GroceryListService(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient("API");
    }

    public async Task<GetGroceryListResponse> Get()
    {
        GroceryList ??= await _httpClient.GetFromJsonAsync<GetGroceryListResponse>(api);
        return GroceryList;
    }

    public async Task<(bool Valid, string Message)> AddGrocery(HashSet<GroceryResponse> request)
    {
        var response = await _httpClient.PostAsJsonAsync(api + nameof(AddGrocery), request);
        if (response.IsSuccessStatusCode)
        {
            foreach (var grocery in request)
            {
                var existingGrocery = GroceryList.Grocery.First(x => x.Ingredient.Id == grocery.Ingredient.Id);
                if (existingGrocery != null)
                    existingGrocery.Quantity.Value += grocery.Quantity.Value;
                else
                    GroceryList.Grocery.Add(grocery);
            }
        }
        return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Valid, string Message)> Update(HashSet<GroceryResponse> request)
    {
        var response = await _httpClient.PutAsJsonAsync(api, request);
        if (response.IsSuccessStatusCode)
            GroceryList.Grocery = request;
        return (response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }
}

public interface IGroceryListService
{
    Task<GetGroceryListResponse> Get();
    Task<(bool Valid, string Message)> AddGrocery(HashSet<GroceryResponse> request);
    Task<(bool Valid, string Message)> Update(HashSet<GroceryResponse> request);
}