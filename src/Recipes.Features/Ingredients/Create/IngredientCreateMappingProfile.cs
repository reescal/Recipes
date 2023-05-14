using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Ingredients.Create;

public class IngredientCreateMappingProfile : Profile
{
    public IngredientCreateMappingProfile()
    {
        CreateMap<IngredientCreateRequest, Ingredient>();
    }
}
