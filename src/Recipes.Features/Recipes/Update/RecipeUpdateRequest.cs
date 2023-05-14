using MediatR;
using Recipes.Data.Entities;
using Recipes.Features.Recipes.GetById;
using Recipes.Shared.Models;

namespace Recipes.Features.Recipes.Update;
public class RecipeUpdateRequest : IRequest<RecipeGetResponse>
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public IndexHashSet<RecipeProperties> Properties { get; set; }
    public List<IngredientRow> Ingredients { get; set; }
    public List<RecipeMaterial> Materials { get; set; }
}