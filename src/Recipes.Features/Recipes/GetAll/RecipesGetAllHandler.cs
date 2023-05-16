using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetAll;

public class RecipesGetAllHandler : IRequestHandler<RecipesGetAllRequest, IEnumerable<RecipeGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipesGetAllHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public Task<IEnumerable<RecipeGetResponse>> Handle(RecipesGetAllRequest request, CancellationToken cancellationToken)
    {
        var recipes = _docsContext.Recipes.AsEnumerable();

        var response = recipes.IncludeIngredientsAndMaterials(_docsContext, _mapper);

        return Task.FromResult(response);
    }
}
