using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Recipes.GetById;

namespace Recipes.Features.Recipes.GetAll;

public class RecipesGetAllHandler : IRequestHandler<RecipesGetAllRequest, List<RecipeGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipesGetAllHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<List<RecipeGetResponse>> Handle(RecipesGetAllRequest request, CancellationToken cancellationToken)
    {
        var recipes = await _docsContext.Recipes.ToListAsync();

        var response = _mapper.Map<List<RecipeGetResponse>>(recipes);

        return response;
    }
}
