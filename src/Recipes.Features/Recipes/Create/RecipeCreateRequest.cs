using MediatR;
using Recipes.Data.Entities;
using Recipes.Shared.Models;

namespace Recipes.Features.Recipes.Create;

public class RecipeCreateRequest : IRequest<Guid>
{
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public IndexHashSet<RecipeProperties> Properties { get; set; }
    public List<IngredientRow> Ingredients { get; set; }
    public List<RecipeMaterial> Materials { get; set; }
}
