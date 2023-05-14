using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;

namespace Recipes.Features.Recipes.Create;

public class RecipeCreateHandler : IRequestHandler<RecipeCreateRequest, Guid>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public RecipeCreateHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<Guid> Handle(RecipeCreateRequest message, CancellationToken cancellationToken)
    {
        var recipe = _mapper.Map<Data.Entities.Recipe>(message);

        await _docsContext.Recipes.AddAsync(recipe, cancellationToken);
        await _docsContext.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }
}
