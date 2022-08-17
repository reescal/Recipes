using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Recipes.Shared.Interfaces;
using Recipes.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recipes.Shared.Models;
using static Recipes.Api.Constants.Responses;

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

    public static bool LangsExist(IEnumerable<IEntityProperties> properties)
    {
        var langs = properties.Select(x => x.LangId);
        foreach (var lang in langs)
        {
            if (!LangExists(lang))
                return false;
        }
        return true;
    }

    public static async Task<T> FindById<T>(DbSet<T> ctx, Guid id) where T : class
    {
        var result = await ctx.FindAsync(id);

        if (result == null)
            throw new ApiException(NotFound(nameof(T), id), 404);

        return result;
    }

    public static IEnumerable<SimpleEntity> FilterLang(this IEnumerable<SimpleEntity> l, int? _lang)
    {
        if (_lang == null)
            return l;

        if (!LangExists((int)_lang))
            throw new ApiException(InvalidLang((int)_lang), 400);

        l = l.Where(x => x.Properties.Any(y => y.LangId == _lang))
                            .Select(x => new SimpleEntity()
                            {
                                Id = x.Id,
                                Properties = x.Properties
                                                .Where(y => y.LangId == _lang)
                                                .ToHashSet()
                            });

        return l;
    }
}
