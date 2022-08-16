using Recipes.Api.Entities;
using Recipes.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Recipes.Api.Wrappers;
using static Recipes.Api.Wrappers.Helpers;
using static Recipes.Api.Constants.Responses;
using Recipes.Shared.Interfaces;

namespace Recipes.Api.Services;

public class IngredientsService : IIngredientsService
{
    private readonly DocsContext context;

    public IngredientsService(IDbContextFactory<DocsContext> factory)
    {
        context = factory.CreateDbContext();
    }

    public IEnumerable<Ingredient> Get()
    {
        var result = context.Ingredients.AsNoTracking().AsEnumerable();
        return result;
    }

    public async Task<Ingredient> GetAsync(Guid id)
    {
        var result = await context.Ingredients.FindAsync(id);

        if (result == null)
            throw new ApiException(NotFound(nameof(Ingredient), id), 404);

        return result;
    }

    public IEnumerable<SimpleEntity> GetNames(int _lang)
    {
        if (!LangExists(_lang))
            throw new ApiException(InvalidLang(_lang), 400);

        var result = context.Ingredients.AsNoTracking().AsEnumerable();
        var response = result.Where(x => x.Properties.Any(y => y.LangId == _lang))
                            .Select(x => new SimpleEntity()
                            {
                                Id = x.Id,
                                Properties = x.Properties
                                                .Where(y => y.LangId == _lang)
                                                .Cast<IEntityProperties>()
                                                .ToHashSet()
                            });
        return response;
    }

    public async Task<string> InsertAsync(IngredientCreate ingredient)
    {
        if (!LangsExist(ingredient.Properties.Cast<IEntityProperties>().ToHashSet()))
            throw new ApiException(PropertyInvalidLang(nameof(Ingredient)), 400);

        var i = new Ingredient
        {
            Id = Guid.NewGuid(),
            Image = ingredient.Image,
            Properties = ingredient.Properties
        };
        context.Add(i);

        await context.SaveChangesAsync();

        return i.Id.ToString();
    }

    public async Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient)
    {
        if (!LangsExist(ingredient.Properties.Cast<IEntityProperties>().ToHashSet()))
            throw new ApiException(PropertyInvalidLang(nameof(Ingredient)), 400);

        var i = await context.Ingredients.FindAsync(id);

        if (i == null)
            throw new ApiException(NotFound(nameof(Ingredient), id), 404);

        foreach (var prop in ingredient.Properties)
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
        i.Image = ingredient.Image;

        await context.SaveChangesAsync();

        return i;
    }
}

public interface IIngredientsService
{
    public IEnumerable<Ingredient> Get();
    public Task<Ingredient> GetAsync(Guid id);
    public IEnumerable<SimpleEntity> GetNames(int _lang);
    public Task<string> InsertAsync(IngredientCreate ingredient);
    public Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient);
}
