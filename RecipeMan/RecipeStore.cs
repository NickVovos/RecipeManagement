using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecipeMan
{
    public static class RecipeStore
    {
        private static List<CreateRecipeForm.RecipeData> cachedRecipes = new List<CreateRecipeForm.RecipeData>();
        private static Dictionary<CreateRecipeForm.RecipeData, int> recipeIdMap = new Dictionary<CreateRecipeForm.RecipeData, int>();

        public static IReadOnlyList<CreateRecipeForm.RecipeData> All
        {
            get
            {
                RefreshCache();
                return cachedRecipes;
            }
        }

        private static void RefreshCache()
        {
            try
            {
                var apiRecipes = Task.Run(async () => await RecipeApiClient.GetAllRecipesAsync()).Result;
                cachedRecipes.Clear();
                recipeIdMap.Clear();

                foreach (var apiRecipe in apiRecipes)
                {
                    var recipe = new CreateRecipeForm.RecipeData
                    {
                        Name = apiRecipe.Name,
                        CategoryName = apiRecipe.CategoryName,
                        Difficulty = apiRecipe.Difficulty,
                        Description = apiRecipe.Description,
                        Images = apiRecipe.Images,
                        Steps = apiRecipe.Steps
                    };
                    cachedRecipes.Add(recipe);
                    recipeIdMap[recipe] = apiRecipe.Id;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to load recipes from API: {ex.Message}\n\nMake sure the RecipeApi is running on https://localhost:5001", 
                    "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Add(CreateRecipeForm.RecipeData recipe)
        {
            try
            {
                var id = Task.Run(async () => await RecipeApiClient.CreateRecipeAsync(recipe)).Result;
                var cloned = Clone(recipe);
                cachedRecipes.Add(cloned);
                recipeIdMap[cloned] = id;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to create recipe: {ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public static void Update(CreateRecipeForm.RecipeData existing, CreateRecipeForm.RecipeData updated)
        {
            try
            {
                var idx = cachedRecipes.FindIndex(r => ReferenceEquals(r, existing) || r.Name == existing.Name);
                if (idx < 0)
                {
                    MessageBox.Show("Recipe not found in cache", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int id;
                if (!recipeIdMap.TryGetValue(existing, out id))
                {
                    MessageBox.Show("Recipe ID not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Task.Run(async () => await RecipeApiClient.UpdateRecipeAsync(id, updated)).Wait();

                var cloned = Clone(updated);
                cachedRecipes[idx] = cloned;
                recipeIdMap.Remove(existing);
                recipeIdMap[cloned] = id;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to update recipe: {ex.Message}", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public static void Remove(CreateRecipeForm.RecipeData recipe)
        {
            try
            {
                int id;
                if (!recipeIdMap.TryGetValue(recipe, out id))
                {
                    var idx = cachedRecipes.FindIndex(r => r.Name == recipe.Name);
                    if (idx >= 0)
                    {
                        recipeIdMap.TryGetValue(cachedRecipes[idx], out id);
                    }
                }

                if (id > 0)
                {
                    Task.Run(async () => await RecipeApiClient.DeleteRecipeAsync(id)).Wait();
                }

                var index = cachedRecipes.FindIndex(r => ReferenceEquals(r, recipe) || r.Name == recipe.Name);
                if (index >= 0)
                {
                    var removed = cachedRecipes[index];
                    cachedRecipes.RemoveAt(index);
                    recipeIdMap.Remove(removed);
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
    }
}
