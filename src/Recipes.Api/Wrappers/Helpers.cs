using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Recipes.Api.Enums;
using Recipes.Api.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes.Api.Wrappers;

public static class Helpers
{
    public static async Task<T> DeserializeAsync<T>(HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<T>(requestBody);
        return input;
    }

    public static bool LangExists(int langId) => Enum.IsDefined(typeof(Lang), langId);

    public static bool LangsExist(HashSet<IEntityProperties> properties)
    {
        var langs = properties.Select(x => x.LangId);
        foreach (var lang in langs)
        {
            if (!LangExists(lang))
                return false;
        }
        return true;
    }
}
