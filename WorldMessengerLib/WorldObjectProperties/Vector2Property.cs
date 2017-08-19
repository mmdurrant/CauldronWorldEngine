using System;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class Vector2Property : IWorldObjectProperty
    {
        public PropertyType Type { get; } = PropertyType.Vector2;
        public WorldVector2 Value { get; set; }
    }
}