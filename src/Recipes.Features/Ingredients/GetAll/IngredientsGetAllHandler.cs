using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.Ingredients.GetAll;

public class IngredientsGetAllHandler : IRequestHandler<IngredientsGetAllRequest, List<IngredientGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public IngredientsGetAllHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<List<IngredientGetResponse>> Handle(IngredientsGetAllRequest request, CancellationToken cancellationToken)
    {
        var ingredients = await _docsContext.Ingredients.ToListAsync();

        var response = _mapper.Map<List<IngredientGetResponse>>(ingredients);

        return response;
    }
}
