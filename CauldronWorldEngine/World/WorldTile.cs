using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMessengerLib.WorldMessages;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldTile
    {
        public string Name;
        public TilesetName Tileset;
        public Dictionary<int, WorldLayer> WorldLayers;
        public WorldVector2 Size;

        public WorldTile()
        {
            WorldLayers = new Dictionary<int, WorldLayer>();
        }
        public WorldTile(string name)
        {
            Name = name;
            WorldLayers = new Dictionary<int, WorldLayer>();
        }
    }
}