using AutoMapper;
using Recipes.Data.Entities;

namespace Recipes.Features.Materials.GetById;

public class MaterialGetMappingProfile : Profile
{
    public MaterialGetMappingProfile()
    {
        CreateMap<Material, MaterialGetResponse>();
    }
}
