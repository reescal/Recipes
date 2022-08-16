using Recipes.Shared.Interfaces;

namespace Recipes.Shared.Models;

public class SimpleEntity
{
    public Guid Id { get; set; }
    public HashSet<IEntityProperties> Properties { get; set; }
}
