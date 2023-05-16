using FluentValidation;
using Recipes.Data.Entities;
using Recipes.Features.Recipes.Create;
using Recipes.Shared.Constants;

namespace Recipes.Features.GroceryList.AddGrocery;
public class AddGroceryValidator : AbstractValidator<AddGroceryRequest>
{
    public AddGroceryValidator()
    {
        RuleFor(x => x.Grocery).NotEmpty()
                                .WithMessage(ValidationError.Required(nameof(AddGroceryRequest.Grocery)));
        RuleForEach(x => x.Grocery).SetValidator(new GroceryValidator());
    }
}

public class GroceryValidator : AbstractValidator<Grocery>
{
    public GroceryValidator()
    {
        RuleFor(x => x.IngredientId).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(Grocery.IngredientId)));
        RuleFor(x => x.Quantity).SetValidator(new QuantityValidator());
    }
}