using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RecipeMan.Repository
{
    /// <summary>
    /// Repository interface - Polymorphism pattern
    /// Abstracts data access layer
    /// </summary>
    public interface IRecipeRepository
    {
        Task<IReadOnlyList<Recipe>> GetAllAsync();
        Task<Recipe> GetByIdAsync(int id);
        Task<Recipe> CreateAsync(Recipe recipe);
        Task UpdateAsync(Recipe recipe);
        Task DeleteAsync(int id);
    }
}
