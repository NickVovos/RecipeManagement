using System.Collections.Generic;

namespace Common.DTOs
{
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
}
