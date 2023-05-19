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
        RuleFor(p => p.Image).NotEmpty()
                                .WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeImageUri)
                                .When(p => !string.IsNullOrWhiteSpace(p.Image))
                                .WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Video).Must(BeUri)
                                .When(p => !string.IsNullOrWhiteSpace(p.Video))
                                .WithMessage(ValidationError.Invalid("video link"));
        RuleFor(p => p.Yield).NotEmpty()
                                .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Yield)));
        RuleFor(p => p.Time).InclusiveBetween(5, 2880)
                            .WithMessage(ValidationError.InclusiveBetween(nameof(RecipeCreateRequest.Time), 5, 2880));
        RuleFor(p => p.Name).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Name)));
        RuleFor(p => p.Name).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooShort(nameof(RecipeCreateRequest.Name)))
                            .MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooLong(nameof(RecipeCreateRequest.Name)));
        RuleFor(p => p.Description).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Description)));
        RuleFor(p => p.Preparation).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Description)));
        RuleFor(p => p.Type).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Type)));
        RuleFor(p => p.Type).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooShort(nameof(RecipeCreateRequest.Type)))
                            .MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooLong(nameof(RecipeCreateRequest.Type)));
        RuleFor(x => x.Ingredients).Must(p => p != null && p.Any())
                                    .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Ingredients)));
        RuleForEach(x => x.Ingredients).SetValidator(new IngredientRowValidator(_docsContext));
        RuleForEach(x => x.Materials).SetValidator(new RecipeMaterialValidator(_docsContext));
    }
}
