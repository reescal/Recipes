using Recipes.Api.Entities;
using Recipes.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Recipes.Api.Services
{
    public class IngredientsService : IIngredientsService
    {
        private readonly IDbContextFactory<DocsContext> factory;

        public IngredientsService(IDbContextFactory<DocsContext> factory)
        {
            this.factory = factory;
        }

        public string Gets() => "Hello";

        public IEnumerable<Ingredient> Get()
        {
            var context = factory.CreateDbContext();
            var result = context.Ingredients.AsEnumerable();
            return result;
        }

        public async Task<Ingredient> GetAsync(Guid id)
        {
            var context = factory.CreateDbContext();
            var result = await context.Ingredients.FindAsync(id);
            return result;
        }

        public IEnumerable<string> GetNames(int _lang)
        {
            var context = factory.CreateDbContext();
            var result = context.Ingredients.Select(x => x.Properties
                                                    .Where(y => y.LangId == _lang)
                                                    .SingleOrDefault()
                                                    .Name);
            return result;
        }

        public async Task<string> InsertAsync(IngredientCreate ingredient)
        {
            var context = factory.CreateDbContext();

            var i = new Ingredient
            {
                Id = Guid.NewGuid(),
                Image = ingredient.Image,
                Properties = ingredient.Properties
            };
            context.Add(i);

            await context.SaveChangesAsync();

            return i.Id.ToString();
        }

        public async Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient)
        {
            var context = factory.CreateDbContext();

            var i = await context.Ingredients.FindAsync(id);

            if (i == null)
               return null;

            foreach(var prop in ingredient.Properties)
            {
                var iProp = i.Properties.SingleOrDefault(x => x.LangId == prop.LangId);
                if (iProp == null)
                    i.Properties.Add(prop);
                else
                    iProp = prop;
            }
            i.Image = ingredient.Image;

            await context.SaveChangesAsync();

            return i;
        }
    }

    public interface IIngredientsService
    {
        public IEnumerable<Ingredient> Get();
        public string Gets();
        public Task<Ingredient> GetAsync(Guid id);
        public IEnumerable<string> GetNames(int _lang);
        public Task<string> InsertAsync(IngredientCreate ingredient);
        public Task<Ingredient> UpdateAsync(Guid id, IngredientCreate ingredient);
    }
}
