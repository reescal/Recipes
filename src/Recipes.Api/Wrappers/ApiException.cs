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
            switch (Response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    return new BadRequestObjectResult(Response.ReasonPhrase);
                case HttpStatusCode.NotFound:
                    return new NotFoundObjectResult(Response.ReasonPhrase);
                default:
                    return new BadRequestResult();
            }
        }
    }
}
