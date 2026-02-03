using System;

namespace Common.Models
{
    public abstract class BaseEntity : ICloneable
    {
        public int Id { get; set; }

        public abstract object Clone();
    }
}
