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

public class MaterialsTests
{
    private readonly Materials _sut;

    public MaterialsTests()
    {
        _sut = new Materials(Provider.GetRequiredService<ILogger<Materials>>(),
            Provider.GetRequiredService<IMaterialsService>(),
            Provider.GetRequiredService<IValidator<MaterialCreate>>());
    }

    public void ShouldGetMaterials()
    {
        var req = new Mock<HttpRequest>();

        var result = _sut.GetMaterials(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var materials = ((OkObjectResult)result).Value;
        materials.ShouldBeAssignableTo<IEnumerable<Material>>();
    }

    public async Task ShouldGetMaterial()
    {
        var req = new Mock<HttpRequest>();

        var result = await _sut.GetMaterial(req.Object, Guid.NewGuid());
        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        var material = await CreateMaterial();

        result = await _sut.GetMaterial(req.Object, material.Id);
        result.ShouldBeAssignableTo<OkObjectResult>();
        var materialResult = ((OkObjectResult)result).Value;
        materialResult.ShouldBeAssignableTo<Material>();
        material.Id.ShouldBe((materialResult as Material)!.Id);
    }

    public void ShouldGetMaterialNames()
    {
        var req = new Mock<HttpRequest>();
        req.Setup(x => x.Query).Returns(new QueryCollection());

        var result = _sut.GetMaterialNames(req.Object);

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

        result = _sut.GetMaterialNames(req.Object);

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

        result = _sut.GetMaterialNames(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var entitiesResult = ((OkObjectResult)result).Value;
        entitiesResult.ShouldBeAssignableTo<IEnumerable<ComplexEntity>>();
        var complexEntities = entitiesResult as IEnumerable<ComplexEntity>;
        complexEntities!
            .All(x => x.Properties
                        .All(y => y.LangId == Shared.Enums.Lang.English))
            .ShouldBeTrue();
    }

    public async Task ShouldCreateMaterial()
    {
        var material = new MaterialCreate();
        var req = CreateMockRequest(material);

        var result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        var validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required("Image link"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialCreate.Properties)));

        material.Image = "http://invalid/link";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("image link"));

        material.Image = "https://valid/link.png";
        material.Properties = new();
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialCreate.Properties)));

        material.Properties = new() { new() };
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("Language"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialProperties.Name)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialProperties.Description)));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialProperties.Type)));

        var materialProperty = material.Properties.First();
        materialProperty.Name = string.Empty;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialProperties.Name)));

        materialProperty.Name = "Aa";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooShort(nameof(MaterialProperties.Name)));

        materialProperty.Name = "A23456789012345678901234567890123456789012345678901";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(4);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(MaterialProperties.Name)));

        materialProperty.Name = "Valid";
        materialProperty.Description = string.Empty;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(3);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialProperties.Description)));

        materialProperty.Description = "Valid";
        materialProperty.Type = string.Empty;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialProperties.Type)));

        materialProperty.Type = "Aa";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooShort(nameof(MaterialProperties.Type)));

        materialProperty.Type = "A23456789012345678901234567890123456789012345678901";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.TooLong(nameof(MaterialProperties.Type)));

        materialProperty.Type = "Valid";
        materialProperty.LangId = 0;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(1);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Invalid("Language"));

        materialProperty.LangId = Shared.Enums.Lang.English;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<string>();
        var parsed = Guid.TryParse(resultObject as string, out var guid);
        parsed.ShouldBeTrue();

        req = new Mock<HttpRequest>();
        result = await _sut.GetMaterial(req.Object, guid);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateMaterial()
    {
        var material = await CreateMaterial();

        var materialUpdate = new MaterialCreate()
        {
            Image = material.Image,
            Properties = material.Properties
        };
        var req = CreateMockRequest(materialUpdate);

        var result = await _sut.UpdateMaterial(req.Object, Guid.NewGuid());

        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        materialUpdate.Image = material.Image + "_updated.png";

        req = CreateMockRequest(materialUpdate);

        result = await _sut.UpdateMaterial(req.Object, material.Id);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var materialResult = ((OkObjectResult)result).Value;
        materialResult.ShouldBeAssignableTo<Material>();
        (materialResult as Material)!.Image.ShouldNotBe(material.Image);
        (materialResult as Material)!.Image.ShouldBe(materialUpdate.Image);
    }
}
