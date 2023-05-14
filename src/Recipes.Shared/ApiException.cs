using System.Net;

namespace Recipes.Shared;

public class ApiException : Exception
{
    public ApiException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
        ReasonPhrase = message;
    }

    public HttpStatusCode StatusCode { get; }
    public string ReasonPhrase { get; }
}