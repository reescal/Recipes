using AutoMapper;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.Ingredients.Create;

public class IngredientCreateMappingProfile : Profile
{
    public IngredientCreateMappingProfile()
    {
        CreateMap<IngredientCreateRequest, Ingredient>();
        CreateMap<IngredientCreateRequest, IngredientGetResponse>();
    }
}
