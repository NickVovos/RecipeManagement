using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RecipeMan
{
    public class RecipeApiClient
    {
        private static readonly HttpClient _httpClient;
        private static readonly string _baseUrl;

        static RecipeApiClient()
        {
            _baseUrl = "https://localhost:44352/";
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public static async Task<List<RecipeData>> GetAllRecipesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/recipes").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dtos = JsonConvert.DeserializeObject<List<RecipeDto>>(json);
                return dtos.ConvertAll(MapToRecipeData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get recipes: {ex.Message}", ex);
            }
        }

        public static async Task<RecipeData> GetRecipeAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/recipes/{id}").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dto = JsonConvert.DeserializeObject<RecipeDto>(json);
                return MapToRecipeData(dto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get recipe: {ex.Message}", ex);
            }
        }

        public static async Task<int> CreateRecipeAsync(CreateRecipeForm.RecipeData recipe)
        {
            try
            {
                var dto = MapToRecipeDto(recipe);
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/recipes", content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var createdDto = JsonConvert.DeserializeObject<RecipeDto>(responseJson);
                return createdDto.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create recipe: {ex.Message}", ex);
            }
        }

        public static async Task UpdateRecipeAsync(int id, CreateRecipeForm.RecipeData recipe)
        {
            try
            {
                var dto = MapToRecipeDto(recipe);
                dto.Id = id;
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/recipes/{id}", content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update recipe: {ex.Message}", ex);
            }
        }

        public static async Task DeleteRecipeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/recipes/{id}").ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete recipe: {ex.Message}", ex);
            }
        }

        private static RecipeData MapToRecipeData(RecipeDto dto)
        {
            var recipe = new RecipeData
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryName = dto.CategoryName,
                Difficulty = (CreateRecipeForm.Difficulty)dto.Difficulty,
                Description = dto.Description,
                Images = new List<CreateRecipeForm.ImageData>(),
                Steps = new List<CreateRecipeForm.StepData>()
            };

            if (dto.Images != null)
            {
                foreach (var img in dto.Images)
                {
                    recipe.Images.Add(new CreateRecipeForm.ImageData
                    {
                        Name = img.Name,
                        Data = img.Data
                    });
                }
            }

            if (dto.Steps != null)
            {
                foreach (var step in dto.Steps)
                {
                    var stepData = new CreateRecipeForm.StepData
                    {
                        Order = step.Order,
                        Title = step.Title,
                        Description = step.Description,
                        Duration = step.Duration,
                        Ingredients = new List<CreateRecipeForm.IngredientData>(),
                        Images = new List<CreateRecipeForm.ImageData>()
                    };

                    if (step.Ingredients != null)
                    {
                        foreach (var ing in step.Ingredients)
                        {
                            stepData.Ingredients.Add(new CreateRecipeForm.IngredientData
                            {
                                Quantity = ing.Quantity,
                                Name = ing.Name
                            });
                        }
                    }

                    if (step.Images != null)
                    {
                        foreach (var img in step.Images)
                        {
                            stepData.Images.Add(new CreateRecipeForm.ImageData
                            {
                                Name = img.Name,
                                Data = img.Data
                            });
                        }
                    }

                    recipe.Steps.Add(stepData);
                }
            }

            return recipe;
        }

        private static RecipeDto MapToRecipeDto(CreateRecipeForm.RecipeData recipe)
        {
            var dto = new RecipeDto
            {
                Name = recipe.Name,
                CategoryName = recipe.CategoryName,
                Difficulty = (int)recipe.Difficulty,
                Description = recipe.Description,
                Images = new List<ImageDto>(),
                Steps = new List<StepDto>()
            };

            if (recipe.Images != null)
            {
                foreach (var img in recipe.Images)
                {
                    dto.Images.Add(new ImageDto
                    {
                        Name = img.Name,
                        Data = img.Data
                    });
                }
            }

            if (recipe.Steps != null)
            {
                foreach (var step in recipe.Steps)
                {
                    var stepDto = new StepDto
                    {
                        Order = step.Order,
                        Title = step.Title,
                        Description = step.Description,
                        Duration = step.Duration,
                        Ingredients = new List<StepIngredientDto>(),
                        Images = new List<ImageDto>()
                    };

                    if (step.Ingredients != null)
                    {
                        foreach (var ing in step.Ingredients)
                        {
                            stepDto.Ingredients.Add(new StepIngredientDto
                            {
                                Quantity = ing.Quantity,
                                Name = ing.Name
                            });
                        }
                    }

                    if (step.Images != null)
                    {
                        foreach (var img in step.Images)
                        {
                            stepDto.Images.Add(new ImageDto
                            {
                                Name = img.Name,
                                Data = img.Data
                            });
                        }
                    }

                    dto.Steps.Add(stepDto);
                }
            }

            return dto;
        }

        public class RecipeData : CreateRecipeForm.RecipeData
        {
            public int Id { get; set; }
        }

        public class RecipeDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string CategoryName { get; set; }
            public int Difficulty { get; set; }
            public string Description { get; set; }
            public List<ImageDto> Images { get; set; } = new List<ImageDto>();
            public List<StepDto> Steps { get; set; } = new List<StepDto>();
        }

        public class StepDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Order { get; set; }
            public int Duration { get; set; }
            public List<StepIngredientDto> Ingredients { get; set; } = new List<StepIngredientDto>();
            public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        }

        public class StepIngredientDto
        {
            public string Quantity { get; set; }
            public string Name { get; set; }
        }

        public class ImageDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public byte[] Data { get; set; }
        }
    }
}
