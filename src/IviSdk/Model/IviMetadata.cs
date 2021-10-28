using System.Collections.Generic;

namespace Mythical.Game.IviSdkCSharp.Model
{
    public class IviMetadata
    {
        private string _name;
        private string _description;
        private string _image;
        private Dictionary<string, object> _properties;

        public IviMetadata(string name, string description, string image, Dictionary<string, object> properties)
        {
            _name = name;
            _description = description;
            _image = image;
            _properties = properties;
        }
    }
}