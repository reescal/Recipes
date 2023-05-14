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
using Recipes.Shared.Enums;
using MediatR;
using Recipes.Features.Recipes.GetById;
using Recipes.Features.Recipes.GetAll;
using Recipes.Features.Recipes.GetByIngredients;
using Recipes.Features.Recipes.Create;
using Recipes.Data.Entities;
using Recipes.Features.Recipes.Update;

namespace Recipes.Api;

public class Recipes
{
    private const string _name = nameof(Recipes);
    private readonly ILogger<Recipes> _logger;
    private readonly IMediator _mediator;
    private readonly IHttpFunctionExecutor _httpFunctionExecutor;

    public Recipes(ILogger<Recipes> log,
        IMediator mediator,
        IHttpFunctionExecutor httpFunctionExecutor)
    {
        _logger = log;
        _mediator = mediator;
        _httpFunctionExecutor = httpFunctionExecutor;
    }

    [FunctionName(nameof(GetRecipes))]
    [OpenApiOperation(operationId: nameof(GetRecipes), tags: new[] { _name })]
    [OpenApiParameter(name: langId, In = ParameterLocation.Query, Required = false, Type = typeof(Lang), Description = "The **Lang Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<RecipeGetResponse>), Description = "The OK response")]
    public async Task<IActionResult> GetRecipes(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name + "/Names")] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetRecipes)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new RecipesGetAllRequest());

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(GetRecipe))]
    [OpenApiOperation(operationId: nameof(GetRecipe), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(RecipeGetResponse), Description = "The OK response")]
    public async Task<IActionResult> GetRecipe(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        _logger.LogInformation($"{nameof(GetRecipe)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new RecipeGetRequest { Id = id });

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(GetRecipesByIngredients))]
    [OpenApiOperation(operationId: nameof(GetRecipesByIngredients), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(Guid[]), Description = nameof(Guid), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<RecipeGetResponse>), Description = "The OK response")]
    public async Task<IActionResult> GetRecipesByIngredients(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name + "/Ingredients")] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetRecipesByIngredients)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var ids = await DeserializeAsync<List<Guid>>(req);
            var response = await _mediator.Send(new RecipeGetByIngredientsRequest { Ingredients = ids });

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(CreateRecipe))]
    [OpenApiOperation(operationId: nameof(CreateRecipe), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(RecipeCreateRequest), Description = nameof(Recipe), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: textPlain, bodyType: typeof(Guid), Description = "The OK response")]
    public async Task<IActionResult> CreateRecipe(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(CreateRecipe)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var input = await DeserializeAsync<RecipeCreateRequest>(req);

            var response = await _mediator.Send(input);

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(UpdateRecipe))]
    [OpenApiOperation(operationId: nameof(UpdateRecipe), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(RecipeUpdateRequest), Description = nameof(Recipe), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Recipe), Description = "The OK response")]
    public async Task<IActionResult> UpdateRecipe(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(UpdateRecipe)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var input = await DeserializeAsync<RecipeUpdateRequest>(req);

            var response = await _mediator.Send(input);

            return new OkObjectResult(response);
        });
    }
}

