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
using Recipes.Api.Models;
using Recipes.Api.Services;
using Recipes.Api.Wrappers;
using static Recipes.Api.Constants.Constants;
using static Recipes.Api.Constants.ContentTypes;
using static Recipes.Api.Constants.HttpMethods;
using static Recipes.Api.Wrappers.Helpers;

namespace Recipes.Api;

public class Ingredients
{
    private const string _name = nameof(Ingredients);
    private readonly ILogger<Ingredients> _logger;
    private readonly IIngredientsService _ingredientService;

    public Ingredients(ILogger<Ingredients> log, IIngredientsService ingredientService)
    {
        _logger = log;
        _ingredientService = ingredientService;
    }

    [FunctionName(nameof(GetIngredients))]
    [OpenApiOperation(operationId: nameof(GetIngredients), tags: new[] { _name })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<Ingredient>), Description = "The OK response")]
    public IActionResult GetIngredients(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name)] HttpRequest req)
    {
        var ingredients = _ingredientService.Get();
        return new OkObjectResult(ingredients);
    }

    [FunctionName(nameof(GetIngredient))]
    [OpenApiOperation(operationId: nameof(GetIngredient), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Ingredient), Description = "The OK response")]
    public async Task<IActionResult> GetIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        try
        {
            var ingredient = await _ingredientService.GetAsync(id);
            return new OkObjectResult(ingredient);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(GetIngredientNames))]
    [OpenApiOperation(operationId: nameof(GetIngredientNames), tags: new[] { _name })]
    [OpenApiParameter(name: langId, In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **Lang Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<string>), Description = "The OK response")]
    public IActionResult GetIngredientNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name + "/Names")] HttpRequest req)
    {
        try
        {
            _ = int.TryParse(req.Query["langId"], out int langId);

            var ingredients = _ingredientService.GetNames(langId);

            return new OkObjectResult(ingredients);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(CreateIngredient))]
    [OpenApiOperation(operationId: nameof(CreateIngredient), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(IngredientCreate), Description = nameof(Ingredient), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: textPlain, bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> CreateIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name)] HttpRequest req)
    {
        try
        {
            var input = await DeserializeAsync<IngredientCreate>(req);

            var id = await _ingredientService.InsertAsync(input);

            return new OkObjectResult(id);
        }
        catch(ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(UpdateIngredient))]
    [OpenApiOperation(operationId: nameof(UpdateIngredient), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(IngredientCreate), Description = nameof(Ingredient), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Ingredient), Description = "The OK response")]
    public async Task<IActionResult> UpdateIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        try
        {
            var input = await DeserializeAsync<IngredientCreate>(req);

            var obj = await _ingredientService.UpdateAsync(id, input);

            return new OkObjectResult(obj);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }
}

