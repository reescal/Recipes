using AutoMapper;
using Recipes.Data.Entities;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.GroceryList.Get;
public class GetGroceryListMappingProfile : Profile
{
    public GetGroceryListMappingProfile()
    {
        CreateMap<Data.Entities.GroceryList, GetGroceryListResponse>();
        CreateMap<Grocery, GroceryResponse>()
            .ForMember(dest => dest.Ingredient, opt => opt.MapFrom(src => new IngredientGetResponse { Id = src.IngredientId }));
    }
}
