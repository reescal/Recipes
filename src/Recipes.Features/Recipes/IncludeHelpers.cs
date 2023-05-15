using Recipes.Data.Entities;
using Recipes.Data;
using Recipes.Features.Recipes.GetById;
using AutoMapper;
using Recipes.Features.Ingredients.GetById;
using Recipes.Features.Materials.GetById;

namespace Recipes.Features.Recipes;
public static class IncludeHelpers
{
    public static RecipeGetResponse IncludeIngredientsAndMaterials(this Recipe recipe, DocsContext _docsContext, IMapper _mapper)
    {
        var ingredientIds = recipe.Ingredients.Select(x => x.IngredientId);
        var ingredients = _docsContext.Ingredients.Where(x => ingredientIds.Contains(x.Id)).AsEnumerable();

        var materialIds = recipe.Materials.Select(x => x.MaterialId);
        var materials = _docsContext.Materials.Where(x => materialIds.Contains(x.Id)).ToList();

        var response = _mapper.Map<RecipeGetResponse>(recipe);

        response.Ingredients.ForEach(i => i.Ingredient = _mapper.Map<IngredientGetResponse>(ingredients.First(x => x.Id == i.Ingredient.Id)));
        response.Materials.ForEach(m => m = _mapper.Map<MaterialGetResponse>(materials.First(x => x.Id == m.Id)));
        return response;
    }

    public static List<RecipeGetResponse> IncludeIngredientsAndMaterials(this IEnumerable<Recipe> recipes, DocsContext _docsContext, IMapper _mapper)
    {
        var response = new List<RecipeGetResponse>();
        foreach (var recipe in recipes)
        {
            response.Add(recipe.IncludeIngredientsAndMaterials(_docsContext, _mapper));
        }
        return response;
    }
}
