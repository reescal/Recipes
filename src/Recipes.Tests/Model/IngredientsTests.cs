using Microsoft.AspNetCore.Mvc;
using static Recipes.Tests.Testing;
using Recipes.Api;
using Recipes.Shared.Models;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Moq;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Recipes.Shared.Constants;
using MediatR;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.Create;
using Recipes.Features.Ingredients.Update;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Tests.Model;

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
        ingredients.ShouldBeAssignableTo<IEnumerable<IngredientGetResponse>>();
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
        ingredientResult.ShouldBeAssignableTo<IngredientGetResponse>();
        ingredient.Id.ShouldBe((ingredientResult as IngredientGetResponse)!.Id);
    }

    public async Task ShouldCreateIngredient()
    {
        var ingredient = new IngredientCreateRequest();
        var req = CreateMockRequest(ingredient);

        var result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        var validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required("Image link"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientCreateRequest.Properties)));

        ingredient.Image = "http://invalid/link";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("image link"));

        ingredient.Image = "https://valid/link.png";
        ingredient.Properties = new();
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientCreateRequest.Properties)));

        ingredient.Properties = new() { new() };
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("Language"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientProperties.Name)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientProperties.Description)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientProperties.Type)));

        var ingredientProperty = ingredient.Properties.First();
        ingredientProperty.Name = string.Empty;
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientProperties.Name)));

        ingredientProperty.Name = "Aa";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooShort(nameof(IngredientProperties.Name)));

        ingredientProperty.Name = "A23456789012345678901234567890123456789012345678901";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(IngredientProperties.Name)));

        ingredientProperty.Name = "Valid";
        ingredientProperty.Description = string.Empty;
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientProperties.Description)));

        ingredientProperty.Description = "Valid";
        ingredientProperty.Type = string.Empty;
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientProperties.Type)));

        ingredientProperty.Type = "Aa";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooShort(nameof(IngredientProperties.Type)));

        ingredientProperty.Type = "A23456789012345678901234567890123456789012345678901";
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(IngredientProperties.Type)));

        ingredientProperty.Type = "Valid";
        ingredientProperty.LangId = 0;
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("Language"));

        ingredientProperty.LangId = Shared.Enums.Lang.English;
        ingredient.Properties.Add(new() { LangId = Shared.Enums.Lang.Spanish, Description = "Descripcion", Name = "Nombre", Type = "Tipo" });
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<Guid>();

        req = new Mock<HttpRequest>();
        result = await _sut.GetIngredient(req.Object, (Guid)resultObject);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateIngredient()
    {
        var ingredient = await CreateIngredient();

        var ingredientUpdate = new IngredientUpdateRequest()
        {
            Id = Guid.NewGuid(),
            Image = ingredient.Image,
            Properties = ingredient.Properties
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
        ingredientResult.ShouldBeAssignableTo<IngredientGetResponse>();
        (ingredientResult as IngredientGetResponse)!.Image.ShouldNotBe(ingredient.Image);
        (ingredientResult as IngredientGetResponse)!.Image.ShouldBe(ingredientUpdate.Image);
    }
}
