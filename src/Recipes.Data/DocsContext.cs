using Microsoft.EntityFrameworkCore;
using Recipes.Data.Entities;

namespace Recipes.Data;

public class DocsContext : DbContext
{
    private readonly string _container;
    public DocsContext(DbContextOptions<DocsContext> options, CosmosConfig config) : base(options)
    {
        _container = config.ContainerName ?? nameof(DocsContext);
    }

    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<GroceryList> GroceryList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer(_container);
        modelBuilder.Entity<Ingredient>()
            .HasPartitionKey(x => x.Id);
        modelBuilder.Entity<Material>()
            .HasPartitionKey(x => x.Id);
        modelBuilder.Entity<Recipe>()
            .HasPartitionKey(x => x.Id)
            .OwnsMany(x => x.Ingredients);
        modelBuilder.Entity<Recipe>()
            .OwnsMany(x => x.Materials);
        modelBuilder.Entity<GroceryList>()
            .HasPartitionKey(x => x.Id)
            .OwnsMany(x => x.Grocery);
    }
}
