using FluentValidation;
using Recipes.Shared.Enums;
using Recipes.Shared.Interfaces;

namespace Recipes.Shared.Models;

public class Material : MaterialCreate, IEntity
{
    public Guid Id { get; set; }
}

public class MaterialCreate
{
    public string Image { get; set; }
    public IndexHashSet<MaterialProperties> Properties { get; set; }
}

public class MaterialProperties : IEntityProperties
{
    public Lang LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}

public class MaterialValidator : AbstractValidator<MaterialCreate>
{
    public MaterialValidator()
    {
        RuleFor(p => p.Image).NotEmpty().WithMessage("Image link required");
        RuleFor(x => x.Properties).NotNull().WithMessage("Properties required");
        RuleForEach(x => x.Properties).SetValidator(new MaterialPropertiesValidator());
    }
}

public class MaterialPropertiesValidator : AbstractValidator<MaterialProperties>
{
    public MaterialPropertiesValidator()
    {
        RuleFor(p => p.LangId).IsInEnum().WithMessage("Invalid language");
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name required");
        RuleFor(p => p.Description).NotEmpty().WithMessage("Description required");
        RuleFor(p => p.Type).NotEmpty().WithMessage("Type required");
    }
}
