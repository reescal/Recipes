using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Recipes.Api.Wrappers;

public static class Helpers
{
    public static async Task<T> DeserializeAsync<T>(HttpRequest req)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<T>(requestBody);
        return input;
    }
}
