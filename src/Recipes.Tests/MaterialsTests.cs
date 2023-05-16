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
using Recipes.Features.Materials.Create;
using Recipes.Features.Materials.Update;
using Recipes.Features.Materials.GetById;

namespace Recipes.Tests;

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
        materials.ShouldBeAssignableTo<ApiResponse<IEnumerable<MaterialGetResponse>>>();
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
        materialResult.ShouldBeAssignableTo<ApiResponse<MaterialGetResponse>>();
        material.Id.ShouldBe((materialResult as ApiResponse<MaterialGetResponse>)!.Result.Id);
    }

    public async Task ShouldCreateMaterial()
    {
        var material = new MaterialCreateRequest();
        var req = CreateMockRequest(material);

        var result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        var validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(4);
        validationFailures.Errors.ShouldContain(ValidationError.Required("Image link"));

        material.Image = "http://invalid/link";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(4);
        validationFailures.Errors.ShouldContain(ValidationError.Invalid("image link"));

        material.Image = "https://valid/link.png";
        material.Name = string.Empty;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(MaterialCreateRequest.Name)));

        material.Name = "Aa";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(MaterialCreateRequest.Name)));

        material.Name = new string('a', 51);
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(3);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(MaterialCreateRequest.Name)));

        material.Name = "Valid";
        material.Description = string.Empty;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(2);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(MaterialCreateRequest.Description)));

        material.Description = "Valid";
        material.Type = string.Empty;
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.Required(nameof(MaterialCreateRequest.Type)));

        material.Type = "Aa";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.TooShort(nameof(MaterialCreateRequest.Type)));

        material.Type = new string('a', 51);
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        resultObject = ((BadRequestObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse>();
        validationFailures = resultObject as ApiResponse;
        validationFailures!.Errors.Count().ShouldBe(1);
        validationFailures.Errors.ShouldContain(ValidationError.TooLong(nameof(MaterialCreateRequest.Type)));

        material.Type = "Valid";
        req = CreateMockRequest(material);
        result = await _sut.CreateMaterial(req.Object);
        result.ShouldBeAssignableTo<OkObjectResult>();
        resultObject = ((OkObjectResult)result).Value;
        resultObject.ShouldBeAssignableTo<ApiResponse<Guid>>();

        req = new Mock<HttpRequest>();
        result = await _sut.GetMaterial(req.Object, ((ApiResponse<Guid>)resultObject).Result);
        result.ShouldNotBeAssignableTo<NotFoundObjectResult>();
    }

    public async Task ShouldUpdateMaterial()
    {
        var material = await CreateMaterial();

        var materialUpdate = new MaterialUpdateRequest()
        {
            Id = Guid.NewGuid(),
            Image = material.Image,
            Name = material.Name,
            Description = material.Description,
            Type = material.Type
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
        materialResult.ShouldBeAssignableTo<ApiResponse<MaterialGetResponse>>();
        (materialResult as ApiResponse<MaterialGetResponse>)!.Result.Image.ShouldNotBe(material.Image);
        (materialResult as ApiResponse<MaterialGetResponse>)!.Result.Image.ShouldBe(materialUpdate.Image);
    }
}
