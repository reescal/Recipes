namespace Recipes.Shared;

public class ApiResponse
{
    public IEnumerable<string> Errors { get; }

    public ApiResponse()
    {
        Errors = null;
    }

    public ApiResponse(string error)
    {
        Errors = new List<string> { error };
    }

    public ApiResponse(IEnumerable<string> errors)
    {
        Errors = errors;
    }

    public bool Success => Errors == null;
}

public class ApiResponse<TModel> : ApiResponse
{
    public TModel Result { get; }

    public ApiResponse(TModel result) : base()
    {
        Result = result;
    }

    public static ApiResponse<T> FromResult<T>(T result) => new ApiResponse<T>(result);
}