using System.Collections.Generic;

namespace Common.DTOs
{
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
