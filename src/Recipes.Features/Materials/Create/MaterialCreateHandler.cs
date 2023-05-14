using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Recipes.Data;

namespace Recipes.Features.Materials.Create;

public class MaterialCreateHandler : IRequestHandler<MaterialCreateRequest, Guid>
{
    private readonly IMapper _mapper;
    private readonly DocsContext _docsContext;

    public MaterialCreateHandler(IMapper mapper, IDbContextFactory<DocsContext> factory)
    {
        _mapper = mapper;
        _docsContext = factory.CreateDbContext();
    }

    public async Task<Guid> Handle(MaterialCreateRequest message, CancellationToken cancellationToken)
    {
        var material = _mapper.Map<Data.Entities.Material>(message);

        await _docsContext.Materials.AddAsync(material, cancellationToken);
        await _docsContext.SaveChangesAsync(cancellationToken);

        return material.Id;
    }
}
