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
        public Dictionary<int, WorldLayer> WorldLayers;
        public WorldVector2 TopLeft;
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

        public static NetWorldTile ToNetWorldTile(WorldTile tile)
        {
            var layers = new NetLayer[tile.WorldLayers.Count];
            for (var i = 0; i < tile.WorldLayers.Count; i++)
            {
                layers[i] = WorldLayer.ToNetLayer(tile.WorldLayers[i]);
            }
            return new NetWorldTile {Layers = layers, Name = tile.Name, Size = tile.Size, TopLeft = tile.TopLeft};
        }

        public static WorldTile ConvertToWorldTile(NetWorldTile tile)
        {
            var worldTile = new WorldTile(tile.Name) {Size = tile.Size, TopLeft = tile.TopLeft};
            for (var i = 0; i < tile.Layers.Length; i++)
            {
                worldTile.WorldLayers.Add(i, WorldLayer.ConvertToWorldLayer(tile.Layers[i]));
            }
            return worldTile;
        }
    }
}