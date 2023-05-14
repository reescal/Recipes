using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Recipes.Create;

public class RecipeCreateMappingProfile : Profile
{
    public RecipeCreateMappingProfile()
    {
        CreateMap<RecipeCreateRequest, Recipe>();
    }
}
