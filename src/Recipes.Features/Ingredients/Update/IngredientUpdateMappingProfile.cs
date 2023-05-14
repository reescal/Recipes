using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Ingredients.Update;
public class IngredientUpdateMappingProfile : Profile
{
    public IngredientUpdateMappingProfile()
    {
        CreateMap<IngredientUpdateRequest, Ingredient>();
    }
}
