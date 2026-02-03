using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.DTOs;

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

        public static async Task<List<RecipeDto>> GetAllRecipesAsync()
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

        public static async Task<RecipeDto> GetRecipeAsync(int id)
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

        public static async Task<int> CreateRecipeAsync(RecipeDto recipe)
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

        public static async Task UpdateRecipeAsync(int id, RecipeDto recipe)
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

        private static RecipeDto MapToRecipeData(RecipeDto dto)
        {
            var recipe = new RecipeDto()
            {
                Id = dto.Id,
                Name = dto.Name,
                CategoryName = dto.CategoryName,
                Difficulty = (Difficulty)dto.Difficulty,
                Description = dto.Description,
                Images = new List<ImageDto>(),
                Steps = new List<StepDto>()
            };

            if (dto.Images != null)
            {
                foreach (var img in dto.Images)
                {
                    recipe.Images.Add(new ImageDto
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
                    var stepData = new StepDto
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
                            stepData.Ingredients.Add(new StepIngredientDto
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
                            stepData.Images.Add(new ImageDto
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

        private static RecipeDto MapToRecipeDto(RecipeDto recipe)
        {
            var dto = new RecipeDto
            {
                Name = recipe.Name,
                CategoryName = recipe.CategoryName,
                Difficulty = (Common.DTOs.Difficulty)recipe.Difficulty,
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

    }
}
