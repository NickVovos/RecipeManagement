using System;

namespace Common.Models
{
    public class Ingredient : ICloneable
    {
        public string Quantity { get; set; }
        public string Name { get; set; }

        public Ingredient() { }

        public Ingredient(string quantity, string name)
        {
            Quantity = quantity;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Quantity} {Name}";
        }

        public object Clone()
        {
            return new Ingredient
            {
                Quantity = this.Quantity,
                Name = this.Name
            };
        }
    }
}
