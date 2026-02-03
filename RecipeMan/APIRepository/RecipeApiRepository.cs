using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DTOs;
using Common.Models;

namespace RecipeMan.Repository
{
    public class RecipeApiRepository : IRecipeApiRepository
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

        private Recipe MapFromApiData(RecipeDto apiData)
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

        private RecipeDto MapToApiData(Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                CategoryName = recipe.CategoryName,
                Difficulty = (Difficulty)recipe.Difficulty,
                Description = recipe.Description,
                Images = recipe.Images?
                    .Where(i => i.Data != null && i.Data.Length > 0)
                    .Select(i => new ImageDto
                    {
                        Name = i.Name,
                        Data = i.Data
                    }).ToList() ?? new List<ImageDto>(),
                Steps = recipe.Steps?.Select(s => new StepDto
                {
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description,
                    Duration = s.Duration,
                    Ingredients = s.Ingredients?.Select(ing => new StepIngredientDto
                    {
                        Quantity = ing.Quantity,
                        Name = ing.Name
                    }).ToList() ?? new List<StepIngredientDto>(),

                    Images = s.Images?
                        .Where(i => i.Data != null && i.Data.Length > 0)
                        .Select(i => new ImageDto
                        {
                            Name = i.Name,
                            Data = i.Data
                        }).ToList() ?? new List<ImageDto>()
                }).ToList() ?? new List<StepDto>()
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
