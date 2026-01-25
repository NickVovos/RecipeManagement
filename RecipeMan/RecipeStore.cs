using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeMan
{
    public static class RecipeStore
    {
        private static Dictionary<string, int> _recipeIdMap = new Dictionary<string, int>();

        public static async Task<IReadOnlyList<CreateRecipeForm.RecipeData>> GetAll()
        {

            try
            {
                var apiRecipes = await RecipeApiClient.GetAllRecipesAsync();
                _recipeIdMap.Clear();
                var recipeList = new List<CreateRecipeForm.RecipeData>();

                foreach (var apiRecipe in apiRecipes)
                {
                    var recipeData = new CreateRecipeForm.RecipeData
                    {
                        Name = apiRecipe.Name,
                        CategoryName = apiRecipe.CategoryName,
                        Difficulty = apiRecipe.Difficulty,
                        Description = apiRecipe.Description,
                        Images = apiRecipe.Images,
                        Steps = apiRecipe.Steps
                    };
                    _recipeIdMap[apiRecipe.Name] = apiRecipe.Id;
                    recipeList.Add(recipeData);
                }

                return recipeList;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load recipes from API: {ex.Message}\n\nMake sure the RecipeApi is running on https://localhost:44352",
                    "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<CreateRecipeForm.RecipeData>();
            }

        }

        public static async Task Add(CreateRecipeForm.RecipeData recipe)
        {
            try
            {
                var id = await RecipeApiClient.CreateRecipeAsync(recipe);
                _recipeIdMap[recipe.Name] = id;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to create recipe: {ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public static async Task Update(CreateRecipeForm.RecipeData existing, CreateRecipeForm.RecipeData updated)
        {
            try
            {
                int id;
                if (_recipeIdMap.TryGetValue(existing.Name, out id))
                {
                    await RecipeApiClient.UpdateRecipeAsync(id, updated);

                    if (existing.Name != updated.Name)
                    {
                        _recipeIdMap.Remove(existing.Name);
                    }
                    _recipeIdMap[updated.Name] = id;
                }
                else
                {
                    var allRecipes = await RecipeApiClient.GetAllRecipesAsync();
                    var apiRecipe = allRecipes.FirstOrDefault(r => r.Name == existing.Name);

                    if (apiRecipe == null)
                    {
                        MessageBox.Show("Recipe not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    await RecipeApiClient.UpdateRecipeAsync(apiRecipe.Id, updated);
                    _recipeIdMap[updated.Name] = apiRecipe.Id;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to update recipe: {ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public static async Task Remove(CreateRecipeForm.RecipeData recipe)
        {
            try
            {
                int id;
                if (_recipeIdMap.TryGetValue(recipe.Name, out id))
                {
                    await RecipeApiClient.DeleteRecipeAsync(id);
                    _recipeIdMap.Remove(recipe.Name);
                }
                else
                {
                    var allRecipes = await RecipeApiClient.GetAllRecipesAsync();
                    var apiRecipe = allRecipes.FirstOrDefault(r => r.Name == recipe.Name);

                    if (apiRecipe != null && apiRecipe.Id > 0)
                    {
                        await RecipeApiClient.DeleteRecipeAsync(apiRecipe.Id);
                    }
                    else
                    {
                        MessageBox.Show("Recipe not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to delete recipe: {ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public static CreateRecipeForm.RecipeData Clone(CreateRecipeForm.RecipeData recipe)
        {
            return new CreateRecipeForm.RecipeData
            {
                Name = recipe.Name,
                CategoryName = recipe.CategoryName,
                Difficulty = recipe.Difficulty,
                Description = recipe.Description,
                Images = recipe.Images?.Select(i => new CreateRecipeForm.ImageData { Name = i.Name, Data = i.Data }).ToList() ?? new List<CreateRecipeForm.ImageData>(),
                Steps = recipe.Steps?.Select(s => new CreateRecipeForm.StepData
                {
                    Order = s.Order,
                    Title = s.Title,
                    Description = s.Description,
                    Duration = s.Duration,
                    Ingredients = s.Ingredients?.Select(i => new CreateRecipeForm.IngredientData { Quantity = i.Quantity, Name = i.Name }).ToList() ?? new List<CreateRecipeForm.IngredientData>(),
                    Images = s.Images?.Select(i => new CreateRecipeForm.ImageData { Name = i.Name, Data = i.Data }).ToList() ?? new List<CreateRecipeForm.ImageData>()
                }).ToList() ?? new List<CreateRecipeForm.StepData>()
            };
        }

        public static async Task<int> GetRecipeId(string recipeName)
        {
            int id;
            if (_recipeIdMap.TryGetValue(recipeName, out id))
            {
                return id;
            }

            var allRecipes = await RecipeApiClient.GetAllRecipesAsync();
            var apiRecipe = allRecipes.FirstOrDefault(r => r.Name == recipeName);
            if (apiRecipe != null)
            {
                _recipeIdMap[recipeName] = apiRecipe.Id;
                return apiRecipe.Id;
            }


            return 0;
        }
    }
}
