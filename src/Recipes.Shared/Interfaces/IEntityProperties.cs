namespace Recipes.Shared.Interfaces;

public interface IEntityProperties
{
    public int LangId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}