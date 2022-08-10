using Recipes.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Recipes.Api.Entities
{
    public class DocsContext : DbContext
    {
        public DocsContext(DbContextOptions<DocsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

    }
}
