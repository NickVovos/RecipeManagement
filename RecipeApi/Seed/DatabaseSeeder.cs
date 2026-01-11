using Microsoft.Extensions.Configuration;
using RecipeApi.Data;
using RecipeApi.Models;
using System;
using System.IO;
using System.Linq;

namespace RecipeApi.Seed
{
    public static class DatabaseSeeder
    {
        private static Random _random = new Random();
        private static string[] _imageFiles;
        private const string ImagePath = @"T:\test\New folder";

        public static void Seed(IConfiguration config)
        {
            var repo = new SqlRepository(config);
            if (repo.GetAllRecipes().Any()) return;

            // Load available image files
            if (Directory.Exists(ImagePath))
            {
                _imageFiles = Directory.GetFiles(ImagePath, "*.*")
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                               f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
            else
            {
                _imageFiles = new string[0];
            }

            // Create multiple recipes with varying complexity
            CreateLasagnaRecipe(repo);
            CreateChickenCurryRecipe(repo);
            CreateChocolateCakeRecipe(repo);
            CreateCaesarSaladRecipe(repo);
            CreateBeefStewRecipe(repo);
            CreateMargaritaPizzaRecipe(repo);
            CreateTiramisuRecipe(repo);
            CreateSpaghettiCarbonaraRecipe(repo);
            CreateVegetableStirFryRecipe(repo);
            CreateFrenchOnionSoupRecipe(repo);
        }

        private static byte[] LoadRandomImage()
        {
            if (_imageFiles == null || _imageFiles.Length == 0) return null;
            
            var randomFile = _imageFiles[_random.Next(_imageFiles.Length)];
            try
            {
                return File.ReadAllBytes(randomFile);
            }
            catch
            {
                return null;
            }
        }

        private static ImageDto CreateRandomImage()
        {
            var data = LoadRandomImage();
            return new ImageDto
            {
                Name = $"image_{Guid.NewGuid().ToString().Substring(0, 8)}.jpg",
                Data = data
            };
        }

        private static void CreateLasagnaRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Classic Lasagna",
                CategoryName = "Pasta",
                Difficulty = Difficulty.Medium,
                Description = "Traditional Italian layered pasta with rich meat sauce, creamy béchamel, and melted cheese. A hearty family favorite perfect for gatherings.",
                Images = { CreateRandomImage(), CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare the meat sauce",
                Description = "Brown the ground beef in a large skillet, drain excess fat, then add tomato sauce, crushed tomatoes, garlic, and Italian herbs. Simmer for 20-30 minutes.",
                Order = 1,
                Duration = 35,
                Ingredients = {
                    new StepIngredientDto { Name = "Ground beef", Quantity = "500g" },
                    new StepIngredientDto { Name = "Tomato sauce", Quantity = "400g" },
                    new StepIngredientDto { Name = "Crushed tomatoes", Quantity = "200g" },
                    new StepIngredientDto { Name = "Garlic cloves", Quantity = "3" },
                    new StepIngredientDto { Name = "Italian herbs", Quantity = "2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Make béchamel sauce",
                Description = "Melt butter in a saucepan, add flour to make a roux, gradually whisk in milk until smooth and thickened. Season with salt, pepper, and nutmeg.",
                Order = 2,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Butter", Quantity = "50g" },
                    new StepIngredientDto { Name = "Flour", Quantity = "50g" },
                    new StepIngredientDto { Name = "Milk", Quantity = "500ml" },
                    new StepIngredientDto { Name = "Nutmeg", Quantity = "1/4 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Assemble and layer",
                Description = "In a baking dish, layer meat sauce, lasagna sheets, béchamel, and cheese. Repeat layers 2-3 times, finishing with cheese on top.",
                Order = 3,
                Duration = 20,
                Ingredients = {
                    new StepIngredientDto { Name = "Lasagna sheets", Quantity = "12" },
                    new StepIngredientDto { Name = "Mozzarella cheese", Quantity = "300g" },
                    new StepIngredientDto { Name = "Parmesan cheese", Quantity = "100g" }
                },
                Images = { CreateRandomImage(), CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Bake",
                Description = "Cover with foil and bake at 180°C for 30 minutes, then remove foil and bake for another 15 minutes until golden and bubbly.",
                Order = 4,
                Duration = 45,
                Ingredients = { },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateChickenCurryRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Chicken Tikka Masala",
                CategoryName = "Indian",
                Difficulty = Difficulty.Medium,
                Description = "Aromatic Indian curry with tender chicken pieces in a creamy tomato-based sauce with warming spices. Serve with rice or naan bread.",
                Images = { CreateRandomImage(), CreateRandomImage(), CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Marinate chicken",
                Description = "Cut chicken into cubes and marinate in yogurt, lemon juice, and spices for at least 30 minutes or overnight for best results.",
                Order = 1,
                Duration = 40,
                Ingredients = {
                    new StepIngredientDto { Name = "Chicken breast", Quantity = "600g" },
                    new StepIngredientDto { Name = "Plain yogurt", Quantity = "150g" },
                    new StepIngredientDto { Name = "Lemon juice", Quantity = "2 tbsp" },
                    new StepIngredientDto { Name = "Garam masala", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Cumin", Quantity = "1 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Cook chicken",
                Description = "Grill or pan-fry the marinated chicken pieces until slightly charred and cooked through. Set aside.",
                Order = 2,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Cooking oil", Quantity = "2 tbsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare curry sauce",
                Description = "Sauté onions, ginger, and garlic until golden. Add tomato puree, spices, and cream. Simmer until thickened.",
                Order = 3,
                Duration = 25,
                Ingredients = {
                    new StepIngredientDto { Name = "Onions", Quantity = "2 large" },
                    new StepIngredientDto { Name = "Ginger", Quantity = "1 inch piece" },
                    new StepIngredientDto { Name = "Garlic cloves", Quantity = "4" },
                    new StepIngredientDto { Name = "Tomato puree", Quantity = "400g" },
                    new StepIngredientDto { Name = "Heavy cream", Quantity = "200ml" },
                    new StepIngredientDto { Name = "Paprika", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Turmeric", Quantity = "1/2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Combine and finish",
                Description = "Add the cooked chicken to the sauce, simmer for 10 minutes. Garnish with fresh cilantro before serving.",
                Order = 4,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Fresh cilantro", Quantity = "handful" }
                },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateChocolateCakeRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Rich Chocolate Cake",
                CategoryName = "Dessert",
                Difficulty = Difficulty.Hard,
                Description = "Decadent multi-layer chocolate cake with smooth chocolate ganache frosting. Perfect for special occasions and chocolate lovers.",
                Images = { CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Mix dry ingredients",
                Description = "Sift together flour, cocoa powder, baking powder, baking soda, and salt in a large bowl.",
                Order = 1,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "All-purpose flour", Quantity = "280g" },
                    new StepIngredientDto { Name = "Cocoa powder", Quantity = "75g" },
                    new StepIngredientDto { Name = "Baking powder", Quantity = "2 tsp" },
                    new StepIngredientDto { Name = "Baking soda", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Salt", Quantity = "1/2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Cream butter and sugar",
                Description = "Beat butter and sugar until light and fluffy, about 5 minutes. Add eggs one at a time, then vanilla extract.",
                Order = 2,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Butter", Quantity = "200g" },
                    new StepIngredientDto { Name = "Sugar", Quantity = "350g" },
                    new StepIngredientDto { Name = "Eggs", Quantity = "3 large" },
                    new StepIngredientDto { Name = "Vanilla extract", Quantity = "2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Combine and bake",
                Description = "Alternately add dry ingredients and milk to the butter mixture. Pour into prepared pans and bake at 175°C for 30-35 minutes.",
                Order = 3,
                Duration = 40,
                Ingredients = {
                    new StepIngredientDto { Name = "Milk", Quantity = "240ml" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Make chocolate ganache",
                Description = "Heat cream until simmering, pour over chopped chocolate. Stir until smooth and glossy. Let cool until spreadable.",
                Order = 4,
                Duration = 25,
                Ingredients = {
                    new StepIngredientDto { Name = "Dark chocolate", Quantity = "400g" },
                    new StepIngredientDto { Name = "Heavy cream", Quantity = "300ml" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Assemble and frost",
                Description = "Level the cakes, spread ganache between layers, then frost the entire cake. Refrigerate for 1 hour before serving.",
                Order = 5,
                Duration = 30,
                Ingredients = { },
                Images = { CreateRandomImage(), CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateCaesarSaladRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Caesar Salad",
                CategoryName = "Salad",
                Difficulty = Difficulty.Easy,
                Description = "Classic Caesar salad with crisp romaine lettuce, homemade dressing, crunchy croutons, and fresh parmesan shavings.",
                Images = { CreateRandomImage(), CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Make Caesar dressing",
                Description = "Whisk together garlic, anchovy paste, lemon juice, Dijon mustard, egg yolk, and Worcestershire sauce. Slowly drizzle in oil while whisking.",
                Order = 1,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Garlic cloves", Quantity = "2" },
                    new StepIngredientDto { Name = "Anchovy paste", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Lemon juice", Quantity = "3 tbsp" },
                    new StepIngredientDto { Name = "Dijon mustard", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Egg yolk", Quantity = "1" },
                    new StepIngredientDto { Name = "Worcestershire sauce", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Olive oil", Quantity = "120ml" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare croutons",
                Description = "Cut bread into cubes, toss with olive oil and garlic. Bake at 180°C for 10-15 minutes until golden and crispy.",
                Order = 2,
                Duration = 20,
                Ingredients = {
                    new StepIngredientDto { Name = "Baguette", Quantity = "1/2 loaf" },
                    new StepIngredientDto { Name = "Olive oil", Quantity = "3 tbsp" },
                    new StepIngredientDto { Name = "Garlic powder", Quantity = "1/2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Assemble salad",
                Description = "Tear romaine lettuce, toss with dressing, top with croutons and shaved parmesan. Season with black pepper and serve immediately.",
                Order = 3,
                Duration = 5,
                Ingredients = {
                    new StepIngredientDto { Name = "Romaine lettuce", Quantity = "2 heads" },
                    new StepIngredientDto { Name = "Parmesan cheese", Quantity = "50g" },
                    new StepIngredientDto { Name = "Black pepper", Quantity = "to taste" }
                },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateBeefStewRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Hearty Beef Stew",
                CategoryName = "Main Course",
                Difficulty = Difficulty.Medium,
                Description = "Comforting slow-cooked beef stew with tender chunks of beef, vegetables, and rich gravy. Perfect for cold winter days.",
                Images = { CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Brown the beef",
                Description = "Season beef chunks with salt and pepper. Brown in batches in a hot Dutch oven with oil until well-seared on all sides.",
                Order = 1,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Beef chuck", Quantity = "1kg" },
                    new StepIngredientDto { Name = "Vegetable oil", Quantity = "3 tbsp" },
                    new StepIngredientDto { Name = "Salt", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Black pepper", Quantity = "1/2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Sauté vegetables",
                Description = "In the same pot, sauté onions, carrots, and celery until softened. Add tomato paste and flour, cook for 2 minutes.",
                Order = 2,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Onions", Quantity = "2 large" },
                    new StepIngredientDto { Name = "Carrots", Quantity = "4 large" },
                    new StepIngredientDto { Name = "Celery stalks", Quantity = "3" },
                    new StepIngredientDto { Name = "Tomato paste", Quantity = "2 tbsp" },
                    new StepIngredientDto { Name = "Flour", Quantity = "2 tbsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Add liquid and simmer",
                Description = "Return beef to pot, add beef stock, red wine, bay leaves, and thyme. Bring to boil, then reduce heat and simmer covered.",
                Order = 3,
                Duration = 120,
                Ingredients = {
                    new StepIngredientDto { Name = "Beef stock", Quantity = "1L" },
                    new StepIngredientDto { Name = "Red wine", Quantity = "250ml" },
                    new StepIngredientDto { Name = "Bay leaves", Quantity = "2" },
                    new StepIngredientDto { Name = "Fresh thyme", Quantity = "4 sprigs" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Add potatoes and finish",
                Description = "Add cubed potatoes and peas in the last 30 minutes of cooking. Adjust seasoning and serve hot with crusty bread.",
                Order = 4,
                Duration = 35,
                Ingredients = {
                    new StepIngredientDto { Name = "Potatoes", Quantity = "500g" },
                    new StepIngredientDto { Name = "Frozen peas", Quantity = "150g" }
                },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateMargaritaPizzaRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Margherita Pizza",
                CategoryName = "Italian",
                Difficulty = Difficulty.Easy,
                Description = "Classic Italian pizza with simple but delicious toppings: tomato sauce, fresh mozzarella, basil, and olive oil on a crispy homemade crust.",
                Images = { CreateRandomImage(), CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Make pizza dough",
                Description = "Mix flour, yeast, salt, and warm water. Knead for 10 minutes until smooth and elastic. Let rise in a warm place for 1 hour.",
                Order = 1,
                Duration = 75,
                Ingredients = {
                    new StepIngredientDto { Name = "Bread flour", Quantity = "400g" },
                    new StepIngredientDto { Name = "Active dry yeast", Quantity = "7g" },
                    new StepIngredientDto { Name = "Salt", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Warm water", Quantity = "250ml" },
                    new StepIngredientDto { Name = "Olive oil", Quantity = "2 tbsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare toppings",
                Description = "Drain and slice fresh mozzarella. Make simple tomato sauce by blending canned tomatoes with salt and oregano. Pick fresh basil leaves.",
                Order = 2,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Fresh mozzarella", Quantity = "250g" },
                    new StepIngredientDto { Name = "Canned tomatoes", Quantity = "400g" },
                    new StepIngredientDto { Name = "Dried oregano", Quantity = "1 tsp" },
                    new StepIngredientDto { Name = "Fresh basil", Quantity = "1 bunch" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Shape and top",
                Description = "Roll out dough into a circle, spread tomato sauce, arrange mozzarella slices. Drizzle with olive oil.",
                Order = 3,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Extra virgin olive oil", Quantity = "2 tbsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Bake",
                Description = "Bake in a preheated oven at 250°C for 10-12 minutes until crust is golden and cheese is bubbling. Top with fresh basil leaves.",
                Order = 4,
                Duration = 12,
                Ingredients = { },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateTiramisuRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Tiramisu",
                CategoryName = "Dessert",
                Difficulty = Difficulty.Medium,
                Description = "Classic Italian dessert with layers of coffee-soaked ladyfingers and creamy mascarpone filling, dusted with cocoa powder.",
                Images = { CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare coffee mixture",
                Description = "Brew strong espresso coffee and let it cool. Mix with coffee liqueur if desired.",
                Order = 1,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Espresso coffee", Quantity = "300ml" },
                    new StepIngredientDto { Name = "Coffee liqueur", Quantity = "3 tbsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Make mascarpone cream",
                Description = "Beat egg yolks with sugar until pale and thick. Fold in mascarpone cheese. In a separate bowl, whip cream to soft peaks, then fold into mascarpone mixture.",
                Order = 2,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Egg yolks", Quantity = "6" },
                    new StepIngredientDto { Name = "Sugar", Quantity = "100g" },
                    new StepIngredientDto { Name = "Mascarpone cheese", Quantity = "500g" },
                    new StepIngredientDto { Name = "Heavy cream", Quantity = "200ml" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Assemble layers",
                Description = "Quickly dip ladyfingers in coffee, arrange in a dish, spread half the mascarpone cream. Repeat with another layer.",
                Order = 3,
                Duration = 20,
                Ingredients = {
                    new StepIngredientDto { Name = "Ladyfinger biscuits", Quantity = "300g" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Chill and dust",
                Description = "Cover and refrigerate for at least 4 hours or overnight. Before serving, dust generously with cocoa powder.",
                Order = 4,
                Duration = 245,
                Ingredients = {
                    new StepIngredientDto { Name = "Cocoa powder", Quantity = "for dusting" }
                },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateSpaghettiCarbonaraRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Spaghetti Carbonara",
                CategoryName = "Pasta",
                Difficulty = Difficulty.Easy,
                Description = "Authentic Roman pasta dish with eggs, pecorino cheese, guanciale, and black pepper. Creamy without using cream!",
                Images = { CreateRandomImage(), CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Cook pasta",
                Description = "Bring a large pot of salted water to boil. Cook spaghetti until al dente according to package instructions. Reserve 1 cup pasta water.",
                Order = 1,
                Duration = 12,
                Ingredients = {
                    new StepIngredientDto { Name = "Spaghetti", Quantity = "400g" },
                    new StepIngredientDto { Name = "Salt", Quantity = "for pasta water" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare egg mixture",
                Description = "Beat eggs with grated pecorino cheese and lots of freshly ground black pepper in a bowl.",
                Order = 2,
                Duration = 5,
                Ingredients = {
                    new StepIngredientDto { Name = "Eggs", Quantity = "4 large" },
                    new StepIngredientDto { Name = "Pecorino Romano cheese", Quantity = "100g" },
                    new StepIngredientDto { Name = "Black pepper", Quantity = "2 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Cook guanciale",
                Description = "Cut guanciale into small pieces and cook in a large pan until crispy and fat is rendered. Remove from heat.",
                Order = 3,
                Duration = 8,
                Ingredients = {
                    new StepIngredientDto { Name = "Guanciale", Quantity = "200g" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Combine and serve",
                Description = "Add hot pasta to the pan with guanciale, toss. Remove from heat, add egg mixture, toss vigorously. Add pasta water to create creamy sauce. Serve immediately with extra cheese.",
                Order = 4,
                Duration = 5,
                Ingredients = { },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateVegetableStirFryRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "Asian Vegetable Stir Fry",
                CategoryName = "Asian",
                Difficulty = Difficulty.Easy,
                Description = "Quick and healthy stir-fried vegetables with a savory Asian sauce. Perfect weeknight dinner served over rice or noodles.",
                Images = { CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Prepare vegetables",
                Description = "Wash and cut all vegetables into similar-sized pieces for even cooking. Keep them separate as they have different cooking times.",
                Order = 1,
                Duration = 15,
                Ingredients = {
                    new StepIngredientDto { Name = "Broccoli florets", Quantity = "200g" },
                    new StepIngredientDto { Name = "Bell peppers", Quantity = "2" },
                    new StepIngredientDto { Name = "Carrots", Quantity = "2" },
                    new StepIngredientDto { Name = "Snow peas", Quantity = "150g" },
                    new StepIngredientDto { Name = "Baby corn", Quantity = "100g" },
                    new StepIngredientDto { Name = "Mushrooms", Quantity = "200g" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Make stir fry sauce",
                Description = "Whisk together soy sauce, oyster sauce, sesame oil, ginger, garlic, and cornstarch in a small bowl.",
                Order = 2,
                Duration = 5,
                Ingredients = {
                    new StepIngredientDto { Name = "Soy sauce", Quantity = "3 tbsp" },
                    new StepIngredientDto { Name = "Oyster sauce", Quantity = "2 tbsp" },
                    new StepIngredientDto { Name = "Sesame oil", Quantity = "1 tbsp" },
                    new StepIngredientDto { Name = "Fresh ginger", Quantity = "1 inch piece" },
                    new StepIngredientDto { Name = "Garlic cloves", Quantity = "3" },
                    new StepIngredientDto { Name = "Cornstarch", Quantity = "1 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Stir fry vegetables",
                Description = "Heat wok or large skillet over high heat with oil. Add vegetables in order of cooking time (carrots first, then broccoli, peppers, finally snow peas and mushrooms). Stir fry for 5-7 minutes.",
                Order = 3,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Vegetable oil", Quantity = "3 tbsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Add sauce and finish",
                Description = "Pour sauce over vegetables, toss to coat. Cook for 2 minutes until sauce thickens. Garnish with sesame seeds and green onions.",
                Order = 4,
                Duration = 5,
                Ingredients = {
                    new StepIngredientDto { Name = "Sesame seeds", Quantity = "1 tbsp" },
                    new StepIngredientDto { Name = "Green onions", Quantity = "2" }
                },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }

        private static void CreateFrenchOnionSoupRecipe(SqlRepository repo)
        {
            var recipe = new RecipeDto
            {
                Name = "French Onion Soup",
                CategoryName = "Soup",
                Difficulty = Difficulty.Medium,
                Description = "Rich and flavorful soup with caramelized onions in beef broth, topped with crusty bread and melted Gruyère cheese.",
                Images = { CreateRandomImage(), CreateRandomImage() }
            };

            recipe.Steps.Add(new StepDto
            {
                Title = "Caramelize onions",
                Description = "Thinly slice onions. Cook slowly in butter over medium-low heat for 40-50 minutes, stirring occasionally, until deep golden brown and caramelized.",
                Order = 1,
                Duration = 55,
                Ingredients = {
                    new StepIngredientDto { Name = "Yellow onions", Quantity = "6 large" },
                    new StepIngredientDto { Name = "Butter", Quantity = "50g" },
                    new StepIngredientDto { Name = "Sugar", Quantity = "1 tsp" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Deglaze and add stock",
                Description = "Add flour to onions, cook for 1 minute. Deglaze with wine, scraping up brown bits. Add beef stock, thyme, bay leaf, and simmer for 30 minutes.",
                Order = 2,
                Duration = 35,
                Ingredients = {
                    new StepIngredientDto { Name = "All-purpose flour", Quantity = "2 tbsp" },
                    new StepIngredientDto { Name = "Dry white wine", Quantity = "150ml" },
                    new StepIngredientDto { Name = "Beef stock", Quantity = "1.5L" },
                    new StepIngredientDto { Name = "Fresh thyme", Quantity = "3 sprigs" },
                    new StepIngredientDto { Name = "Bay leaf", Quantity = "1" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Toast bread",
                Description = "Slice baguette and toast in oven until golden and crispy. Rub with garlic clove if desired.",
                Order = 3,
                Duration = 10,
                Ingredients = {
                    new StepIngredientDto { Name = "Baguette", Quantity = "1" },
                    new StepIngredientDto { Name = "Garlic clove", Quantity = "1" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Broil with cheese",
                Description = "Ladle soup into oven-safe bowls, top with toasted bread and grated Gruyère. Broil until cheese is melted and bubbling.",
                Order = 4,
                Duration = 8,
                Ingredients = {
                    new StepIngredientDto { Name = "Gruyère cheese", Quantity = "200g" }
                },
                Images = { CreateRandomImage() }
            });

            recipe.Steps.Add(new StepDto
            {
                Title = "Serve",
                Description = "Carefully remove hot bowls from oven. Let cool for a minute before serving. Garnish with fresh thyme if desired.",
                Order = 5,
                Duration = 2,
                Ingredients = { },
                Images = { CreateRandomImage() }
            });

            repo.CreateRecipe(recipe);
        }
    }
}
