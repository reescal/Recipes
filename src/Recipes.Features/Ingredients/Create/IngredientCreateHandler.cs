using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;

namespace Recipes.Features.Ingredients.Create;

public class IngredientCreateHandler : IRequestHandler<IngredientCreateRequest, Guid>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public IngredientCreateHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<Guid> Handle(IngredientCreateRequest message, CancellationToken cancellationToken)
    {
        var ingredient = _mapper.Map<Data.Entities.Ingredient>(message);

        await _docsContext.Ingredients.AddAsync(ingredient, cancellationToken);
        await _docsContext.SaveChangesAsync(cancellationToken);

        return ingredient.Id;
    }
}
