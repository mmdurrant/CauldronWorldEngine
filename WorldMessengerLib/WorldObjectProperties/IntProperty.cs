using System;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class IntProperty : IWorldObjectProperty
    {
        public PropertyType Type { get; } = PropertyType.Int;
        public int Value { get; set; }
    }
}