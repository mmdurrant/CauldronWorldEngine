using System;
using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.World;

namespace WorldMessengerLib.WorldMessages
{
    [Serializable]
    public class WorldObjectType
    {
        private Dictionary<string, IWorldObjectProperty> Properties { get; set; } = new Dictionary<string, IWorldObjectProperty>();

        public string Type { get; set; }
        public bool IsData { get; set; }
        public bool AddProperty(string name, IWorldObjectProperty property)
        {
            if (!Properties.ContainsKey(name))
            {
                Properties.Add(name, property);
                return true;
            }
            return false;
        }

        public bool RemoveProperty(string name)
        {
            return Properties.Remove(name);
        }

        public IWorldObjectProperty GetProperty(string name)
        {
            return Properties.ContainsKey(name) ? Properties[name] : null;
        }

        public bool SetProperty(string name, IWorldObjectProperty property)
        {
            if (Properties.ContainsKey(name) && Properties[name].Type == property.Type)
            {
                Properties[name] = property;
                return true;
            }
            return false;
        }

        public Dictionary<string, IWorldObjectProperty> GetProperties()
        {
            return Properties;
        }
    }
}