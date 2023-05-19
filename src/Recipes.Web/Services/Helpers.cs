using Recipes.Shared;
using System.Net.Http.Json;

namespace Recipes.Web.Services;

public static class Helpers
{
    public static async Task<(bool Valid, string Message)> ErrorResponse(this HttpContent content)
    {
        var apiResponse = await content.ReadFromJsonAsync<ApiResponse>();
        return (apiResponse.Success, string.Join('\n', apiResponse.Errors));
    }
}
