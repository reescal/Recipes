using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;
using Recipes.Features.Ingredients.GetById;

namespace Recipes.Features.Ingredients.GetAll;

public class IngredientsGetAllHandler : IRequestHandler<IngredientsGetAllRequest, IEnumerable<IngredientGetResponse>>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public IngredientsGetAllHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public Task<IEnumerable<IngredientGetResponse>> Handle(IngredientsGetAllRequest request, CancellationToken cancellationToken)
    {
        var ingredients = _docsContext.Ingredients.AsEnumerable();

        var response = _mapper.Map<IEnumerable<IngredientGetResponse>>(ingredients);

        return Task.FromResult(response);
    }
}
