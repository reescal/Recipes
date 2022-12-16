using Microsoft.Extensions.Configuration;
using Recipes.Shared.Constants;
using static Recipes.Shared.Constants.DBConstants;

namespace Recipes.Shared.Helpers;
public static class Helpers
{
    public static class Configuration
    {
        public static (string ConnectionString, string DatabaseName, string ContainerName) Cosmos(IConfigurationRoot config)
        {
            var connectionString = config[connString] ?? config.GetSection("Values").GetValue<string>(connString);
            var databaseName = config[DBConstants.dbName] ?? config.GetSection("Values").GetValue<string>(dbName);
            var containerName = config[DBConstants.containerName] 
                ?? config.GetSection("Values").GetValue<string>(DBConstants.containerName);
            return string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName)
                ? throw new InvalidOperationException(ex)
                : ((string ConnectionString, string DatabaseName, string ContainerName))(connectionString, databaseName, containerName);
        }
    }
}
