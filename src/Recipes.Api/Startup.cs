using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Recipes.Api.Entities;
using Recipes.Api.Services;
using Recipes.Api;
using static Recipes.Api.Constants.DBConstants;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Recipes.Api;

public class Startup : FunctionsStartup
{
    private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
                                                                    .AddEnvironmentVariables()
                                                                    .Build();

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddDbContextFactory<DocsContext>(
           (DbContextOptionsBuilder opts) =>
           {
               var connectionString = Configuration[connString];
               var databaseName = Configuration[dbName];
               if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
                   throw new InvalidOperationException(ex);

               opts.UseCosmos(connectionString, databaseName);
           });

        builder.Services.AddScoped<IIngredientsService, IngredientsService>();
        builder.Services.AddScoped<IMaterialsService, MaterialsService>();
    }
}