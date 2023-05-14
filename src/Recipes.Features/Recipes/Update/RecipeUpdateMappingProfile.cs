using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Recipes.Update;
public class RecipeUpdateMappingProfile : Profile
{
    public RecipeUpdateMappingProfile()
    {
        CreateMap<RecipeUpdateRequest, Recipe>();
    }
}
