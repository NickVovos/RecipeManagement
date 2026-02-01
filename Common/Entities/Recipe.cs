using System;
using System.Collections.Generic;
using System.Linq;
using Common.Validation;

namespace Common.Models
{
    /// <summary>
    /// Domain model for Recipe - Information Expert pattern
    /// Contains logic for manipulating its own data
    /// </summary>
    public class Recipe : BaseEntity
    {
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public RecipeDifficulty Difficulty { get; set; }
        public string Description { get; set; }
        public List<Step> Steps { get; set; } = new List<Step>();
        public List<Image> Images { get; set; } = new List<Image>();

        public Recipe() { }

        public Recipe(string name, string category, RecipeDifficulty difficulty, string description)
        {
            Name = name;
            CategoryName = category;
            Difficulty = difficulty;
            Description = description;
        }

        public int CalculateTotalDuration()
        {
            return Steps?.Sum(s => s.Duration) ?? 0;
        }

        public ValidationResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Name))
                return ValidationResult.Failure("Recipe name is required");

            if (Steps == null || Steps.Count == 0)
                return ValidationResult.Warning("Recipe has no steps");

            return ValidationResult.Success();
        }

        public void ReorderSteps()
        {
            if (Steps != null)
            {
                for (int i = 0; i < Steps.Count; i++)
                {
                    Steps[i].Order = i + 1;
                }
            }
        }

        public override object Clone()
        {
            return new Recipe
            {
                Id = this.Id,
                Name = this.Name,
                CategoryName = this.CategoryName,
                Difficulty = this.Difficulty,
                Description = this.Description,
                Images = this.Images?.Select(i => (Image)i.Clone()).ToList() ?? new List<Image>(),
                Steps = this.Steps?.Select(s => (Step)s.Clone()).ToList() ?? new List<Step>()
            };
        }
    }
}
