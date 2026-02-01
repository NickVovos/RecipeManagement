using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecipeMan.Models;

namespace RecipeMan.Repository
{
    /// <summary>
    /// Repository implementation using API
    /// High Cohesion: Focuses only on data access via API
    /// </summary>
    public class RecipeApiRepository : IRecipeRepository
    {
        public RecipeApiRepository()
        {
        }

        public async Task<IReadOnlyList<Recipe>> GetAllAsync()
        {
            try
            {
                var apiRecipes = await RecipeApiClient.GetAllRecipesAsync();
                return apiRecipes.Select(MapFromApiData).ToList();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to load recipes from API", ex);
            }
        }

        public async Task<Recipe> GetByIdAsync(int id)
        {
            try
            {
                var apiRecipe = await RecipeApiClient.GetRecipeAsync(id);
                return MapFromApiData(apiRecipe);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to get recipe {id} from API", ex);
            }
        }

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            try
            {
                var id = await RecipeApiClient.CreateRecipeAsync(MapToApiData(recipe));
                recipe.Id = id;
                return recipe;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to create recipe via API", ex);
            }
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            try
            {
                await RecipeApiClient.UpdateRecipeAsync(recipe.Id, MapToApiData(recipe));
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to update recipe {recipe.Id} via API", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await RecipeApiClient.DeleteRecipeAsync(id);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to delete recipe {id} via API", ex);
            }
        }

        // Mapping methods
        private Recipe MapFromApiData(RecipeApiClient.RecipeData apiData)
        {
            return new Recipe
            {
                Id = apiData.Id,
                Name = apiData.Name,
                CategoryName = apiData.CategoryName,
                Difficulty = (RecipeDifficulty)apiData.Difficulty,
                Description = apiData.Description,
                Images = apiData.Images?.Select(i => new Image
                {
                    Name = i.Name,
                    Data = i.Data
                }).ToList() ?? new List<Image>(),
                Steps = apiData.Steps?.Select(s => new Step
                {
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description,
                    Duration = s.Duration,
                    Ingredients = s.Ingredients?.Select(ing => new Ingredient
                    {
                        Quantity = ing.Quantity,
                        Name = ing.Name
                    }).ToList() ?? new List<Ingredient>(),
                    Images = s.Images?.Select(i => new Image
                    {
                        Name = i.Name,
                        Data = i.Data
                    }).ToList() ?? new List<Image>()
                }).ToList() ?? new List<Step>()
            };
        }

        private RecipeApiClient.RecipeData MapToApiData(Recipe recipe)
        {
            return new RecipeApiClient.RecipeData
            {
                Id = recipe.Id,
                Name = recipe.Name,
                CategoryName = recipe.CategoryName,
                Difficulty = (CreateRecipeForm.Difficulty)recipe.Difficulty,
                Description = recipe.Description,
                Images = recipe.Images?.Select(i => new CreateRecipeForm.ImageData
                {
                    Name = i.Name,
                    Data = i.Data
                }).ToList() ?? new List<CreateRecipeForm.ImageData>(),
                Steps = recipe.Steps?.Select(s => new CreateRecipeForm.StepData
                {
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description,
                    Duration = s.Duration,
                    Ingredients = s.Ingredients?.Select(ing => new CreateRecipeForm.IngredientData
                    {
                        Quantity = ing.Quantity,
                        Name = ing.Name
                    }).ToList() ?? new List<CreateRecipeForm.IngredientData>(),
                    Images = s.Images?.Select(i => new CreateRecipeForm.ImageData
                    {
                        Name = i.Name,
                        Data = i.Data
                    }).ToList() ?? new List<CreateRecipeForm.ImageData>()
                }).ToList() ?? new List<CreateRecipeForm.StepData>()
            };
        }
    }

    public class RepositoryException : Exception
    {
        public RepositoryException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
