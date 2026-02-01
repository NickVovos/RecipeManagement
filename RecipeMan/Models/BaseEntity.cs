using System;

namespace RecipeMan.Models
{
    /// <summary>
    /// Base class for all entities with ID
    /// Implements inheritance to share common properties
    /// </summary>
    public abstract class BaseEntity : ICloneable
    {
        public int Id { get; set; }

        public abstract object Clone();
    }
}
