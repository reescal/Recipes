using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Materials.Create;

public class MaterialCreateMappingProfile : Profile
{
    public MaterialCreateMappingProfile()
    {
        CreateMap<MaterialCreateRequest, Material>();
    }
}
