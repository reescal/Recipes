using FluentValidation;
using Recipes.Features.GroceryList.AddGrocery;

namespace Recipes.Features.GroceryList.Update;
public class UpdateGroceryListValidator : AbstractValidator<UpdateGroceryListRequest>
{
    public UpdateGroceryListValidator()
    {
        RuleForEach(x => x.Grocery).SetValidator(new GroceryValidator());
    }
}
