using Recipes.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Recipes.Api.Entities
{
    public class DocsContext : DbContext
    {
        private readonly string _container;
        public DocsContext(DbContextOptions<DocsContext> options, string container) : base(options)
        {
            _container = container ?? nameof(DocsContext);
        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Recipe> Recipes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer(_container);
            modelBuilder.Entity<Ingredient>()
                .HasPartitionKey(x => x.Id)
                .OwnsMany(x => x.Properties);
            modelBuilder.Entity<Material>()
                .HasPartitionKey(x => x.Id)
                .OwnsMany(x => x.Properties);
            modelBuilder.Entity<Recipe>()
                .HasPartitionKey(x => x.Id)
                .OwnsMany(x => x.Properties);
            modelBuilder.Entity<Recipe>()
                .OwnsMany(x => x.Ingredients);
        }

    }
}
