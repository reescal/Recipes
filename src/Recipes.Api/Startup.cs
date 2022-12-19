using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Shared.Entities;
using Recipes.Api.Services;
using Recipes.Api;
using FluentValidation;
using Recipes.Shared.Models;
using static Recipes.Shared.Helpers.Helpers.Configuration;
using Microsoft.EntityFrameworkCore;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Recipes.Api;

public class Startup : FunctionsStartup
{
    public IConfigurationRoot Configuration = new ConfigurationBuilder()
                                                    .SetBasePath(Environment.CurrentDirectory)
                                                    .AddJsonFile("local.settings.json", true)
                                                    .AddEnvironmentVariables()
                                                    .Build();

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddTransient<IConfiguration>(o => Configuration);
        builder.Services.AddDbContextFactory<DocsContext>(
           (DbContextOptionsBuilder opts) =>
           {
               var cosmosConfig = Cosmos(Configuration);
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