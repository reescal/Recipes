using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using static Recipes.Tests.Testing;
using Recipes.Api;
using Recipes.Api.Services;
using Recipes.Shared.Models;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.Extensions.Primitives;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using static Recipes.Shared.Constants.Constants;
using Recipes.Shared.Constants;

namespace Recipes.Tests.Model;

public class IngredientsTests
{
    private readonly Ingredients _sut;

    public IngredientsTests()
    {
        _sut = new Ingredients(Provider.GetRequiredService<ILogger<Ingredients>>(),
            Provider.GetRequiredService<IIngredientsService>(),
            Provider.GetRequiredService<IValidator<IngredientCreate>>());
    }

    public void ShouldGetIngredients()
    {
        var req = new Mock<HttpRequest>();

        var result = _sut.GetIngredients(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var ingredients = ((OkObjectResult)result).Value;
        ingredients.ShouldBeAssignableTo<IEnumerable<Ingredient>>();
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
        ingredientResult.ShouldBeAssignableTo<Ingredient>();
        ingredient.Id.ShouldBe((ingredientResult as Ingredient)!.Id);
    }

    public void ShouldGetIngredientNames()
    {
        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Query).Returns(new QueryCollection());

        var result = _sut.GetIngredientNames(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();

        var qC = new QueryCollection(
            new Dictionary<string, StringValues>()
            {
                {
                    langId,
                    new StringValues("0")
                }
            });
        req.Setup(x => x.Query).Returns(qC);

        result = _sut.GetIngredientNames(req.Object);

        result.ShouldBeAssignableTo<BadRequestObjectResult>();

        qC = new QueryCollection(
            new Dictionary<string, StringValues>()
            {
                {
                    langId,
                    new StringValues(((int)Shared.Enums.Lang.English).ToString())
                }
            });
        req.Setup(x => x.Query).Returns(qC);

        result = _sut.GetIngredientNames(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var entitiesResult = ((OkObjectResult)result).Value;
        entitiesResult.ShouldBeAssignableTo<IEnumerable<ComplexEntity>>();
        var complexEntities = entitiesResult as IEnumerable<ComplexEntity>;
        complexEntities!
            .All(x => x.Properties
                        .All(y => y.LangId == Shared.Enums.Lang.English))
            .ShouldBeTrue();
    }

    public void ShouldGetIngredientTypes()
    {
        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Query).Returns(new QueryCollection());

        var result = _sut.GetIngredientTypes(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();

        var qC = new QueryCollection(
            new Dictionary<string, StringValues>()
            {
                {
                    langId,
                    new StringValues("0")
                }
            });
        req.Setup(x => x.Query).Returns(qC);

        result = _sut.GetIngredientTypes(req.Object);

        result.ShouldBeAssignableTo<BadRequestObjectResult>();

        qC = new QueryCollection(
            new Dictionary<string, StringValues>()
            {
                {
                    langId,
                    new StringValues(((int)Shared.Enums.Lang.English).ToString())
                }
            });
        req.Setup(x => x.Query).Returns(qC);

        result = _sut.GetIngredientTypes(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var typesResult = ((OkObjectResult)result).Value;
        typesResult.ShouldBeAssignableTo<HashSet<IngredientTypes>>();
        var ingredientTypes = typesResult as HashSet<IngredientTypes>;
        ingredientTypes!
            .All(x => x.LangId == Shared.Enums.Lang.English)
            .ShouldBeTrue();
    }

    public async Task ShouldCreateIngredient()
    {
        var ingredient = new IngredientCreate();
        var req = CreateMockRequest(ingredient);

        var result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        var validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required("Image link"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientCreate.Properties)));

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
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(IngredientCreate.Properties)));

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
        req = CreateMockRequest(ingredient);
        result = await _sut.CreateIngredient(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<string>();
        var parsed = Guid.TryParse(resultObject as string, out var guid);
        parsed.ShouldBeTrue();

        req = new Mock<HttpRequest>();
        result = await _sut.GetIngredient(req.Object, guid);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateIngredient()
    {
        var ingredient = await CreateIngredient();

        var ingredientUpdate = new IngredientCreate()
        {
            Image = ingredient.Image,
            Properties = ingredient.Properties
        };
        var req = CreateMockRequest(ingredientUpdate);

        var result = await _sut.UpdateIngredient(req.Object, Guid.NewGuid());

        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        ingredientUpdate.Image = ingredient.Image + "_updated.png";

        req = CreateMockRequest(ingredientUpdate);

        result = await _sut.UpdateIngredient(req.Object, ingredient.Id);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var ingredientResult = ((OkObjectResult)result).Value;
        ingredientResult.ShouldBeAssignableTo<Ingredient>();
        (ingredientResult as Ingredient)!.Image.ShouldNotBe(ingredient.Image);
        (ingredientResult as Ingredient)!.Image.ShouldBe(ingredientUpdate.Image);
    }
}
