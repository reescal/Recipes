using MediatR;
using Recipes.Data.Entities;

namespace Recipes.Features.Recipes.Create;

public class RecipeCreateRequest : IRequest<Guid>
{
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Preparation { get; set; }
    public string Type { get; set; }
    public List<string> Tags { get; set; }
    public List<IngredientRow> Ingredients { get; set; }
    public List<RecipeMaterial> Materials { get; set; }
}
