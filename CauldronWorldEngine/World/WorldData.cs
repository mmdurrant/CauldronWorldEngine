using System;
using System.Collections.Generic;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldData
    {
        public List<WorldTile> WorldTiles { get; set; }
        public DateTime Timestamp { get; set; }
    }
}