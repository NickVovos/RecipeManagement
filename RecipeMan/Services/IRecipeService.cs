using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace RecipeMan.Services
{
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
