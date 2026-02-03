using System;

namespace Common.Models
{
    public class Image : BaseEntity
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }

        public Image() { }

        public Image(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public int GetSizeInBytes()
        {
            return Data?.Length ?? 0;
        }

        public string GetFormattedSize()
        {
            int bytes = GetSizeInBytes();
            if (bytes < 1024)
                return $"{bytes} bytes";
            if (bytes < 1024 * 1024)
                return $"{bytes / 1024} KB";
            return $"{bytes / (1024 * 1024)} MB";
        }

        public override object Clone()
        {
            return new Image
            {
                Id = this.Id,
                Name = this.Name,
                Data = this.Data != null ? (byte[])this.Data.Clone() : null
            };
        }
    }
}
