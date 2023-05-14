using AutoMapper;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Materials.GetById;

namespace Recipes.Features.Recipes.GetById;

public class RecipeGetMappingProfile : Profile
{
    public RecipeGetMappingProfile()
    {
        CreateMap<Recipe, RecipeGetResponse>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));
        CreateMap<IngredientRow, IngredientRowResponse>()
            .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => new IngredientGetResponse { Id = src.IngredientId }));
        CreateMap<RecipeMaterial, MaterialGetResponse>()
            .ForMember(dest => dest.Id, src => src.MapFrom(x => x.MaterialId));
    }
}
