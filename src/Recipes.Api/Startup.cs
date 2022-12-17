using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Shared.Entities;
using Recipes.Api.Services;
using Recipes.Api;
using FluentValidation;
using Recipes.Shared.Models;
using static Recipes.Api.Wrappers.Helpers;

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
        builder.Services.AddScoped<DocsContext>(s => CreateContext(Configuration));

        builder.Services.AddScoped<IValidator<IngredientCreate>, IngredientValidator>();
        builder.Services.AddScoped<IValidator<MaterialCreate>, MaterialValidator>();
        builder.Services.AddScoped<IValidator<RecipeCreate>, RecipeValidator>();

        builder.Services.AddScoped<IIngredientsService, IngredientsService>();
        builder.Services.AddScoped<IMaterialsService, MaterialsService>();
        builder.Services.AddScoped<IRecipesService, RecipesService>();
    }
}