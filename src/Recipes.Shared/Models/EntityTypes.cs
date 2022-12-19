using Recipes.Shared.Enums;

namespace Recipes.Shared.Models;

public class EntityTypes
{
    public Lang LangId { get; set; }
    public HashSet<string> Types { get; set; }
}
