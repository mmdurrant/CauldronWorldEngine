using System;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class FloatProperty : IWorldObjectProperty
    {
        public PropertyType Type { get; } = PropertyType.Float;
        public float Value { get; set; }
    }
}