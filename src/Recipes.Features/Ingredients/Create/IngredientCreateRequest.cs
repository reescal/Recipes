using MediatR;

namespace Recipes.Features.Ingredients.Create;

public class IngredientCreateRequest : IRequest<Guid>
{
    public string Image { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string NutritionalInfo { get; set; }
}
