using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.DTOs;
using RecipeMan.Services;
using Common.Models;

namespace RecipeMan
{
    public static class RecipeStore
    {
        private static readonly IRecipeService _recipeService = ServiceFactory.GetRecipeService();

        public static async Task<IReadOnlyList<RecipeDto>> GetAll()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return recipes.Select(ConvertToFormData).ToList();
        }

        public static async Task Add(RecipeDto recipe)
        {
            var domainRecipe = ConvertToDomainModel(recipe);
            await _recipeService.CreateRecipeAsync(domainRecipe);
        }

        public static async Task Update(RecipeDto existing, RecipeDto updated)
        {
            int id = await _recipeService.GetRecipeIdByNameAsync(existing.Name);
            if (id == 0)
                throw new InvalidOperationException("Recipe not found");

            var domainRecipe = ConvertToDomainModel(updated);
            domainRecipe.Id = id;
            await _recipeService.UpdateRecipeAsync(domainRecipe);
        }

        public static async Task Remove(RecipeDto recipe)
        {
            int id = await _recipeService.GetRecipeIdByNameAsync(recipe.Name);
            if (id == 0)
                throw new InvalidOperationException("Recipe not found");

            await _recipeService.DeleteRecipeAsync(id);
        }

        public static RecipeDto Clone(RecipeDto recipe)
        {
            var domainRecipe = ConvertToDomainModel(recipe);
            var cloned = (Recipe)domainRecipe.Clone();
            return ConvertToFormData(cloned);
        }

        public static async Task<int> GetRecipeId(string recipeName)
        {
            return await _recipeService.GetRecipeIdByNameAsync(recipeName);
        }

        private static RecipeDto ConvertToFormData(Recipe recipe)
        {
            return new RecipeDto
            {
                Name = recipe.Name,
                CategoryName = recipe.CategoryName,
                Difficulty = (Difficulty)recipe.Difficulty,
                Description = recipe.Description,
                Images = recipe.Images?.Select(i => new ImageDto
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
                    Ingredients = s.Ingredients?.Select(ing => new StepIngredientDto()
                    {
                        Quantity = ing.Quantity,
                        Name = ing.Name
                    }).ToList() ?? new List<StepIngredientDto>(),
                    Images = s.Images?.Select(i => new ImageDto
                    {
                        Name = i.Name,
                        Data = i.Data
                    }).ToList() ?? new List<ImageDto>()
                }).ToList() ?? new List<StepDto>()
            };
        }

        private static Recipe ConvertToDomainModel(RecipeDto formData)
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
