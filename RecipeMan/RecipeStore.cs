using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecipeMan.Services;
using Common.Models;

namespace RecipeMan
{
    public static class RecipeStore
    {
        private static readonly IRecipeService _recipeService = ServiceFactory.GetRecipeService();

        public static async Task<IReadOnlyList<CreateRecipeForm.RecipeData>> GetAll()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return recipes.Select(ConvertToFormData).ToList();
        }

        public static async Task Add(CreateRecipeForm.RecipeData recipe)
        {
            var domainRecipe = ConvertToDomainModel(recipe);
            await _recipeService.CreateRecipeAsync(domainRecipe);
        }

        public static async Task Update(CreateRecipeForm.RecipeData existing, CreateRecipeForm.RecipeData updated)
        {
            int id = await _recipeService.GetRecipeIdByNameAsync(existing.Name);
            if (id == 0)
                throw new InvalidOperationException("Recipe not found");

            var domainRecipe = ConvertToDomainModel(updated);
            domainRecipe.Id = id;
            await _recipeService.UpdateRecipeAsync(domainRecipe);
        }

        public static async Task Remove(CreateRecipeForm.RecipeData recipe)
        {
            int id = await _recipeService.GetRecipeIdByNameAsync(recipe.Name);
            if (id == 0)
                throw new InvalidOperationException("Recipe not found");

            await _recipeService.DeleteRecipeAsync(id);
        }

        public static CreateRecipeForm.RecipeData Clone(CreateRecipeForm.RecipeData recipe)
        {
            var domainRecipe = ConvertToDomainModel(recipe);
            var cloned = (Recipe)domainRecipe.Clone();
            return ConvertToFormData(cloned);
        }

        public static async Task<int> GetRecipeId(string recipeName)
        {
            return await _recipeService.GetRecipeIdByNameAsync(recipeName);
        }

        private static CreateRecipeForm.RecipeData ConvertToFormData(Recipe recipe)
        {
            return new CreateRecipeForm.RecipeData
            {
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

        private static Recipe ConvertToDomainModel(CreateRecipeForm.RecipeData formData)
        {
            return new Recipe
            {
                Name = formData.Name,
                CategoryName = formData.CategoryName,
                Difficulty = (RecipeDifficulty)formData.Difficulty,
                Description = formData.Description,
                Images = formData.Images?.Select(i => new Image
                {
                    Name = i.Name,
                    Data = i.Data
                }).ToList() ?? new List<Image>(),
                Steps = formData.Steps?.Select(s => new Step
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
    }
}
