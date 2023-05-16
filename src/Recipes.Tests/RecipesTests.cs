using Microsoft.AspNetCore.Mvc;
using static Recipes.Tests.Testing;
using Recipes.Shared.Models;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Shared.Constants;
using MediatR;
using Recipes.Api;
using Recipes.Features.Recipes.GetById;
using Recipes.Features.Recipes.Create;
using Recipes.Data.Entities;
using Recipes.Features.Recipes.Update;
using Recipes.Features.Recipes.GetByIngredients;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Tests;

public class RecipesTests
{
    private readonly Api.Recipes _sut;

    public RecipesTests()
    {
        _sut = new Api.Recipes(Provider.GetRequiredService<ILogger<Api.Recipes>>(),
            Provider.GetRequiredService<IMediator>(),
            Provider.GetRequiredService<IHttpFunctionExecutor>());
    }

    public async Task ShouldGetRecipe()
    {
        var req = new Mock<HttpRequest>();

        var recipe = await CreateRecipe();

        var result = await _sut.GetRecipe(req.Object, Guid.NewGuid());

        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        result = await _sut.GetRecipe(req.Object, recipe.Id);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var oORV = ((OkObjectResult)result).Value;
        oORV.ShouldBeAssignableTo<ApiResponse<RecipeGetResponse>>();
        recipe.Id.ShouldBe((oORV as ApiResponse<RecipeGetResponse>)!.Result.Id);
    }

    public async Task ShouldGetRecipesByIngredients()
    {
        var ingredients = new List<Guid>();
        var req = CreateMockRequest(ingredients.ToArray());
        var result = await _sut.GetRecipesByIngredients(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        var validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeGetByIngredientsRequest.Ingredients)));

        ingredients = new List<Guid>() { Guid.NewGuid() };
        req = CreateMockRequest(ingredients.ToArray());
        result = await _sut.GetRecipesByIngredients(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(NotFound(nameof(Ingredient)));

        var recipe = await CreateRecipe();

        ingredients = new List<Guid>() { recipe.Ingredients.First().IngredientId };
        req = CreateMockRequest(ingredients.ToArray());
        result = await _sut.GetRecipesByIngredients(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        var oORV = ((OkObjectResult)result).Value;
        oORV.ShouldBeAssignableTo<ApiResponse<IEnumerable<RecipeGetResponse>>>();
        var entities = (oORV as ApiResponse<IEnumerable<RecipeGetResponse>>);
        entities!.Result.Select(x => x.Id).ShouldContain(recipe.Id);
    }

    public async Task ShouldCreateRecipe()
    {
        var recipe = new RecipeCreateRequest();
        var req = CreateMockRequest(recipe);

        var result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        var validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(7);
        validationFailures.Errors.ShouldContain(ValidationError.Required("Image link"));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Yield)));
        validationFailures.Errors.ShouldContain(ValidationError.InclusiveBetween(nameof(RecipeCreateRequest.Time), 5, 2880));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Name)));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Description)));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Type)));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Ingredients)));

        recipe.Image = "http://invalid/link";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(7);
        validationFailures.Errors.ShouldContain(ValidationError.Invalid("image link"));

        recipe.Image = "https://valid/link.png";
        recipe.Yield = "8s";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(6);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(RecipeCreateRequest.Yield)));

        recipe.Yield = new string('a', 51);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(6);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(RecipeCreateRequest.Yield)));

        recipe.Image = "https://valid/link.png";
        recipe.Yield = "8 servings";
        recipe.Time = 2881;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(5);
        validationFailures.Errors.ShouldContain(ValidationError.InclusiveBetween(nameof(RecipeCreateRequest.Time), 5, 2880));

        recipe.Time = 30;
        recipe.Name = new string('a', 2);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(4);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(RecipeCreateRequest.Name)));

        recipe.Name = new string('a', 51);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(4);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(RecipeCreateRequest.Name)));

        recipe.Name = "Valid";
        recipe.Type = new string('a', 2);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(RecipeCreateRequest.Type)));

        recipe.Type = new string('a', 51);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(RecipeCreateRequest.Type)));

        recipe.Type = "Valid";
        recipe.Description = "Valid";
        recipe.Ingredients = new();
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Ingredients)));

        recipe.Ingredients.Add(new());
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(Quantity)));
        validationFailures.Errors.ShouldContain(Responses.NotFound(nameof(Ingredient), new Guid()));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(IngredientRow.Preparation)));

        var recipeIngredient = recipe.Ingredients.First();
        recipeIngredient.Preparation = new string('a', 51);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(IngredientRow.Preparation)));

        recipeIngredient.Preparation = new string('a', 50);
        recipeIngredient.Quantity = new();
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(Quantity.Value)));
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(Quantity.Unit)));

        recipeIngredient.Quantity.Value = 1;
        recipeIngredient.Quantity.Unit = new string('a', 11);
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(2);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(Quantity.Unit)));

        recipeIngredient.Quantity.Unit = new string('a', 10);
        var ingredient = await CreateIngredient();
        recipeIngredient.IngredientId = ingredient.Id;
        recipe.Materials = new() { new() };
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(Responses.NotFound(nameof(Material), new Guid()));

        var material = await CreateMaterial();
        recipe.Materials = new() { new() { MaterialId = material.Id } };
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse<Guid>>();

        req = new Mock<HttpRequest>();
        result = await _sut.GetRecipe(req.Object, ((ApiResponse<Guid>)resultObject).Result);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateRecipe()
    {
        var recipe = await CreateRecipe();

        var recipeUpdate = new RecipeUpdateRequest()
        {
            Id = Guid.NewGuid(),
            Image = recipe.Image,
            Yield = recipe.Yield,
            Time = recipe.Time,
            Name = recipe.Name,
            Description = recipe.Description,
            Type = recipe.Type,
            Tags = recipe.Tags,
            Ingredients = recipe.Ingredients
        };
        var req = CreateMockRequest(recipeUpdate);

        var result = await _sut.UpdateRecipe(req.Object);
        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        recipeUpdate.Id = recipe.Id;
        recipeUpdate.Image = recipe.Image + "_updated.png";
        req = CreateMockRequest(recipeUpdate);
        result = await _sut.UpdateRecipe(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        var recipeResult = ((OkObjectResult)result).Value;
        recipeResult.ShouldBeAssignableTo<ApiResponse<RecipeGetResponse>>();
        (recipeResult as ApiResponse<RecipeGetResponse>)!.Result.Image.ShouldNotBe(recipe.Image);
        (recipeResult as ApiResponse<RecipeGetResponse>)!.Result.Image.ShouldBe(recipeUpdate.Image);
    }
}
