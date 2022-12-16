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

public class RecipesService : IRecipesService
{
    private readonly DocsContext context;

    public RecipesService(IDbContextFactory<DocsContext> factory) => context = factory.CreateDbContext();

    public async Task<RecipeResponse> GetAsync(Guid id)
    {
        var result = await FindById(context.Set<Recipe>(), id);

        var ingredientIds = result.Ingredients.Select(x => x.IngredientId);
        var ingredients = context.Ingredients.Where(x => ingredientIds.Contains(x.Id)).AsEnumerable();

        var response = new RecipeResponse()
        {
            Id = result.Id,
            Image = result.Image,
            Video = result.Video,
            Yield = result.Yield,
            Time = result.Time,
            Properties = result.Properties,
            Ingredients = result.Ingredients.Select(x => new IngredientRowResponse()
            {
                Ingredient = ingredients.Single(y => y.Id == x.IngredientId),
                Quantity = x.Quantity,
                Preparation = x.Preparation,
                IsOptional = x.IsOptional
            }).ToList()
        };

        return response;
    }

    public IEnumerable<ComplexEntity> GetByIngredients(Guid[] ids)
    {
        var result = context.Recipes.AsNoTracking().AsEnumerable();
        var response = result.Where(x => x.Ingredients.Any(y => ids.Contains(y.IngredientId)))
                            .Select(x => (ComplexEntity)x);
        return response;
    }

    public IEnumerable<ComplexEntity> GetNames(Lang? _lang)
    {
        var result = context.Recipes.AsNoTracking().AsEnumerable();
        var response = result.Select(x => (ComplexEntity)x);
        return response.FilterLang(_lang);
    }

    public async Task<string> InsertAsync(RecipeCreate recipe)
    {
        await CheckIngredients(recipe);

        var i = new Recipe
        {
            Id = Guid.NewGuid(),
            Image = recipe.Image,
            Video = recipe.Video,
            Time = recipe.Time,
            Yield = recipe.Yield,
            Properties = recipe.Properties,
            Ingredients = recipe.Ingredients
        };
        context.Recipes.Add(i);

        await context.SaveChangesAsync();

        return i.Id.ToString();
    }

    public async Task<Recipe> UpdateAsync(Guid id, RecipeCreate recipe)
    {
        await CheckIngredients(recipe);

        var i = await FindById(context.Set<Recipe>(), id);

        foreach (var prop in recipe.Properties)
        {
            var iProp = i.Properties.SingleOrDefault(x => x.LangId == prop.LangId);
            if (iProp == null)
                i.Properties.Add(prop);
            else
            {
                iProp.Name = prop.Name;
                iProp.Description = prop.Description;
                iProp.Tags = prop.Tags;
            }
        }
        i.Image = recipe.Image;
        i.Video = recipe.Video;
        i.Time = recipe.Time;
        i.Yield = recipe.Yield;
        i.Ingredients = recipe.Ingredients;

        await context.SaveChangesAsync();

        return i;
    }

    private async Task CheckIngredients(RecipeCreate r)
    {
        foreach(var ingredient in r.Ingredients)
        {
            await FindById(context.Set<Ingredient>(), ingredient.IngredientId, 400);
        }
    }
}

public interface IRecipesService
{
    public Task<RecipeResponse> GetAsync(Guid id);
    public IEnumerable<ComplexEntity> GetNames(Lang? _lang);
    public IEnumerable<ComplexEntity> GetByIngredients(Guid[] ids);
    public Task<string> InsertAsync(RecipeCreate recipe);
    public Task<Recipe> UpdateAsync(Guid id, RecipeCreate recipe);
}
