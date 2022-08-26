using Recipes.Shared.Enums;
using Recipes.Shared.Interfaces;

namespace Recipes.Shared.Models;

public class ComplexEntity : IComplexEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public HashSet<IEntityProperties> Properties { get; set; }

    public SimpleEntity ToSimpleEntity(Lang lang)
    {
        return new SimpleEntity()
        {
            Id = Id,
            Image = Image,
            Properties = Properties.Single(x => x.LangId == lang)
        };
    }

    public static explicit operator ComplexEntity(Ingredient i)
    {
        return new ComplexEntity()
        {
            Id = i.Id,
            Image = i.Image,
            Properties = i.Properties.Cast<IEntityProperties>().ToHashSet()
        };
    }

    public static explicit operator ComplexEntity(Material i)
    {
        return new ComplexEntity()
        {
            Id = i.Id,
            Image = i.Image,
            Properties = i.Properties.Cast<IEntityProperties>().ToHashSet()
        };
    }

    public static explicit operator ComplexEntity(Recipe i)
    {
        return new ComplexEntity()
        {
            Id = i.Id,
            Image = i.Image,
            Properties = i.Properties.Cast<IEntityProperties>().ToHashSet()
        };
    }
}

public class SimpleEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
    public IEntityProperties Properties { get; set; }

    public static SimpleEntity FromIngredient(Ingredient i, Lang lang)
    {
        return new SimpleEntity()
        {
            Id = i.Id,
            Image = i.Image,
            Properties = i.Properties.Single(x => x.LangId == lang)
        };
    }
    public override string ToString() => Properties.Name;
}