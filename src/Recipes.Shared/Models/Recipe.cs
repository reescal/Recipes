using FluentValidation;
using Recipes.Shared.Enums;
using Recipes.Shared.Interfaces;

namespace Recipes.Shared.Models;

public class Recipe : RecipeCreate, IEntity
{
    public Guid Id { get; set; }
}

public class RecipeCreate
{
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public IndexHashSet<RecipeProperties> Properties { get; set; }
    public List<IngredientRow> Ingredients { get; set; }
}

public class RecipeResponse : IEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public string Video { get; set; }
    public string Yield { get; set; }
    public int Time { get; set; }
    public HashSet<RecipeProperties> Properties { get; set; }
    public List<IngredientRowResponse> Ingredients { get; set; }
}

public class RecipeProperties : IEntityProperties
{
    public Lang LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public List<string> Tags { get; set; }
}

public class IngredientRow
{
    public Guid IngredientId { get; set; }
    public Quantity Quantity { get; set; }
    public string Preparation { get; set; }
    public bool IsOptional { get; set; }
}

public class IngredientRowResponse
{
    public Ingredient Ingredient { get; set; }
    public Quantity Quantity { get; set; }
    public string Preparation { get; set; }
    public bool IsOptional { get; set; }
}

public class Quantity
{
    public double Value { get; set; }
    public string Unit { get; set; }
}

public class RecipeValidator : AbstractValidator<RecipeCreate>
{
    public RecipeValidator()
    {
        RuleFor(p => p.Image).NotEmpty().WithMessage("Image link required");
        RuleFor(p => p.Yield).NotEmpty().WithMessage("Yield required");
        RuleFor(p => p.Time).NotEmpty().WithMessage("Time required");
        RuleFor(x => x.Properties).NotNull().WithMessage("Properties required");
        RuleForEach(x => x.Properties).SetValidator(new RecipePropertiesValidator());
        RuleForEach(x => x.Ingredients).NotNull().WithMessage("Ingredients required");
        RuleForEach(x => x.Ingredients).SetValidator(new IngredientRowValidator());
    }
}

public class RecipePropertiesValidator : AbstractValidator<RecipeProperties>
{
    public RecipePropertiesValidator()
    {
        RuleFor(p => p.LangId).IsInEnum().WithMessage("Invalid language");
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name required");
        RuleFor(p => p.Description).NotEmpty().WithMessage("Description required");
        RuleFor(p => p.Type).NotEmpty().WithMessage("Type required");
    }
}

public class IngredientRowValidator : AbstractValidator<IngredientRow>
{
    public IngredientRowValidator()
    {
        //RuleFor(p => p.IngredientId) DI context and check here???
        RuleFor(x => x.Quantity).NotNull().WithMessage("Quantity required");
        RuleFor(p => p.Quantity).SetValidator(new QuantityValidator());
        RuleFor(p => p.Preparation).NotNull().WithMessage("Preparation required");
    }
}

public class QuantityValidator : AbstractValidator<Quantity>
{
    public QuantityValidator()
    {
        RuleFor(p => p.Value).GreaterThan(0).WithMessage("Value required");
        RuleFor(p => p.Unit).NotEmpty().WithMessage("Unit required");
    }
}