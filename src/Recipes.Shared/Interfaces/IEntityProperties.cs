using Recipes.Shared.Enums;

namespace Recipes.Shared.Interfaces;

public interface IEntityProperties
{
    public Lang LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}