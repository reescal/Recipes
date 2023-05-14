using Microsoft.AspNetCore.Mvc;
using static Recipes.Tests.Testing;
using Recipes.Shared.Models;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.Results;
using Recipes.Shared.Constants;
using MediatR;
using Recipes.Api;
using Recipes.Features.Recipes.GetById;
using Recipes.Features.Recipes.Create;
using Recipes.Data.Entities;
using Recipes.Features.Recipes.Update;
using Recipes.Features.Recipes.GetByIngredients;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Tests.Model;

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
        oORV.ShouldBeAssignableTo<RecipeGetResponse>();
        recipe.Id.ShouldBe((oORV as RecipeGetResponse)!.Id);
    }

    public async Task ShouldGetRecipesByIngredients()
    {
        var ingredients = new List<Guid>();
        var req = CreateMockRequest(ingredients.ToArray());
        var result = await _sut.GetRecipesByIngredients(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        var validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeGetByIngredientsRequest.Ingredients)));

        ingredients = new List<Guid>() { Guid.NewGuid() };
        req = CreateMockRequest(ingredients.ToArray());
        result = await _sut.GetRecipesByIngredients(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(NotFound(nameof(Ingredient)));

        var recipe = await CreateRecipe();

        ingredients = new List<Guid>() { recipe.Ingredients.First().IngredientId };
        req = CreateMockRequest(ingredients.ToArray());
        result = await _sut.GetRecipesByIngredients(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        var oORV = ((OkObjectResult)result).Value;
        oORV.ShouldBeAssignableTo<IEnumerable<RecipeGetResponse>>();
        var entities = (oORV as IEnumerable<RecipeGetResponse>);
        entities!.Select(x => x.Id).ShouldContain(recipe.Id);
    }

    public async Task ShouldCreateRecipe()
    {
        var recipe = new RecipeCreateRequest();
        var req = CreateMockRequest(recipe);

        var result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        var validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(5);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required("Image link"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Yield)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.InclusiveBetween(nameof(RecipeCreateRequest.Time), 5, 2880));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Properties)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Ingredients)));

        recipe.Image = "http://invalid/link";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(5);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("image link"));

        recipe.Image = "https://valid/link.png";
        recipe.Yield = "8 servings";
        recipe.Time = 2881;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.InclusiveBetween(nameof(RecipeCreateRequest.Time), 5, 2880));

        recipe.Time = 30;
        recipe.Properties = new();
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Properties)));

        recipe.Properties = new() { new() };
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(5);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("Language"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeProperties.Name)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeProperties.Description)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeProperties.Type)));

        var recipeProperty = recipe.Properties.First();
        recipeProperty.Name = string.Empty;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(5);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeProperties.Name)));

        recipeProperty.Name = "Aa";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(5);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooShort(nameof(RecipeProperties.Name)));

        recipeProperty.Name = "A23456789012345678901234567890123456789012345678901";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(5);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(RecipeProperties.Name)));

        recipeProperty.Name = "Valid";
        recipeProperty.Type = string.Empty;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeProperties.Type)));

        recipeProperty.Type = "Aa";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooShort(nameof(RecipeProperties.Type)));

        recipeProperty.Type = "A23456789012345678901234567890123456789012345678901";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(RecipeProperties.Type)));

        recipeProperty.Type = "Valid";
        recipeProperty.Description = string.Empty;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeProperties.Description)));

        recipeProperty.Description = "Valid";
        recipeProperty.LangId = 0;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("Language"));

        recipeProperty.LangId = Shared.Enums.Lang.English;
        recipe.Properties.Add(new() { LangId = Shared.Enums.Lang.Spanish, Description = "Descripcion", Name = "Nombre", Type = "Tipo" });
        recipe.Ingredients = new();
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(RecipeCreateRequest.Ingredients)));

        recipe.Ingredients = new() { new() };
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(Quantity)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(Responses.NotFound(nameof(Ingredient), new Guid()));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientRow.Preparation)));

        var recipeIngredient = recipe.Ingredients.First();
        recipeIngredient.Preparation = string.Empty;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientRow.Preparation)));

        recipeIngredient.Preparation = "012345678901234567890123456789012345678901234567890";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(IngredientRow.Preparation)));

        recipeIngredient.Preparation = "01234567890123456789012345678901234567890123456789";
        recipeIngredient.Quantity = new();
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(Quantity.Value)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(Quantity.Unit)));

        recipeIngredient.Quantity.Value = 1;
        recipeIngredient.Quantity.Unit = string.Empty;
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(Quantity.Unit)));

        recipeIngredient.Quantity.Unit = "01234567890";
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(Quantity.Unit)));

        recipeIngredient.Quantity.Unit = "0123456789";
        var ingredient = await CreateIngredient();
        recipeIngredient.IngredientId = ingredient.Id;
        recipe.Materials = new() { new() };
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(Responses.NotFound(nameof(Material), new Guid()));

        var material = await CreateMaterial();
        recipe.Materials = new() { new() { MaterialId = material.Id } };
        req = CreateMockRequest(recipe);
        result = await _sut.CreateRecipe(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<Guid>();

        req = new Mock<HttpRequest>();
        result = await _sut.GetRecipe(req.Object, (Guid)resultObject);
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
            Properties = recipe.Properties,
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
        recipeResult.ShouldBeAssignableTo<RecipeGetResponse>();
        (recipeResult as RecipeGetResponse)!.Image.ShouldNotBe(recipe.Image);
        (recipeResult as RecipeGetResponse)!.Image.ShouldBe(recipeUpdate.Image);
    }
}
