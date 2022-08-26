using Recipes.Api.Entities;
using Recipes.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Recipes.Api.Wrappers;
using static Recipes.Api.Wrappers.Helpers;
using Recipes.Shared.Enums;

namespace Recipes.Api.Services;

public class MaterialsService : IMaterialsService
{
    private readonly DocsContext context;

    public MaterialsService(IDbContextFactory<DocsContext> factory) => context = factory.CreateDbContext();

    public IEnumerable<Material> Get() => context.Materials.AsNoTracking().AsEnumerable();

    public async Task<Material> GetAsync(Guid id) => await FindById(context.Set<Material>(), id);

    public IEnumerable<ComplexEntity> GetNames(Lang? _lang)
    {
        var result = context.Materials.AsNoTracking().AsEnumerable();
        var response = result.Select(x => (ComplexEntity)x);
        return response.FilterLang(_lang);
    }

    public async Task<string> InsertAsync(MaterialCreate material)
    {
        var i = new Material
        {
            Id = Guid.NewGuid(),
            Image = material.Image,
            Properties = material.Properties
        };
        context.Materials.Add(i);

        await context.SaveChangesAsync();

        return i.Id.ToString();
    }

    public async Task<Material> UpdateAsync(Guid id, MaterialCreate material)
    {
        var i = await FindById(context.Set<Material>(), id);

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
    public IEnumerable<ComplexEntity> GetNames(Lang? _lang);
    public Task<string> InsertAsync(MaterialCreate ingredient);
    public Task<Material> UpdateAsync(Guid id, MaterialCreate ingredient);
}
