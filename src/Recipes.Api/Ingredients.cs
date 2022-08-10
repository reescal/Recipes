using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Recipes.Api.Models;
using Recipes.Api.Services;

namespace Recipes.Api
{
    public class Ingredients
    {
        private readonly ILogger<Ingredients> _logger;
        private readonly IIngredientsService _ingredientService;

        public Ingredients(ILogger<Ingredients> log, IIngredientsService ingredientService)
        {
            _logger = log;
            _ingredientService = ingredientService;
        }

        [FunctionName("GetIngredients")]
        [OpenApiOperation(operationId: "GetIngredients", tags: new[] { "Ingredients" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<Ingredient>), Description = "The OK response")]
        public IActionResult GetIngredients(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "Ingredients")] HttpRequest req)
        {
            _logger.LogError("C# HTTP trigger function processed a request.");
            try
            {
                var ingredients = _ingredientService.Get();
                return new OkObjectResult(ingredients);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("GetIngredient")]
        [OpenApiOperation(operationId: "GetIngredient", tags: new[] { "Ingredients" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Ingredient), Description = "The OK response")]
        public async Task<IActionResult> GetIngredient(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "Ingredients/{id:Guid}")] HttpRequest req, Guid id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var ingredients = await _ingredientService.GetAsync(id);

            if (ingredients == null)
                return new NotFoundResult();

            return new OkObjectResult(ingredients);
        }

        [FunctionName("GetIngredientNames")]
        [OpenApiOperation(operationId: "GetIngredientNames", tags: new[] { "Ingredients" })]
        [OpenApiParameter(name: "langId", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The **Lang Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<string>), Description = "The OK response")]
        public IActionResult GetIngredientNames(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Ingredients/Names")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var query = req.Query.TryGetValue("langId", out Microsoft.Extensions.Primitives.StringValues param);
            int langId = 0;
            if (query)
                _ = int.TryParse(param, out langId);

            var ingredients = _ingredientService.GetNames(langId);
            return new OkObjectResult(ingredients);
        }

        [FunctionName("CreateIngredient")]
        [OpenApiOperation(operationId: "CreateIngredient", tags: new[] { "Ingredients" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(IngredientCreate), Description = "Ingredient", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> CreateIngredient(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Ingredients")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<IngredientCreate>(requestBody);

            var id = await _ingredientService.InsertAsync(input);

            return new OkObjectResult(id);
        }

        [FunctionName("UpdateIngredient")]
        [OpenApiOperation(operationId: "UpdateIngredient", tags: new[] { "Ingredients" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The **Id** parameter")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(IngredientCreate), Description = "Ingredient", Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Ingredient), Description = "The OK response")]
        public async Task<IActionResult> UpdateIngredient(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Ingredients/{id:Guid}")] HttpRequest req, Guid id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<IngredientCreate>(requestBody);

            var obj = await _ingredientService.UpdateAsync(id, input);

            return new OkObjectResult(obj);
        }
    }
}

