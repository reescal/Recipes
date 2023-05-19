using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Recipes.Data.Entities;
using System.Collections.Generic;
using System.Net;
using static Recipes.Api.Wrappers.Helpers;
using System.Threading.Tasks;
using static Recipes.Shared.Constants.HttpMethods;
using static Recipes.Shared.Constants.ContentTypes;
using Recipes.Features.GroceryList.Get;
using Recipes.Features.GroceryList.AddGrocery;
using Recipes.Features.GroceryList.Update;
using Recipes.Shared;

namespace Recipes.Api;
public class GroceryLists
{
    private const string _name = nameof(GroceryLists);
    private readonly ILogger<GroceryLists> _logger;
    private readonly IMediator _mediator;
    private readonly IHttpFunctionExecutor _httpFunctionExecutor;

    public GroceryLists(ILogger<GroceryLists> log,
        IMediator mediator,
        IHttpFunctionExecutor httpFunctionExecutor)
    {
        _logger = log;
        _mediator = mediator;
        _httpFunctionExecutor = httpFunctionExecutor;
    }

    [FunctionName(nameof(GetGroceryList))]
    [OpenApiOperation(operationId: nameof(GetGroceryList), tags: new[] { _name })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(GetGroceryListResponse), Description = "The OK response")]
    public async Task<IActionResult> GetGroceryList(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetGroceryList)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new GetGroceryListRequest());

            return new OkObjectResult(ApiResponse<GetGroceryListResponse>.FromResult(response));
        });
    }

    [FunctionName(nameof(AddGrocery))]
    [OpenApiOperation(operationId: nameof(AddGrocery), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(HashSet<Grocery>), Description = nameof(Grocery), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: json, bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> AddGrocery(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name + $"/{nameof(AddGrocery)}")] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(AddGrocery)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var groceries = await DeserializeAsync<HashSet<Grocery>>(req);

            await _mediator.Send(new AddGroceryRequest { Grocery = groceries });

            return new NoContentResult();
        });
    }

    [FunctionName(nameof(UpdateGroceryList))]
    [OpenApiOperation(operationId: nameof(UpdateGroceryList), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(HashSet<Grocery>), Description = nameof(Grocery), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: json, bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> UpdateGroceryList(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(UpdateGroceryList)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var groceries = await DeserializeAsync<HashSet<Grocery>>(req);

            await _mediator.Send(new UpdateGroceryListRequest { Grocery = groceries });

            return new NoContentResult();
        });
    }
}
