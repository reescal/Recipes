using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using static Recipes.Shared.Constants.Constants;
using static Recipes.Shared.Constants.ContentTypes;
using static Recipes.Shared.Constants.HttpMethods;
using static Recipes.Api.Wrappers.Helpers;
using MediatR;
using Recipes.Features.Ingredients.GetAll;
using Recipes.Features.Ingredients.GetById;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.Create;
using Recipes.Features.Ingredients.Update;

namespace Recipes.Api;

public class Ingredients
{
    private const string _name = nameof(Ingredients);
    private readonly ILogger<Ingredients> _logger;
    private readonly IMediator _mediator;
    private readonly IHttpFunctionExecutor _httpFunctionExecutor;

    public Ingredients(ILogger<Ingredients> log,
        IMediator mediator,
        IHttpFunctionExecutor httpFunctionExecutor)
    {
        _logger = log;
        _mediator = mediator;
        _httpFunctionExecutor = httpFunctionExecutor;
    }

    [FunctionName(nameof(GetIngredients))]
    [OpenApiOperation(operationId: nameof(GetIngredients), tags: new[] { _name })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<Ingredient>), Description = "The OK response")]
    public async Task<IActionResult> GetIngredients(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetIngredients)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new IngredientsGetAllRequest());

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(GetIngredient))]
    [OpenApiOperation(operationId: nameof(GetIngredient), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Ingredient), Description = "The OK response")]
    public async Task<IActionResult> GetIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        _logger.LogInformation($"{nameof(GetIngredient)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new IngredientGetRequest { Id = id });

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(CreateIngredient))]
    [OpenApiOperation(operationId: nameof(CreateIngredient), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(IngredientCreateRequest), Description = nameof(Ingredient), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: textPlain, bodyType: typeof(Guid), Description = "The OK response")]
    public async Task<IActionResult> CreateIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(CreateIngredient)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var input = await DeserializeAsync<IngredientCreateRequest>(req);

            var response = await _mediator.Send(input);

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(UpdateIngredient))]
    [OpenApiOperation(operationId: nameof(UpdateIngredient), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(IngredientUpdateRequest), Description = nameof(Ingredient), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Ingredient), Description = "The OK response")]
    public async Task<IActionResult> UpdateIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(UpdateIngredient)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var input = await DeserializeAsync<IngredientUpdateRequest>(req);

            var response = await _mediator.Send(input);

            return new OkObjectResult(response);
        });
    }
}

