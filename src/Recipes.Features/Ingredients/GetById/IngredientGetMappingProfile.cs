using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Ingredients.GetById;

public class IngredientGetMappingProfile : Profile
{
    public IngredientGetMappingProfile()
    {
        CreateMap<Ingredient, IngredientGetResponse>();
    }
}
