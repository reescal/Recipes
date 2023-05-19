using FluentValidation;
using Recipes.Shared.Constants;
using static Recipes.Shared.Models.ValidatorHelpers.ValidatorHelpers;

namespace Recipes.Features.Ingredients.Create;
public  class IngredientCreateValidator : AbstractValidator<IngredientCreateRequest>
{
    public IngredientCreateValidator()
    {
        RuleFor(p => p.Image).NotEmpty()
                            .WithMessage(ValidationError.Required("Image link"));
        RuleFor(p => p.Image).Must(BeImageUri)
                            .When(p => !string.IsNullOrWhiteSpace(p.Image))
                            .WithMessage(ValidationError.Invalid("image link"));
        RuleFor(p => p.Name).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(IngredientCreateRequest.Name)));
        RuleFor(p => p.Name).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooShort(nameof(IngredientCreateRequest.Name)));
        RuleFor(p => p.Name).MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Name))
                            .WithMessage(ValidationError.TooLong(nameof(IngredientCreateRequest.Name)));
        RuleFor(p => p.Description).NotEmpty()
                                    .WithMessage(ValidationError.Required(nameof(IngredientCreateRequest.Description)));
        RuleFor(p => p.Type).NotEmpty()
                            .WithMessage(ValidationError.Required(nameof(IngredientCreateRequest.Type)));
        RuleFor(p => p.Type).MinimumLength(3)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooShort(nameof(IngredientCreateRequest.Type)));
        RuleFor(p => p.Type).MaximumLength(50)
                            .When(p => !string.IsNullOrWhiteSpace(p.Type))
                            .WithMessage(ValidationError.TooLong(nameof(IngredientCreateRequest.Type)));
        RuleFor(p => p.NutritionalInfo).NotEmpty()
                                        .WithMessage(ValidationError.Required("Nutritional info"));
        RuleFor(p => p.NutritionalInfo).Must(BeUri)
                                        .When(p => !string.IsNullOrWhiteSpace(p.NutritionalInfo))
                                        .WithMessage(ValidationError.Invalid("Nutritional info"));
    }
}