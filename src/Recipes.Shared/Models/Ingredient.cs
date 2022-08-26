using Recipes.Shared.Interfaces;
using FluentValidation;
using Recipes.Shared.Enums;

namespace Recipes.Shared.Models;

public class Ingredient : IngredientCreate, IEntity
{
    public Guid Id { get; set; }
}

public class IngredientCreate
{
    public string Image { get; set; }
    public IndexHashSet<IngredientProperties> Properties { get; set; }
}

public class IngredientProperties : IEntityProperties
{
    public Lang LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}

public class IngredientTypes
{
    public Lang LangId { get; set; }
    public HashSet<string> Types { get; set; }
}

public class IngredientValidator : AbstractValidator<IngredientCreate>
{
    public IngredientValidator()
    {
        RuleFor(p => p.Image).NotEmpty().WithMessage("Image link required");
        RuleFor(x => x.Properties).NotNull().WithMessage("Properties required");
        RuleForEach(x => x.Properties).SetValidator(new IngredientPropertiesValidator());
    }
}

public class IngredientPropertiesValidator : AbstractValidator<IngredientProperties>
{
    public IngredientPropertiesValidator()
    {
        RuleFor(p => p.LangId).IsInEnum().WithMessage("Invalid language");
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name required");
        RuleFor(p => p.Description).NotEmpty().WithMessage("Description required");
        RuleFor(p => p.Type).NotEmpty().WithMessage("Type required");
    }
}