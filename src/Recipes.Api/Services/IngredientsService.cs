using Recipes.Api.Entities;
using Recipes.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Recipes.Api.Wrappers;
using static Recipes.Api.Wrappers.Helpers;
using Recipes.Shared.Interfaces;

namespace Recipes.Api.Services;

public class IngredientsService : IIngredientsService
{
    private readonly DocsContext context;

    public IngredientsService(IDbContextFactory<DocsContext> factory) => context = factory.CreateDbContext();

    public IEnumerable<Ingredient> Get() => context.Ingredients.AsNoTracking().AsEnumerable();

    public async Task<Ingredient> GetAsync(Guid id) => await FindById(context.Set<Ingredient>(), id);

    public IEnumerable<SimpleEntity> GetNames(int? _lang)
    {
        var result = context.Ingredients.AsNoTracking().AsEnumerable();
        var response = result.Select(x => new SimpleEntity()
                                                    {
                                                        Id = x.Id,
                                                        Properties = x.Properties
                                                                            .Cast<IEntityProperties>()
                                                                            .ToHashSet()
                                                    });
        return response.FilterLang(_lang);
    }

    public HashSet<IngredientTypes> GetTypes(int? _lang)
    {
        var result = context.Ingredients.AsNoTracking().AsEnumerable().Select(x => x.Properties);
        var response = new HashSet<IngredientTypes>()
            {
                new IngredientTypes()
                {
                    LangId = 1,
                    Types = result.Select(x => x.First(y => y.LangId == 1).Type).ToHashSet()
                },
                new IngredientTypes()
                {
                    LangId = 2,
                    Types = result.Select(x => x.First(y => y.LangId == 2).Type).ToHashSet()
                }
        };

        return response.FilterLang(_lang, x => x.LangId == _lang).ToHashSet();
    }

    public async Task<string> InsertAsync(IngredientCreate ingredient)
    {
        LangsExist(ingredient.Properties.Cast<IEntityProperties>());

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
        LangsExist(ingredient.Properties.Cast<IEntityProperties>());

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
    public IEnumerable<SimpleEntity> GetNames(int? _lang);
    public HashSet<IngredientTypes> GetTypes(int? _lang);
    public Task<string> InsertAsync(IngredientCreate ingredient);
    public Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient);
}
