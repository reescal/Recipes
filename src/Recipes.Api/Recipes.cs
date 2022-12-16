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
using Recipes.Shared.Enums;
using FluentValidation;

namespace Recipes.Api;

public class Recipes
{
    private const string _name = nameof(Recipes);
    private readonly ILogger<Recipes> _logger;
    private readonly IRecipesService _recipeService;
    private readonly IValidator<RecipeCreate> _validator;

    public Recipes(ILogger<Recipes> log, IRecipesService recipeService, IValidator<RecipeCreate> validator)
    {
        _logger = log;
        _recipeService = recipeService;
        _validator = validator;
    }

    [FunctionName(nameof(GetRecipe))]
    [OpenApiOperation(operationId: nameof(GetRecipe), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Recipe), Description = "The OK response")]
    public async Task<IActionResult> GetRecipe(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        try
        {
            var recipe = await _recipeService.GetAsync(id);
            return new OkObjectResult(recipe);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(GetRecipeNames))]
    [OpenApiOperation(operationId: nameof(GetRecipeNames), tags: new[] { _name })]
    [OpenApiParameter(name: langId, In = ParameterLocation.Query, Required = false, Type = typeof(Lang), Description = "The **Lang Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<ComplexEntity>), Description = "The OK response")]
    public IActionResult GetRecipeNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name + "/Names")] HttpRequest req)
    {
        try
        {
            Lang? lang = null;

            if (req.Query.ContainsKey(langId))
            {
                LangExists(req.Query[langId]);
                lang = Enum.Parse<Lang>(req.Query[langId]);
            }

            var ingredients = _recipeService.GetNames(lang);

            return new OkObjectResult(ingredients);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(GetRecipesByIngredients))]
    [OpenApiOperation(operationId: nameof(GetRecipesByIngredients), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(Guid[]), Description = nameof(Guid), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<string>), Description = "The OK response")]
    public async Task<IActionResult> GetRecipesByIngredients(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name + "/Ingredients")] HttpRequest req)
    {
        try
        {
            var ids = await DeserializeAsync<Guid[]>(req);

            var recipes = _recipeService.GetByIngredients(ids);

            return new OkObjectResult(recipes);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(CreateRecipe))]
    [OpenApiOperation(operationId: nameof(CreateRecipe), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(RecipeCreate), Description = nameof(Recipe), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: textPlain, bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> CreateRecipe(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name)] HttpRequest req)
    {
        var input = await DeserializeAsync<RecipeCreate>(req);

        var result = await _validator.ValidateAsync(input);
        if (!result.IsValid)
            return new BadRequestObjectResult(result.Errors);

        try
        {
            var id = await _recipeService.InsertAsync(input);
            return new OkObjectResult(id);
        }
        catch(ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(UpdateRecipe))]
    [OpenApiOperation(operationId: nameof(UpdateRecipe), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(RecipeCreate), Description = nameof(Recipe), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Recipe), Description = "The OK response")]
    public async Task<IActionResult> UpdateRecipe(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        var input = await DeserializeAsync<RecipeCreate>(req);

        var result = await _validator.ValidateAsync(input);
        if (!result.IsValid)
            return new BadRequestObjectResult(result.Errors);

        try
        {
            var obj = await _recipeService.UpdateAsync(id, input);
            return new OkObjectResult(obj);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }
}

