using Recipes.Shared.Entities;
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

public class IngredientsService : IIngredientsService
{
    private readonly DocsContext context;

    public IngredientsService(IDbContextFactory<DocsContext> factory) => context = factory.CreateDbContext();

    public IEnumerable<Ingredient> Get() => context.Ingredients.AsNoTracking().AsEnumerable();

    public async Task<Ingredient> GetAsync(Guid id) => await FindById(context.Set<Ingredient>(), id);

    public IEnumerable<ComplexEntity> GetNames(Lang? _lang)
    {
        var result = context.Ingredients.AsNoTracking().AsEnumerable();
        var response = result.Select(x => (ComplexEntity)x);
        return response.FilterLang(_lang);
    }

    public HashSet<EntityTypes> GetTypes(Lang? _lang)
    {
        var result = context.Ingredients.AsNoTracking().AsEnumerable().Select(x => x.Properties);
        var response = new HashSet<EntityTypes>()
            {
                new EntityTypes()
                {
                    LangId = Lang.English,
                    Types = result.Select(x => x.First(y => y.LangId == Lang.English).Type).ToHashSet()
                },
                new EntityTypes()
                {
                    LangId = Lang.Spanish,
                    Types = result.Select(x => x.First(y => y.LangId == Lang.Spanish).Type).ToHashSet()
                }
        };

        return response.FilterLang(_lang, x => x.LangId == _lang).ToHashSet();
    }

    public async Task<string> InsertAsync(IngredientCreate ingredient)
    {
        var i = new Ingredient
        {
            Id = Guid.NewGuid(),
            Image = ingredient.Image,
            Properties = ingredient.Properties
        };
        context.Ingredients.Add(i);

        await context.SaveChangesAsync();

        return i.Id.ToString();
    }

    public async Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient)
    {
        var i = await FindById(context.Set<Ingredient>(), id);

        foreach (var prop in ingredient.Properties)
        {
            var iProp = i.Properties.SingleOrDefault(x => x.LangId == prop.LangId);
            if (iProp == null)
                i.Properties.Add(prop);
            else
            {
                iProp.Name = prop.Name;
                iProp.Description = prop.Description;
                iProp.Type = prop.Type;
            }
        }
        i.Image = ingredient.Image;

        await context.SaveChangesAsync();

        return i;
    }
}

public interface IIngredientsService
{
    public IEnumerable<Ingredient> Get();
    public Task<Ingredient> GetAsync(Guid id);
    public IEnumerable<ComplexEntity> GetNames(Lang? _lang);
    public HashSet<EntityTypes> GetTypes(Lang? _lang);
    public Task<string> InsertAsync(IngredientCreate ingredient);
    public Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient);
}
