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
using static Recipes.Api.Constants.Constants;
using static Recipes.Api.Constants.ContentTypes;
using static Recipes.Api.Constants.HttpMethods;
using static Recipes.Api.Wrappers.Helpers;

namespace Recipes.Api;

public class Materials
{
    private const string _name = nameof(Materials);
    private readonly ILogger<Materials> _logger;
    private readonly IMaterialsService _materialService;

    public Materials(ILogger<Materials> log, IMaterialsService materialService)
    {
        _logger = log;
        _materialService = materialService;
    }

    [FunctionName(nameof(GetMaterials))]
    [OpenApiOperation(operationId: nameof(GetMaterials), tags: new[] { _name })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<Material>), Description = "The OK response")]
    public IActionResult GetMaterials(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name)] HttpRequest req)
    {
        var materials = _materialService.Get();
        return new OkObjectResult(materials);
    }

    [FunctionName(nameof(GetMaterial))]
    [OpenApiOperation(operationId: nameof(GetMaterial), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Material), Description = "The OK response")]
    public async Task<IActionResult> GetMaterial(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        try
        {
            var material = await _materialService.GetAsync(id);
            return new OkObjectResult(material);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(GetMaterialNames))]
    [OpenApiOperation(operationId: nameof(GetMaterialNames), tags: new[] { _name })]
    [OpenApiParameter(name: langId, In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **Lang Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<string>), Description = "The OK response")]
    public IActionResult GetMaterialNames(
        [HttpTrigger(AuthorizationLevel.Anonymous, get, Route = _name + "/Names")] HttpRequest req)
    {
        try
        {
            _ = int.TryParse(req.Query["langId"], out int langId);

            var materials = _materialService.GetNames(langId);

            return new OkObjectResult(materials);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(CreateMaterial))]
    [OpenApiOperation(operationId: nameof(CreateMaterial), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(MaterialCreate), Description = nameof(Material), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: textPlain, bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> CreateMaterial(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name)] HttpRequest req)
    {
        try
        {
            var input = await DeserializeAsync<MaterialCreate>(req);

            var id = await _materialService.InsertAsync(input);

            return new OkObjectResult(id);
        }
        catch(ApiException ex)
        {
            return ex.Exception;
        }
    }

    [FunctionName(nameof(UpdateMaterial))]
    [OpenApiOperation(operationId: nameof(UpdateMaterial), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(MaterialCreate), Description = nameof(Material), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Material), Description = "The OK response")]
    public async Task<IActionResult> UpdateMaterial(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        try
        {
            var input = await DeserializeAsync<MaterialCreate>(req);

            var obj = await _materialService.UpdateAsync(id, input);

            return new OkObjectResult(obj);
        }
        catch (ApiException ex)
        {
            return ex.Exception;
        }
    }
}

