using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Data;
using Recipes.Api;
using static Recipes.Shared.Helpers.Helpers.Configuration;
using Microsoft.EntityFrameworkCore;
using Recipes.Shared.Constants;
using Recipes.Features.Ingredients.GetById;
using MediatR;
using FluentValidation;

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
        builder.Services.AddSingleton(c => 
            new CosmosConfig
            { 
                ContainerName = Configuration[DBConstants.containerName]
                    ?? Configuration.GetSection("Values").GetValue<string>(DBConstants.containerName)
            });
        builder.Services.AddDbContextFactory<DocsContext>(
           (DbContextOptionsBuilder opts) =>
           {
               var cosmosConfig = Cosmos(Configuration);
               opts.UseCosmos(cosmosConfig.ConnectionString, cosmosConfig.DatabaseName);
           });

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IngredientGetResponse).Assembly));
        builder.Services.AddAutoMapper(typeof(IngredientGetResponse).Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        builder.Services.AddSingleton<IHttpFunctionExecutor, HttpFunctionExecutor>();
        builder.Services.AddValidatorsFromAssembly(typeof(IngredientGetResponse).Assembly);
    }
}