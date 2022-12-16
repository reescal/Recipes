using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Entities;
using Recipes.Api.Services;
using Recipes.Api;
using FluentValidation;
using Recipes.Shared.Models;
using Recipes.Shared.Helpers;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Recipes.Api;

public class Startup : FunctionsStartup
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
                                                                    .SetBasePath(Environment.CurrentDirectory)
                                                                    .AddJsonFile("local.settings.json", true)
                                                                    .AddEnvironmentVariables()
                                                                    .Build();

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContextFactory<DocsContext>(
           (DbContextOptionsBuilder opts) =>
           {
               var cosmosConfig = Helpers.Configuration.Cosmos(Configuration);
               opts.UseCosmos(cosmosConfig.ConnectionString, cosmosConfig.DatabaseName);
           });

        builder.Services.AddScoped<IValidator<IngredientCreate>, IngredientValidator>();
        builder.Services.AddScoped<IValidator<MaterialCreate>, MaterialValidator>();
        builder.Services.AddScoped<IValidator<RecipeCreate>, RecipeValidator>();

        builder.Services.AddScoped<IIngredientsService, IngredientsService>();
        builder.Services.AddScoped<IMaterialsService, MaterialsService>();
        builder.Services.AddScoped<IRecipesService, RecipesService>();
    }
}