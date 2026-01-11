using System.Collections.Generic;
using System.Linq;

namespace RecipeMan
{
    public static class RecipeStore
    {
        private static readonly List<CreateRecipeForm.RecipeData> recipes = new List<CreateRecipeForm.RecipeData>();

        public static IReadOnlyList<CreateRecipeForm.RecipeData> All => recipes;

        public static void Add(CreateRecipeForm.RecipeData recipe)
        {
            recipes.Add(Clone(recipe));
        }

        public static void Update(CreateRecipeForm.RecipeData existing, CreateRecipeForm.RecipeData updated)
        {
            // Identify by name to ensure we update the correct stored instance
            var idx = recipes.FindIndex(r => ReferenceEquals(r, existing) || r.Name == existing.Name);
            if (idx >= 0)
            {
                recipes[idx] = Clone(updated);
            }
        }

        public static void Remove(CreateRecipeForm.RecipeData recipe)
        {
            var idx = recipes.FindIndex(r => ReferenceEquals(r, recipe) || r.Name == recipe.Name);
            if (idx >= 0) recipes.RemoveAt(idx);
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
