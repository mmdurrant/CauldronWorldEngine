using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMessengerLib.WorldMessages;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldLayer
    {
        public int Layer;
        public TilesetName TilesetName;
        public Dictionary<WorldVector2, SectionTile> LayerTiles = new Dictionary<WorldVector2, SectionTile>();


        public static NetLayer ToNetLayer(WorldLayer layer)
        {
            var tiles = new NetSectionTile[layer.LayerTiles.Count];
            var tileCount = 0;
            foreach (var tile in layer.LayerTiles)
            {
                tiles[tileCount] = SectionTile.ToNetWorldTile(tile.Value);
            }
            return new NetLayer {Layer = layer.Layer, Tileset = layer.TilesetName, Tiles = tiles};
        }

        public static WorldLayer ConvertToWorldLayer(NetLayer layer)
        {
            var worldLayer = new WorldLayer();
            foreach (var tile in layer.Tiles)
            {
                worldLayer.LayerTiles.Add(new WorldVector2{ X = tile.X,Y =tile.Y }, SectionTile.ConvertToSectionTile(tile));
            }
            return worldLayer;
        }
    }
}