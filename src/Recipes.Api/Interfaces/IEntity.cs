using System;

namespace Recipes.Api.Interfaces;

public interface IEntity
{
    public Guid Id { get; set; }
    public string Image { get; set; }
}