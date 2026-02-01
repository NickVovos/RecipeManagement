using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Models
{
    /// <summary>
    /// Domain model for Step - inherits from BaseEntity
    /// </summary>
    public class Step : BaseEntity
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public List<Image> Images { get; set; } = new List<Image>();

        public Step() { }

        public Step(int order, string title, string description, int duration)
        {
            Order = order;
            Title = title;
            Description = description;
            Duration = duration;
        }

        // Information Expert: Step knows how to format its ingredients
        public string GetFormattedIngredients()
        {
            if (Ingredients == null || Ingredients.Count == 0)
                return "No ingredients";

            return string.Join(", ", Ingredients.Select(i => i.ToString()));
        }

        public override object Clone()
        {
            return new Step
            {
                Id = this.Id,
                Order = this.Order,
                Title = this.Title,
                Description = this.Description,
                Duration = this.Duration,
                Ingredients = this.Ingredients?.Select(i => (Ingredient)i.Clone()).ToList() ?? new List<Ingredient>(),
                Images = this.Images?.Select(i => (Image)i.Clone()).ToList() ?? new List<Image>()
            };
        }
    }
}
