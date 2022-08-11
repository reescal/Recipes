using Recipes.Api.Entities;
using Recipes.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Recipes.Api.Wrappers;
using static Recipes.Api.Wrappers.Helpers;
using static Recipes.Api.Constants.Responses;
using Recipes.Api.Interfaces;

namespace Recipes.Api.Services;

public class MaterialsService : IMaterialsService
{
    private readonly DocsContext context;

    public MaterialsService(IDbContextFactory<DocsContext> factory)
    {
        context = factory.CreateDbContext();
    }

    public IEnumerable<Material> Get()
    {
        var result = context.Materials.AsNoTracking().AsEnumerable();
        return result;
    }

    public async Task<Material> GetAsync(Guid id)
    {
        var result = await context.Materials.FindAsync(id);

        if (result == null)
            throw new ApiException(NotFound(nameof(Material), id), 404);

        return result;
    }

    public IEnumerable<string> GetNames(int _lang)
    {
        if (!LangExists(_lang))
            throw new ApiException(InvalidLang(_lang), 400);

        var result = context.Materials
                                    .AsNoTracking()
                                    .Select(x => x.Properties)
                                    .AsEnumerable();
        result = result.Where(x => x.Any(y => y.LangId == _lang));
        var response = result.Select(x => x.Where(y => y.LangId == _lang).SingleOrDefault().Name);
        return response;
    }

    public async Task<string> InsertAsync(MaterialCreate material)
    {
        if (!LangsExist(material.Properties.Cast<IEntityProperties>().ToHashSet()))
            throw new ApiException(PropertyInvalidLang(nameof(Material)), 400);

        var i = new Material
        {
            Id = Guid.NewGuid(),
            Image = material.Image,
            Properties = material.Properties
        };
        context.Add(i);

        await context.SaveChangesAsync();

        return i.Id.ToString();
    }

    public async Task<Material> UpdateAsync(Guid id, MaterialCreate material)
    {
        if (!LangsExist(material.Properties.Cast<IEntityProperties>().ToHashSet()))
            throw new ApiException(PropertyInvalidLang(nameof(Material)), 400);

        var i = await context.Materials.FindAsync(id);

        if (i == null)
            throw new ApiException(NotFound(nameof(Material), id), 404);

        foreach (var prop in material.Properties)
        {
            var iProp = i.Properties.SingleOrDefault(x => x.LangId == prop.LangId);
            if (iProp == null)
                i.Properties.Add(prop);
            else
            {
                iProp.Name = prop.Name;
                iProp.Description = prop.Description;
            }
        }
        i.Image = material.Image;

        await context.SaveChangesAsync();

        return i;
    }
}

public interface IMaterialsService
{
    public IEnumerable<Material> Get();
    public Task<Material> GetAsync(Guid id);
    public IEnumerable<string> GetNames(int _lang);
    public Task<string> InsertAsync(MaterialCreate ingredient);
    public Task<Material> UpdateAsync(Guid id, MaterialCreate ingredient);
}
