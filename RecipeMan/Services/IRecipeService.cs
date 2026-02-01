using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeMan.Models;

namespace RecipeMan.Services
{
    /// <summary>
    /// Service interface - Polymorphism GRASP pattern
    /// Allows different implementations (API, local DB, mock, etc.)
    /// Controller pattern - coordinates recipe operations
    /// </summary>
    public interface IRecipeService
    {
        Task<IReadOnlyList<Recipe>> GetAllRecipesAsync();
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(int id);
        Task<int> GetRecipeIdByNameAsync(string name);
    }
}
