using System.Collections.Generic;

namespace RecipeApi.Models
{
    public enum Difficulty { Easy, Medium, Hard }

    public class ImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }

    public class StepIngredientDto
    {
        public string Quantity { get; set; }
        public string Name { get; set; }
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

    public class RecipeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public Difficulty Difficulty { get; set; }
        public string Description { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        public List<StepDto> Steps { get; set; } = new List<StepDto>();
    }
}
