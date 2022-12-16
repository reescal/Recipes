using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Recipes.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Recipes.Shared.Models;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Api.Wrappers;

public static class Helpers
{
    public static async Task<T> DeserializeAsync<T>(HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var input = JsonConvert.DeserializeObject<T>(requestBody);
        return input;
    }

    public static void LangExists(string langId)
    {
        var isNotDefined = int.TryParse(langId, out var asInt) ?
            !Enum.IsDefined(typeof(Lang), asInt) :
            !Enum.IsDefined(typeof(Lang), langId);
        if (isNotDefined)
            throw new ApiException(InvalidLang(langId), 400);
    }

    public static async Task<T> FindById<T>(DbSet<T> ctx, Guid id, int statusCode = 404) where T : class
    {
        var result = await ctx.FindAsync(id);
        return result ?? throw new ApiException(NotFound(typeof(T).Name, id), statusCode);
    }

    public static IEnumerable<T> FilterLang<T>(this IEnumerable<T> l, Lang? _lang) where T : ComplexEntity, new()
    {
        if (_lang == null)
            return l;

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

    public static IEnumerable<T> FilterLang<T>(this IEnumerable<T> l, Lang? _lang, Func<T, bool> filter) where T : class
        => _lang == null ? l : l.Where(filter);
}
