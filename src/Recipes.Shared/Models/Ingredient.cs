using Recipes.Shared.Interfaces;
using FluentValidation;
using Recipes.Shared.Enums;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;
using Recipes.Shared.Constants;

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

public class IngredientValidator : AbstractValidator<IngredientCreate>
{
    public IngredientValidator()
    {
        RuleFor(p => p.Image).NotEmpty().WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri).When(p => p.Image != null).WithMessage(ValidationError.Invalid("image link"));
        RuleFor(x => x.Properties).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(IngredientCreate.Properties)));
        RuleForEach(x => x.Properties).SetValidator(new IngredientPropertiesValidator());
    }
}

public class IngredientPropertiesValidator : AbstractValidator<IngredientProperties>
{
    public IngredientPropertiesValidator()
    {
        RuleFor(p => p.LangId).IsInEnum().WithMessage(ValidationError.Invalid("Language"));
        RuleFor(p => p.Name).NotEmpty().WithMessage(ValidationError.Required(nameof(IngredientProperties.Name)));
        RuleFor(p => p.Name).MinimumLength(3).When(p => !string.IsNullOrEmpty(p.Name)).WithMessage(ValidationError.TooShort(nameof(IngredientProperties.Name)));
        RuleFor(p => p.Name).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Name)).WithMessage(ValidationError.TooLong(nameof(IngredientProperties.Name)));
        RuleFor(p => p.Description).NotEmpty().WithMessage(ValidationError.Required(nameof(IngredientProperties.Description)));
        RuleFor(p => p.Type).Must(p => !string.IsNullOrEmpty(p)).WithMessage(ValidationError.Required(nameof(IngredientProperties.Type)));
        RuleFor(p => p.Type).MinimumLength(3).When(p => !string.IsNullOrEmpty(p.Type)).WithMessage(ValidationError.TooShort(nameof(IngredientProperties.Type)));
        RuleFor(p => p.Type).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Type)).WithMessage(ValidationError.TooLong(nameof(IngredientProperties.Type)));
    }
}