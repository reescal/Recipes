﻿using Recipes.Api.Entities;
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

public class RecipesService : IRecipesService
{
    private readonly DocsContext context;

    public RecipesService(IDbContextFactory<DocsContext> factory)
    {
        context = factory.CreateDbContext();
    }

    public async Task<RecipeResponse> GetAsync(Guid id)
    {
        var result = await context.Recipes.FindAsync(id);

        if (result == null)
            throw new ApiException(NotFound(nameof(Recipe), id), 404);

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

    public IEnumerable<SimpleEntity> GetByIngredients(Guid[] ids)
    {
        var result = context.Recipes.AsNoTracking().AsEnumerable();
        var response = result.Where(x => x.Ingredients.Any(y => ids.Contains(y.IngredientId)))
                    .Select(x => new SimpleEntity() 
                    {
                        Id = x.Id,
                        Properties = x.Properties
                                        .Cast<IEntityProperties>()
                                        .ToHashSet() 
                    });

        return response;
    }

    public IEnumerable<SimpleEntity> GetNames(int _lang)
    {
        if (!LangExists(_lang))
            throw new ApiException(InvalidLang(_lang), 400);

        var result = context.Recipes.AsNoTracking().AsEnumerable();
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

    public async Task<string> InsertAsync(RecipeCreate recipe)
    {
        if (!LangsExist(recipe.Properties.Cast<IEntityProperties>().ToHashSet()))
            throw new ApiException(PropertyInvalidLang(nameof(Recipe)), 400);

        CheckIngredients(recipe);

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
        if (!LangsExist(recipe.Properties.Cast<IEntityProperties>().ToHashSet()))
            throw new ApiException(PropertyInvalidLang(nameof(Recipe)), 400);

        CheckIngredients(recipe);

        var i = await context.Recipes.FindAsync(id);

        if (i == null)
            throw new ApiException(NotFound(nameof(Recipe), id), 404);

        foreach (var prop in recipe.Properties)
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
        i.Image = recipe.Image;
        i.Video = recipe.Video;
        i.Time = recipe.Time;
        i.Yield = recipe.Yield;
        i.Ingredients = recipe.Ingredients;

        await context.SaveChangesAsync();

        return i;
    }

    private void CheckIngredients(RecipeCreate r)
    {
        foreach (var ingredient in r.Ingredients)
        {
            _ = context.Ingredients.Find(ingredient.IngredientId) ?? throw new ApiException(NotFound(nameof(Ingredient), ingredient.IngredientId), 404);
        }
    }
}

public interface IRecipesService
{
    public Task<RecipeResponse> GetAsync(Guid id);
    public IEnumerable<SimpleEntity> GetNames(int _lang);
    public IEnumerable<SimpleEntity> GetByIngredients(Guid[] ids);
    public Task<string> InsertAsync(RecipeCreate recipe);
    public Task<Recipe> UpdateAsync(Guid id, RecipeCreate recipe);
}
