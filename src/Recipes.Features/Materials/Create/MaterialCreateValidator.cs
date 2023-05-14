using FluentValidation;
using Recipes.Shared.Constants;
using Recipes.Shared.Models;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;

namespace Recipes.Features.Materials.Create;
public class MaterialCreateValidator : AbstractValidator<MaterialCreateRequest>
{
    public MaterialCreateValidator()
    {
        RuleFor(p => p.Image).NotEmpty().WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri).When(p => p.Image != null).WithMessage(ValidationError.Invalid("image link"));
        RuleFor(x => x.Properties).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(MaterialCreateRequest.Properties)));
        RuleForEach(x => x.Properties).SetValidator(new MaterialPropertiesValidator());
    }
}

public class MaterialPropertiesValidator : AbstractValidator<MaterialProperties>
{
    public MaterialPropertiesValidator()
    {
        RuleFor(p => p.LangId).IsInEnum().WithMessage(ValidationError.Invalid("Language"));
        RuleFor(p => p.Name).NotEmpty().WithMessage(ValidationError.Required(nameof(MaterialProperties.Name)));
        RuleFor(p => p.Name).MinimumLength(3).When(p => !string.IsNullOrEmpty(p.Name)).WithMessage(ValidationError.TooShort(nameof(MaterialProperties.Name)));
        RuleFor(p => p.Name).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Name)).WithMessage(ValidationError.TooLong(nameof(MaterialProperties.Name)));
        RuleFor(p => p.Description).NotEmpty().WithMessage(ValidationError.Required(nameof(MaterialProperties.Description)));
        RuleFor(p => p.Type).Must(p => !string.IsNullOrEmpty(p)).WithMessage(ValidationError.Required(nameof(MaterialProperties.Type)));
        RuleFor(p => p.Type).MinimumLength(3).When(p => !string.IsNullOrEmpty(p.Type)).WithMessage(ValidationError.TooShort(nameof(MaterialProperties.Type)));
        RuleFor(p => p.Type).MaximumLength(50).When(p => !string.IsNullOrEmpty(p.Type)).WithMessage(ValidationError.TooLong(nameof(MaterialProperties.Type)));
    }
}