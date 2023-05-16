using MediatR;
using Recipes.Data.Entities;

namespace Recipes.Features.GroceryList.Update;
public class UpdateGroceryListRequest : IRequest<Unit>
{
    public HashSet<Grocery> Grocery { get; set; }
}
