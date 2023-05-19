using Microsoft.AspNetCore.Mvc;
using static Recipes.Tests.Testing;
using Recipes.Api;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Shared.Constants;
using MediatR;
using Recipes.Features.Ingredients.Create;
using Recipes.Features.Ingredients.Update;
using Recipes.Features.Ingredients.GetById;
using Recipes.Shared;

namespace Recipes.Tests;

public class IngredientsTests
{
    private readonly Ingredients _sut;

    public IngredientsTests()
    {
        _sut = new Ingredients(Provider.GetRequiredService<ILogger<Ingredients>>(),
            Provider.GetRequiredService<IMediator>(),
            Provider.GetRequiredService<IHttpFunctionExecutor>());
    }

    public async Task ShouldGetIngredients()
    {
        var req = new Mock<HttpRequest>();

        var result = await _sut.GetIngredients(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var ingredients = ((OkObjectResult)result).Value;
        ingredients.ShouldBeAssignableTo<ApiResponse<IEnumerable<IngredientGetResponse>>>();
    }

    public async Task ShouldGetIngredient()
    {
        var req = new Mock<HttpRequest>();

        var result = await _sut.GetIngredient(req.Object, Guid.NewGuid());
        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        var ingredient = await CreateIngredient();

        result = await _sut.GetIngredient(req.Object, ingredient.Id);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var ingredientResult = ((OkObjectResult)result).Value;
        ingredientResult.ShouldBeAssignableTo<ApiResponse<IngredientGetResponse>>();
        ingredient.Id.ShouldBe((ingredientResult as ApiResponse<IngredientGetResponse>)!.Result.Id);
    }

    public async Task ShouldCreateIngredient()
    {
        var ingredient = new IngredientCreateRequest();
        var req = CreateMockRequest(ingredient);

        var result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        var validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(5);
        validationFailures.Errors.ShouldContain(ValidationError.Required("Image link"));

        ingredient.Image = "http://invalid/link";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(5);
        validationFailures.Errors.ShouldContain(ValidationError.Invalid("image link"));
        validationFailures.Errors.ShouldContain(ValidationError.Required("Nutritional info"));

        ingredient.Image = "https://valid/link.png";
        ingredient.NutritionalInfo = "http://invalid/link";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(4);
        validationFailures.Errors.ShouldContain(ValidationError.Invalid("Nutritional info"));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(IngredientCreateRequest.Name)));

        ingredient.NutritionalInfo = "https://valid/link.png";
        ingredient.Name = new string('a', 2);
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(IngredientCreateRequest.Name)));

        ingredient.Name = new string('a', 51);
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(IngredientCreateRequest.Name)));

        ingredient.Name = "Valid";
        ingredient.Description = string.Empty;
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(2);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(IngredientCreateRequest.Description)));

        ingredient.Description = "Valid";
        ingredient.Type = string.Empty;
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(IngredientCreateRequest.Type)));

        ingredient.Type = new string('a', 2);
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(IngredientCreateRequest.Type)));

        ingredient.Type = new string('a', 51);
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(IngredientCreateRequest.Type)));

        ingredient.Type = "Valid";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse<Guid>>();

        req = new Mock<HttpRequest>();
        result = await _sut.GetIngredient(req.Object, ((ApiResponse<Guid>)resultObject).Result);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateIngredient()
    {
        var ingredient = await CreateIngredient();

        var ingredientUpdate = new IngredientUpdateRequest()
        {
            Id = Guid.NewGuid(),
            Image = ingredient.Image,
            Name = ingredient.Name,
            Description = ingredient.Description,
            Type = ingredient.Type
        };
        var req = CreateMockRequest(ingredientUpdate);

        var result = await _sut.UpdateIngredient(req.Object);

        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        ingredientUpdate.Id = ingredient.Id;
        ingredientUpdate.Image = ingredient.Image + "_updated.png";

        req = CreateMockRequest(ingredientUpdate);

        result = await _sut.UpdateIngredient(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var ingredientResult = ((OkObjectResult)result).Value;
        ingredientResult.ShouldBeAssignableTo<ApiResponse<IngredientGetResponse>>();
        (ingredientResult as ApiResponse<IngredientGetResponse>)!.Result.Image.ShouldNotBe(ingredient.Image);
        (ingredientResult as ApiResponse<IngredientGetResponse>)!.Result.Image.ShouldBe(ingredientUpdate.Image);
    }
}
