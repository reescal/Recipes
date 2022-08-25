using Recipes.Shared.Interfaces;
using FluentValidation;

namespace Recipes.Shared.Models;

public class Ingredient : IngredientCreate, IEntity
{
    public Guid Id { get; set; }
}

public class IngredientCreate
{
    public string Image { get; set; }
    public HashSet<IngredientProperties> Properties { get; set; }
}

public class IngredientProperties : IEntityProperties
{
    public int LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}

public class IngredientTypes
{
    public int LangId { get; set; }
    public HashSet<string> Types { get; set; }
}

public class IngredientValidator : AbstractValidator<IngredientCreate>
{
    public IngredientValidator()
    {
        RuleFor(p => p.Image).NotEmpty().WithMessage("You must enter image");
    }
}

public class IngredientPropertiesValidator : AbstractValidator<IngredientProperties>
{
    public IngredientPropertiesValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("You must enter Name");
        RuleFor(p => p.Description).NotEmpty().WithMessage("You must enter Description");
        RuleFor(p => p.Type).NotEmpty().WithMessage("You must enter Type");
    }
}