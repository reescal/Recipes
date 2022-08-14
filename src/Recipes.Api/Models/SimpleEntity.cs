using Recipes.Api.Interfaces;
using System;
using System.Collections.Generic;

namespace Recipes.Api.Models
{
    public class SimpleEntity
    {
        public Guid Id { get; set; }
        public HashSet<IEntityProperties> Properties { get; set; }
    }
}
