using System;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class BoolProperty : IWorldObjectProperty
    {
        public PropertyType Type { get; } = PropertyType.Bool;
        public bool Value { get; set; }
    }
}