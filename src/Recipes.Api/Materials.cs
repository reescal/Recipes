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
using Recipes.Features.Materials.GetAll;
using Recipes.Features.Materials.GetById;
using Recipes.Features.Materials.Create;
using Recipes.Features.Materials.Update;
using Recipes.Data.Entities;

namespace Recipes.Api;

public class Materials
{
    private const string _name = nameof(Materials);
    private readonly ILogger<Materials> _logger;
    private readonly IMediator _mediator;
    private readonly IHttpFunctionExecutor _httpFunctionExecutor;

    public Materials(ILogger<Materials> log,
        IMediator mediator,
        IHttpFunctionExecutor httpFunctionExecutor)
    {
        _logger = log;
        _mediator = mediator;
        _httpFunctionExecutor = httpFunctionExecutor;
    }

    [FunctionName(nameof(GetMaterials))]
    [OpenApiOperation(operationId: nameof(GetMaterials), tags: new[] { _name })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(IEnumerable<Material>), Description = "The OK response")]
    public async Task<IActionResult> GetMaterials(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(GetMaterials)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new MaterialsGetAllRequest());

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(GetMaterial))]
    [OpenApiOperation(operationId: nameof(GetMaterial), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Material), Description = "The OK response")]
    public async Task<IActionResult> GetMaterial(
        [HttpTrigger(AuthorizationLevel.Anonymous, get,  Route = _name + "/{id:Guid}")] HttpRequest req, Guid id)
    {
        _logger.LogInformation($"{nameof(GetMaterial)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var response = await _mediator.Send(new MaterialGetRequest { Id = id });

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(CreateMaterial))]
    [OpenApiOperation(operationId: nameof(CreateMaterial), tags: new[] { _name })]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(MaterialCreateRequest), Description = nameof(Material), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: textPlain, bodyType: typeof(Guid), Description = "The OK response")]
    public async Task<IActionResult> CreateMaterial(
        [HttpTrigger(AuthorizationLevel.Anonymous, post, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(CreateMaterial)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var input = await DeserializeAsync<MaterialCreateRequest>(req);

            var response = await _mediator.Send(input);

            return new OkObjectResult(response);
        });
    }

    [FunctionName(nameof(UpdateMaterial))]
    [OpenApiOperation(operationId: nameof(UpdateMaterial), tags: new[] { _name })]
    [OpenApiParameter(name: id, In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
    [OpenApiRequestBody(contentType: json, bodyType: typeof(MaterialUpdateRequest), Description = nameof(Material), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: json, bodyType: typeof(Material), Description = "The OK response")]
    public async Task<IActionResult> UpdateMaterial(
        [HttpTrigger(AuthorizationLevel.Anonymous, put, Route = _name)] HttpRequest req)
    {
        _logger.LogInformation($"{nameof(CreateMaterial)} function triggered.");

        return await _httpFunctionExecutor.ExecuteAsync(async () =>
        {
            var input = await DeserializeAsync<MaterialUpdateRequest>(req);

            var response = await _mediator.Send(input);

            return new OkObjectResult(response);
        });
    }
}

