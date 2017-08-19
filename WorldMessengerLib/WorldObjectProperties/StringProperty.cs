using System;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class StringProperty : IWorldObjectProperty
    {
        public PropertyType Type { get; } = PropertyType.String;
        public string Value { get; set; }
    }
}