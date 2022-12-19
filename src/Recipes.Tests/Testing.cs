using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Recipes.Api;
using static Recipes.Shared.Helpers.Helpers.Configuration;
using Recipes.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Recipes.Shared.Entities;

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
            var startup = new Startup() 
            { 
                Configuration = Configuration 
            };
            return new HostBuilder().ConfigureWebJobs(startup.Configure).Build().Services;
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
        var cosmosConfig = Cosmos(Configuration);
        var options = new DbContextOptionsBuilder<DocsContext>();
        options.UseCosmos(cosmosConfig.ConnectionString, cosmosConfig.DatabaseName);
        var context = new DocsContext(options.Options, Configuration);
        context.Set<T>().Add(t);
        await context.SaveChangesAsync();
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

