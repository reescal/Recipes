namespace Recipes.Shared.Constants;

public static class Responses
{
    public static string NotFound(string entity) => $"{entity} not found";
    public static string NotFound(string entity, Guid id) => $"{entity} with id {id} not found";
    public static string InvalidLang(string id) => $"{invalidLang} {id}";
    private const string invalidLang = "Invalid language id";
}

public static class Constants
{
    public const string id = "id";
    public const string langId = "langId";
}

public static class ContentTypes
{
    public const string textPlain = "text/plain";
    public const string json = "application/json";
}

public static class HttpMethods
{
    public const string get = nameof(get);
    public const string post = nameof(post);
    public const string put = nameof(put);
}

public static class DBConstants
{
    public const string connString = "CosmosDBConnection";
    public const string dbName = "DatabaseName";
    public const string containerName = "ContainerName";
    public const string ex = "Please specify a valid CosmosDB connection string and database name in the local.settings.json file or your Azure Functions Settings.";
}

public static class ValidationError
{
    public static string TooShort(string s) => $"{s} too short";
    public static string TooLong(string s) => $"{s} too long";
    public static string Required(string s) => $"{s} required";
    public static string Invalid(string s) => $"Invalid {s}";
    public static string InclusiveBetween(string s, int min, int max) => $"{s} must be greater than {min} and less than {max}";
}