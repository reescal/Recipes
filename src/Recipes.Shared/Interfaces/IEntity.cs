namespace Recipes.Shared.Interfaces;

public interface IEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
}

public interface IComplexEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public HashSet<IEntityProperties> Properties { get; set; }
}