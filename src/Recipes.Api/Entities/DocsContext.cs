using Recipes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Recipes.Api.Entities
{
    public class DocsContext : DbContext
    {
        public DocsContext(DbContextOptions<DocsContext> options) : base(options)
        {
        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Material> Materials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ingredient>()
                .HasPartitionKey(x => x.Id)
                .OwnsMany(x => x.Properties);
            modelBuilder.Entity<Material>()
                .HasPartitionKey(x => x.Id)
                .OwnsMany(x => x.Properties);
        }

    }
}
