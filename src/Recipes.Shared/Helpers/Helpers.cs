using Microsoft.Extensions.Configuration;
using static Recipes.Shared.Constants.DBConstants;

namespace Recipes.Shared.Helpers;
public static class Helpers
{
    public static class Configuration
    {
        public static (string ConnectionString, string DatabaseName) Cosmos(IConfigurationRoot config)
        {
            var connectionString = config[connString] ?? config.GetSection("Values").GetValue<string>(connString);
            var databaseName = config[dbName] ?? config.GetSection("Values").GetValue<string>(dbName);
            return string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName)
                ? throw new InvalidOperationException(ex)
                : ((string ConnectionString, string DatabaseName))(connectionString, databaseName);
        }
    }
}
