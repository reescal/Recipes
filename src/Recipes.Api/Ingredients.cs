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
using Recipes.Shared.Models;
using Recipes.Api.Services;
using Recipes.Api.Wrappers;
using static Recipes.Shared.Constants.Constants;
using static Recipes.Shared.Constants.ContentTypes;
using static Recipes.Shared.Constants.HttpMethods;
using static Recipes.Api.Wrappers.Helpers;
using FluentValidation;
using Recipes.Shared.Enums;

namespace Recipes.Api;

public class Ingredients
{
    private const string _name = nameof(Ingredients);
    private readonly ILogger<Ingredients> _logger;
    private readonly IIngredientsService _ingredientService;
    private readonly IValidator<IngredientCreate> _validator;

    public Ingredients(ILogger<Ingredients> log, IIngredientsService ingredientService, IValidator<IngredientCreate> validator)
    {
        _logger = log;
        _ingredientService = ingredientService;
        _validator = validator;
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
    [OpenApiParameter(name: langId, In = ParameterLocation.Query, Required = false, Type = typeof(Lang), Description = "The **Lang Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<ComplexEntity>), Description = "The OK response")]
    public IActionResult GetIngredientNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name + "/Names")] HttpRequest req)
    {
        try
        {
            var lang = CheckQueryLangId(req.Query);

            var ingredients = _ingredientService.GetNames(lang);

            return new OkObjectResult(ingredients);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(GetIngredientTypes))]
    [OpenApiOperation(operationId: nameof(GetIngredientTypes), tags: new[] { _name })]
    [OpenApiParameter(name: langId, In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The **Lang Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<EntityTypes>), Description = "The OK response")]
    public IActionResult GetIngredientTypes(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name + "/Types")] HttpRequest req)
    {
        try
        {
            var lang = CheckQueryLangId(req.Query);

            var ingredients = _ingredientService.GetTypes(lang);

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
        var input = await DeserializeAsync<IngredientCreate>(req);

        var result = await _validator.ValidateAsync(input);
        if (!result.IsValid)
            return new BadRequestObjectResult(result.Errors);

        var id = await _ingredientService.InsertAsync(input);
        return new OkObjectResult(id);
    }

    [FunctionName(nameof(UpdateIngredient))]
    [OpenApiOperation(operationId: nameof(UpdateIngredient), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(IngredientCreate), Description = nameof(Ingredient), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Ingredient), Description = "The OK response")]
    public async Task<IActionResult> UpdateIngredient(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        var input = await DeserializeAsync<IngredientCreate>(req);

        var result = await _validator.ValidateAsync(input);
        if (!result.IsValid)
            return new BadRequestObjectResult(result.Errors);

        try
        {
            var obj = await _ingredientService.UpdateAsync(id, input);
            return new OkObjectResult(obj);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }
}

