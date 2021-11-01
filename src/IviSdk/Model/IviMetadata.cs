using System.Collections.Generic;

namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public Dictionary<string, object> Properties { get; set; }

        public IviMetadata(string name, string description, string image, Dictionary<string, object> properties)
        {
            Name = name;
            Description = description;
            Image = image;
            Properties = properties;
        }
    }
}