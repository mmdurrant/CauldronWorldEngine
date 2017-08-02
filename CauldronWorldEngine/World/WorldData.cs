using System;
using System.Collections.Generic;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldData
    {
        public List<WorldTile> WorldTiles { get; set; }
        public DateTime Timestamp { get; set; }
    }
}