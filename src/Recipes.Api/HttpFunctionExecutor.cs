using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Recipes.Shared;
using System;
using System.Linq;
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
            return new BadRequestObjectResult(new ApiResponse(ex.Errors.Select(x => x.ErrorMessage)));
        }
        catch (ApiException ex)
        {
            var response = new ApiResponse(ex.ReasonPhrase);
            return ex.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                _ => new BadRequestResult(),
            };
        }
    }
}