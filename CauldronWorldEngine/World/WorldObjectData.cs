using System;
using System.Collections.Generic;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldObjectData
    {
        public List<WorldObject> WorldObjects { get; set; }
        public List<WorldObjectType> WorldObjectTypes { get; set; }
    }
}