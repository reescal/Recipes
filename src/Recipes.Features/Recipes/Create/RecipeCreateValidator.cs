using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Data.Entities;
using Recipes.Shared.Constants;
using Recipes.Shared.Models;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;
using static Recipes.Shared.Constants.Responses;

namespace Recipes.Features.Recipes.Create;
public class RecipeCreateValidator : AbstractValidator<RecipeCreateRequest>
{
    public RecipeCreateValidator(IDbContextFactory<DocsContext> factory)
    {
        var _docsContext = factory.CreateDbContext();
        RuleFor(p => p.Image).NotEmpty()
                                .WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeUri)
                                .When(p => !string.IsNullOrWhiteSpace(p.Image))
                                .WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Yield).NotEmpty()
                                .WithMessage(ValidationError.Required(nameof(RecipeCreateRequest.Yield)));
        RuleFor(p => p.Yield).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Yield))
                            .WithMessage(ValidationError.TooShort(nameof(RecipeCreateRequest.Yield)))
                            .MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Yield))
                            .WithMessage(ValidationError.TooLong(nameof(RecipeCreateRequest.Yield)));
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

public class IngredientRowValidator : AbstractValidator<IngredientRow>
{
    public IngredientRowValidator(DocsContext context)
    {
        RuleFor(p => p.IngredientId).Must(p => context.Ingredients.Find(p) != null)
                                    .WithMessage(p => NotFound(nameof(Ingredient), p.IngredientId));
        RuleFor(p => p.Quantity).NotNull()
                        .WithMessage(ValidationError.Required(nameof(Quantity)));
        RuleFor(p => p.Quantity).SetValidator(new QuantityValidator());
        RuleFor(p => p.Preparation).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(IngredientRow.Preparation)));
        RuleFor(p => p.Preparation).MaximumLength(50)
                                    .When(p => !string.IsNullOrEmpty(p.Preparation))
                                    .WithMessage(ValidationError.TooLong(nameof(IngredientRow.Preparation)));
    }
}

public class QuantityValidator : AbstractValidator<Quantity>
{
    public QuantityValidator()
    {
        RuleFor(p => p.Value).GreaterThan(0)
                                .WithMessage(ValidationError.Required(nameof(Quantity.Value)));
        RuleFor(p => p.Unit).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(Quantity.Unit)));
        RuleFor(p => p.Unit).MaximumLength(10)
                            .WithMessage(ValidationError.TooLong(nameof(Quantity.Unit)));
    }
}

public class RecipeMaterialValidator : AbstractValidator<RecipeMaterial>
{
    public RecipeMaterialValidator(DocsContext context)
    {
        RuleFor(p => p.MaterialId).Must(p => context.Materials.Find(p) != null)
                                    .WithMessage(p => NotFound(nameof(Material), p.MaterialId));
    }
}