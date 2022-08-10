using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Entities;
using Recipes.Api.Services;
using Recipes.Api;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Recipes.Api;

public class Startup : FunctionsStartup
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
        .SetBasePath(Environment.CurrentDirectory)
        .AddEnvironmentVariables()
        .Build();

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddApplicationInsightsTelemetry();
        builder.Services.AddDbContextFactory<DocsContext>(
           (DbContextOptionsBuilder opts) =>
           {
               var connectionString = Configuration["CosmosDBConnection"];
               var databaseName = Configuration["DatabaseName"];
               if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
                   throw new InvalidOperationException(
                       "Please specify a valid CosmosDB connection string and database name in the local.settings.json file or your Azure Functions Settings.");

               opts.UseCosmos(
                   connectionString,
                   databaseName);
           });

        builder.Services.AddScoped<IIngredientsService, IngredientsService>();
    }
}