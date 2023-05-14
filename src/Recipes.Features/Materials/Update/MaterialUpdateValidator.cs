using FluentValidation;
using Recipes.Features.Materials.Create;
using Recipes.Shared.Constants;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;
using Recipes.Data.Entities;

namespace Recipes.Features.Materials.Update;
public class MaterialUpdateValidator : AbstractValidator<MaterialUpdateRequest>
{
    public MaterialUpdateValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationError.Required(nameof(MaterialUpdateRequest.Id)));
        RuleFor(p => p.Image).NotEmpty().WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri).When(p => p.Image != null).WithMessage(ValidationError.Invalid("image link"));
        RuleFor(x => x.Properties).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(MaterialUpdateRequest.Properties)));
        RuleForEach(x => x.Properties).SetValidator(new MaterialPropertiesValidator());
    }
}
