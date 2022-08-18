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

    public static void LangExists(int langId)
    {
        if (!Enum.IsDefined(typeof(Lang), langId))
            throw new ApiException(InvalidLang((int)langId), 400);
    }

    public static void LangsExist(IEnumerable<IEntityProperties> properties) => properties.Select(x => x.LangId).ToList().ForEach(x => LangExists(x));

    public static async Task<T> FindById<T>(DbSet<T> ctx, Guid id) where T : class
    {
        var result = await ctx.FindAsync(id);

        if (result == null)
            throw new ApiException(NotFound(nameof(T), id), 404);

        return result;
    }

    public static IEnumerable<T> FilterLang<T>(this IEnumerable<T> l, int? _lang) where T : SimpleEntity, new()
    {
        if (_lang == null)
            return l;

        LangExists((int)_lang);

        l = l.Where(x => x.Properties.Any(y => y.LangId == _lang))
                            .Select(x => new T()
                            {
                                Id = x.Id,
                                Properties = x.Properties
                                                .Where(y => y.LangId == _lang)
                                                .ToHashSet()
                            });
        return l;
    }

    public static IEnumerable<T> FilterLang<T>(this IEnumerable<T> l, int? _lang, Func<T, bool> filter) where T : class
    {
        if (_lang == null)
            return l;

        LangExists((int)_lang);

        l = l.Where(filter);
        return l;
    }
}
