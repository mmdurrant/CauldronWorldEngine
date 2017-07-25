using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldLayer
    {
        public int Layer;
        public Dictionary<WorldVector2, SectionTile> LayerTiles = new Dictionary<WorldVector2, SectionTile>();
    }
}