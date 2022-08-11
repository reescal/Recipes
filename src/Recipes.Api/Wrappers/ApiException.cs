using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Recipes.Api.Wrappers;

public class ApiException : HttpResponseException
{
    public ApiException(string msg, int code) : base(new HttpResponseMessage((HttpStatusCode)code)
    {
        ReasonPhrase = msg
    })
    {
    }

    public IActionResult Exception
    {
        get
        {
            return Response.StatusCode switch
            {
                HttpStatusCode.BadRequest => new BadRequestObjectResult(Response.ReasonPhrase),
                HttpStatusCode.NotFound => new NotFoundObjectResult(Response.ReasonPhrase),
                _ => new BadRequestResult(),
            };
        }
    }
}
