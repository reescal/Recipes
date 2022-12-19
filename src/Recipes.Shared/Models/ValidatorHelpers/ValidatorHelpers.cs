namespace Recipes.Shared.Models.ValidatorHelpers;

public static class ValidatorHelpers
{
    public static bool BeUri(string uri) => uri.StartsWith("https://") && ImageFormats.Contains(uri[uri.LastIndexOf('.')..].ToLowerInvariant());

    private static List<string> ImageFormats => new() { ".png", ".jpg", ".jpeg", ".gif" };
}
