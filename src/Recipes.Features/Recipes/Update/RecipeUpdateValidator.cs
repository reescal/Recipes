using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Shared.Constants;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;
using Recipes.Features.Recipes.Create;

namespace Recipes.Features.Recipes.Update;
public class RecipeUpdateValidator : AbstractValidator<RecipeUpdateRequest>
{
    public RecipeUpdateValidator(IDbContextFactory<DocsContext> factory)
    {
        var _docsContext = factory.CreateDbContext();
        RuleFor(x => x.Id).NotEmpty().WithMessage(ValidationError.Required(nameof(RecipeUpdateRequest.Id)));
        RuleFor(p => p.Image).NotEmpty().WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri).When(p => p.Image != null).WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Yield).NotEmpty().WithMessage(ValidationError.Required(nameof(RecipeUpdateRequest.Yield)));
        RuleFor(p => p.Time).InclusiveBetween(5, 2880).WithMessage(ValidationError.InclusiveBetween(nameof(RecipeUpdateRequest.Time), 5, 2880));
        RuleFor(x => x.Properties).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(RecipeUpdateRequest.Properties)));
        RuleForEach(x => x.Properties).SetValidator(new RecipePropertiesValidator());
        RuleFor(x => x.Ingredients).Must(p => p != null && p.Any()).WithMessage(ValidationError.Required(nameof(RecipeUpdateRequest.Ingredients)));
        RuleForEach(x => x.Ingredients).SetValidator(new IngredientRowValidator(_docsContext));
        RuleForEach(x => x.Materials).SetValidator(new RecipeMaterialValidator(_docsContext));
    }
}
