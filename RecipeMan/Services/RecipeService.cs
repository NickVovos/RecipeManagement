using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using RecipeMan.Repository;

namespace RecipeMan.Services
{
    /// <summary>
    /// Service implementation - Controller GRASP pattern
    /// High Cohesion: Focuses only on business logic coordination
    /// Low Coupling: Depends on abstraction (IRecipeRepository)
    /// </summary>
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repository;

        public RecipeService(IRecipeRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<Recipe>> GetAllRecipesAsync()
        {
            var recipes = await _repository.GetAllAsync();
            
            return recipes;
        }

        public async Task<Recipe> GetRecipeByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            var validation = recipe.Validate();
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            recipe.ReorderSteps();

            var created = await _repository.CreateAsync(recipe);
            
            return created;
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            if (recipe == null)
                throw new ArgumentNullException(nameof(recipe));

            if (recipe.Id <= 0)
                throw new InvalidOperationException("Recipe must have a valid ID to update");

            var validation = recipe.Validate();
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.Message);

            recipe.ReorderSteps();

            await _repository.UpdateAsync(recipe);

        }

        public async Task DeleteRecipeAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid recipe ID", nameof(id));

            await _repository.DeleteAsync(id);
        }

        public async Task<int> GetRecipeIdByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            var recipes = await _repository.GetAllAsync();
            var recipe = recipes.FirstOrDefault(r => r.Name == name);
            
            if (recipe != null)
            {
                return recipe.Id;
            }

            return 0;
        }
    }
}
