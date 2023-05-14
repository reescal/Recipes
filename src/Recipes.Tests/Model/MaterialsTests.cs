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
using Recipes.Features.Materials.Create;
using Recipes.Features.Materials.Update;
using Recipes.Features.Materials.GetById;

namespace Recipes.Tests.Model;

public class MaterialsTests
{
    private readonly Materials _sut;

    public MaterialsTests()
    {
        _sut = new Materials(Provider.GetRequiredService<ILogger<Materials>>(),
            Provider.GetRequiredService<IMediator>(),
            Provider.GetRequiredService<IHttpFunctionExecutor>());
    }

    public async Task ShouldGetMaterials()
    {
        var req = new Mock<HttpRequest>();

        var result = await _sut.GetMaterials(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var materials = ((OkObjectResult)result).Value;
        materials.ShouldBeAssignableTo<IEnumerable<MaterialGetResponse>>();
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
        materialResult.ShouldBeAssignableTo<MaterialGetResponse>();
        material.Id.ShouldBe((materialResult as MaterialGetResponse)!.Id);
    }

    public async Task ShouldCreateMaterial()
    {
        var material = new MaterialCreateRequest();
        var req = CreateMockRequest(material);

        var result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<List<ValidationFailure>>();
        var validationFailures = resultObject as List<ValidationFailure>;
        validationFailures!.Count.ShouldBe(2);
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required("Image link"));
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialCreateRequest.Properties)));

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
        validationFailures.Select(x => x.ErrorMessage).ShouldContain(ValidationError.Required(nameof(MaterialCreateRequest.Properties)));

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
        material.Properties.Add(new() { LangId = Shared.Enums.Lang.Spanish, Description = "Descripcion", Name = "Nombre", Type = "Tipo" });
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<Guid>();

        req = new Mock<HttpRequest>();
        result = await _sut.GetMaterial(req.Object, (Guid)resultObject);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateMaterial()
    {
        var material = await CreateMaterial();

        var materialUpdate = new MaterialUpdateRequest()
        {
            Id = Guid.NewGuid(),
            Image = material.Image,
            Properties = material.Properties
        };
        var req = CreateMockRequest(materialUpdate);

        var result = await _sut.UpdateMaterial(req.Object);

        result.ShouldBeAssignableTo<NotFoundObjectResult>();

        materialUpdate.Id = material.Id;
        materialUpdate.Image = material.Image + "_updated.png";

        req = CreateMockRequest(materialUpdate);

        result = await _sut.UpdateMaterial(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var materialResult = ((OkObjectResult)result).Value;
        materialResult.ShouldBeAssignableTo<MaterialGetResponse>();
        (materialResult as MaterialGetResponse)!.Image.ShouldNotBe(material.Image);
        (materialResult as MaterialGetResponse)!.Image.ShouldBe(materialUpdate.Image);
    }
}
