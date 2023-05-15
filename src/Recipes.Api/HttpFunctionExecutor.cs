using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Recipes.Shared;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Recipes.Api;
public class HttpFunctionExecutor : IHttpFunctionExecutor
{
    public async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> func)
    {
        try
        {
            return await func();
        }
        catch (ValidationException ex)
        {
            return new BadRequestObjectResult(string.Join('\n', ex.Errors));
        }
        catch (ApiException ex)
        {
            return ex.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(ex.ReasonPhrase),
                HttpStatusCode.NotFound => new NotFoundObjectResult(ex.ReasonPhrase),
                _ => new BadRequestResult(),
            };
        }
    }
}