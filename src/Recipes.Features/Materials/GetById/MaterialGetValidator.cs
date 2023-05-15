using FluentValidation;
using Recipes.Shared.Constants;

namespace Recipes.Features.Materials.GetById;
public class MaterialGetValidator : AbstractValidator<MaterialGetRequest>
{
    public MaterialGetValidator()
    {
        RuleFor(x => x.Id).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(MaterialGetRequest.Id)));
    }
}
