using Recipes.Api;
using static Recipes.Tests.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;
using Moq;
using Shouldly;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recipes.Features.GroceryList.Get;
using Recipes.Data.Entities;
using Recipes.Shared.Constants;
using Recipes.Features.GroceryList.AddGrocery;

namespace Recipes.Tests;

public class GroceryListsTests
{
    private readonly Api.GroceryLists _sut;

    public GroceryListsTests()
    {
        _sut = new Api.GroceryLists(Provider.GetRequiredService<ILogger<Api.GroceryLists>>(),
            Provider.GetRequiredService<IMediator>(),
            Provider.GetRequiredService<IHttpFunctionExecutor>());
    }

    public async Task ShouldGetGroceryList()
    {
        var req = new Mock<HttpRequest>();

        var result = await _sut.GetGroceryList(req.Object);

        result.ShouldBeAssignableTo<OkObjectResult>();
        var groceryList = ((OkObjectResult)result).Value;
        groceryList.ShouldBeAssignableTo<ApiResponse<GetGroceryListResponse>>();
    }

    public async Task ShouldAddGroceriesToGroceryList()
    {
        var groceries = new HashSet<Grocery> {};
        var req = CreateMockRequest(groceries);

        var result = await _sut.AddGrocery(req.Object);

        result.ShouldBeAssignableTo<BadRequestObjectResult>();
        var errors = ((BadRequestObjectResult)result).Value;
        errors.ShouldBeAssignableTo<ApiResponse>();
        ((ApiResponse)errors).Errors.Single().ShouldBe(ValidationError.Required(nameof(AddGroceryRequest.Grocery)));

        var ingredient = await CreateIngredient();
        groceries = new HashSet<Grocery>()
        {
            new Grocery()
            {
                IngredientId = ingredient.Id,
                Quantity = new()
                {
                    Value = 1,
                    Unit = "unit"
                }
            }
        };
        req = CreateMockRequest(groceries);

        result = await _sut.AddGrocery(req.Object);

        result.ShouldBeAssignableTo<NoContentResult>();
    }

    public async Task ShouldUpdateGroceryList()
    {
        var groceries = new HashSet<Grocery>();
        var req = CreateMockRequest(groceries);

        var result = await _sut.UpdateGroceryList(req.Object);

        result.ShouldBeAssignableTo<NoContentResult>();
    }
}