using MediatR;
using Recipes.Data.Entities;

namespace Recipes.Features.GroceryList.AddGrocery;
public class AddGroceryRequest : IRequest<Unit>
{
    public HashSet<Grocery> Grocery { get; set; }
}
