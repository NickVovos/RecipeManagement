using Microsoft.Extensions.Configuration;
using RecipeApi.Data;
using RecipeApi.Models;
using System.Linq;

namespace RecipeApi.Seed
{
    public static class DatabaseSeeder
    {
        public static void Seed(IConfiguration config)
        {
            var repo = new SqlRepository(config);
            if (repo.GetAllRecipes().Any()) return;

            var recipe = new RecipeDto
            {
                Name = "Lasagna",
                CategoryName = "Pasta",
                Difficulty = Difficulty.Medium,
                Description = "Layered pasta with meat sauce and cheese.",
                Images = { new ImageDto { Name = "lasagna.jpg", Data = null } }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare sauce",
                Description = "Cook meat and tomato sauce.",
                Order = 1,
                Duration = 30,
                Ingredients = {
                    new StepIngredientDto { Name = "Ground beef", Quantity = "500g" },
                    new StepIngredientDto { Name = "Tomato sauce", Quantity = "400g" }
                },
                Images = { new ImageDto { Name = "sauce.jpg", Data = null } }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Layer and bake",
                Description = "Layer pasta, sauce, and cheese; bake.",
                Order = 2,
                Duration = 45,
                Ingredients = {
                    new StepIngredientDto { Name = "Lasagna sheets", Quantity = "12" },
                    new StepIngredientDto { Name = "Mozzarella", Quantity = "200g" }
                },
                Images = { new ImageDto { Name = "layer.jpg", Data = null } }
            });

            repo.CreateRecipe(recipe);
        }
    }
}
