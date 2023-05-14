using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Materials.Update;
public class MaterialUpdateMappingProfile : Profile
{
    public MaterialUpdateMappingProfile()
    {
        CreateMap<MaterialUpdateRequest, Material>();
    }
}
