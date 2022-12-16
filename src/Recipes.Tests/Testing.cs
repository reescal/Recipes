﻿using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Recipes.Api;
using Recipes.Shared.Constants;
using Recipes.Api.Entities;
using Recipes.Shared.Models;
using Recipes.Shared.Helpers;

namespace Recipes.Tests;

public static class Testing
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
                                                                    .SetBasePath(Environment.CurrentDirectory)
                                                                    .AddJsonFile("local.settings.json", true)
                                                                    .AddEnvironmentVariables()
                                                                    .Build();

    public static IServiceProvider Provider
    {
        get
        {
            var dbName = Configuration[DBConstants.dbName];
            if(string.IsNullOrEmpty(dbName))
                Environment.SetEnvironmentVariable(DBConstants.dbName, "RecipesTest");
            return new HostBuilder().ConfigureWebJobs(new Startup().Configure).Build().Services;
        }
    }

    public static Mock<HttpRequest> CreateMockRequest(object body)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);

        var json = JsonConvert.SerializeObject(body);

        sw.Write(json);
        sw.Flush();

        ms.Position = 0;

        var mockRequest = new Mock<HttpRequest>();
        mockRequest.Setup(x => x.Body).Returns(ms);
        //mockRequest.Setup(x => x.QueryString).Returns(new QueryString(""));

        return mockRequest;
    }

    public static async Task<T> CreateEntity<T>(T t) where T : class, new()
    {
        var cosmosConfig = Helpers.Configuration.Cosmos(Configuration);
        var options = new DbContextOptionsBuilder<DocsContext>();
        options.UseCosmos(cosmosConfig.ConnectionString, cosmosConfig.DatabaseName);
        var _sut = new DocsContext(options.Options);
        _sut.Set<T>().Add(t);
        await _sut.SaveChangesAsync();
        return t;
    }

    public static async Task<Ingredient> CreateIngredient()
    {
        var ingredient = new Ingredient()
        {
            Image = "https://image/link.png",
            Properties = new()
            {
                new()
                {
                    LangId = Shared.Enums.Lang.English,
                    Description = "Description",
                    Name = "Name",
                    Type = "Type"
                },
                new()
                {
                    LangId = Shared.Enums.Lang.Spanish,
                    Description = "Descripcion",
                    Name = "Nombre",
                    Type = "Tipo"
                }
            }
        };
        return await CreateEntity(ingredient);
    }

    public static async Task<Material> CreateMaterial()
    {
        var material = new Material()
        {
            Image = "https://image/link.png",
            Properties = new()
            {
                new()
                {
                    LangId = Shared.Enums.Lang.English,
                    Description = "Description",
                    Name = "Name",
                    Type = "Type"
                },
                new()
                {
                    LangId = Shared.Enums.Lang.Spanish,
                    Description = "Descripcion",
                    Name = "Nombre",
                    Type = "Tipo"
                }
            }
        };
        return await CreateEntity(material);
    }

    public static async Task<Recipe> CreateRecipe()
    {
        var ingredient = await CreateIngredient();
        var recipe = new Recipe()
        {
            Image = "https://valid/link.png",
            Time = 30,
            Yield = "8 servings",
            Properties = new IndexHashSet<RecipeProperties>
            {
                new()
                {
                    LangId = Shared.Enums.Lang.English,
                    Description = "Description",
                    Name = "Name",
                    Type= "Type"
                },
                new()
                {
                    LangId = Shared.Enums.Lang.Spanish,
                    Description = "Descripcion",
                    Name = "Nombre",
                    Type= "Tipo"
                }
            },
            Ingredients = new List<IngredientRow>
            {
                new()
                {
                    IngredientId = ingredient.Id,
                    Preparation = "Preparation",
                    Quantity = new()
                    {
                        Value = 10,
                        Unit = "units"
                    }
                }
            }
        };
        return await CreateEntity(recipe);
    }
}
