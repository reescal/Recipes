using FluentValidation;
using Recipes.Shared.Constants;
using Recipes.Shared.Enums;
using Recipes.Shared.Interfaces;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;

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
        RuleFor(p => p.Image).NotEmpty().WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri).When(p => p.Image != null).WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Yield).NotEmpty().WithMessage(ValidationError.Required(nameof(RecipeCreate.Yield)));
        RuleFor(p => p.Time).InclusiveBetween(5, 2880).WithMessage(ValidationError.InclusiveBetween(nameof(RecipeCreate.Time), 5, 2880));
        RuleFor(x => x.Properties).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(RecipeCreate.Properties)));
        RuleForEach(x => x.Properties).SetValidator(new RecipePropertiesValidator());
        RuleFor(x => x.Ingredients).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(RecipeCreate.Ingredients)));
        RuleForEach(x => x.Ingredients).SetValidator(new IngredientRowValidator());
    }
}

public class RecipePropertiesValidator : AbstractValidator<RecipeProperties>
{
    public RecipePropertiesValidator()
    {
        RuleFor(p => p.LangId).IsInEnum().WithMessage(ValidationError.Invalid("Language"));
        RuleFor(p => p.Name).NotEmpty().WithMessage(ValidationError.Required(nameof(RecipeProperties.Name)));
        RuleFor(p => p.Name).MinimumLength(3).When(p => !string.IsNullOrEmpty(p.Name)).WithMessage(ValidationError.TooShort(nameof(RecipeProperties.Name)));
        RuleFor(p => p.Name).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Name)).WithMessage(ValidationError.TooLong(nameof(RecipeProperties.Name)));
        RuleFor(p => p.Description).NotEmpty().WithMessage(ValidationError.Required(nameof(RecipeProperties.Description)));
        RuleFor(p => p.Type).Must(p => !string.IsNullOrEmpty(p)).WithMessage(ValidationError.Required(nameof(RecipeProperties.Type)));
        RuleFor(p => p.Type).MinimumLength(3).When(p => !string.IsNullOrEmpty(p.Type)).WithMessage(ValidationError.TooShort(nameof(RecipeProperties.Type)));
        RuleFor(p => p.Type).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Type)).WithMessage(ValidationError.TooLong(nameof(RecipeProperties.Type)));
    }
}

public class IngredientRowValidator : AbstractValidator<IngredientRow>
{
    public IngredientRowValidator()
    {
        //RuleFor(p => p.IngredientId) DI context and check here instead of CheckIngredients method on RecipesService?
        RuleFor(x => x.Quantity).NotNull().WithMessage(ValidationError.Required(nameof(IngredientRow.Quantity)));
        RuleFor(p => p.Quantity).SetValidator(new QuantityValidator());
        RuleFor(p => p.Preparation).NotEmpty().WithMessage(ValidationError.Required(nameof(IngredientRow.Preparation)));
        RuleFor(p => p.Preparation).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Preparation)).WithMessage(ValidationError.TooLong(nameof(IngredientRow.Preparation)));
    }
}

public class QuantityValidator : AbstractValidator<Quantity>
{
    public QuantityValidator()
    {
        RuleFor(p => p.Value).GreaterThan(0).WithMessage(ValidationError.Required(nameof(Quantity.Value)));
        RuleFor(p => p.Unit).NotEmpty().WithMessage(ValidationError.Required(nameof(Quantity.Unit)));
        RuleFor(p => p.Unit).MaximumLength(10).WithMessage(ValidationError.TooLong(nameof(Quantity.Unit)));
    }
}