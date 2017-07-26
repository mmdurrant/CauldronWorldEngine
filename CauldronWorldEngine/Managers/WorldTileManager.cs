using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.World;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace CauldronWorldEngine.Managers
{
    public class WorldTileManager
    {
        private Dictionary<string, WorldTile> WorldTiles = new Dictionary<string, WorldTile>();
        private Sender CollisionSender = new Sender(StaticChannelNames.CollisionChannel);

        public WorldTileManager()
        {
            CollisionSender.Connect();
        }

        public bool AddNewTile(WorldTile tile)
        {
            if (!WorldTiles.ContainsKey(tile.Name))
            {
                WorldTiles.Add(tile.Name, tile);
                CollisionSender.SendMessage(new AddCollisionEngineMessage
                {
                    Name = tile.Name,
                    Size = tile.Size
                });
                return true;
            }
            return false;
        }

        public bool RemoveTile(string tileName)
        {
            if (WorldTiles.Remove(tileName))
            {
                CollisionSender.SendMessage(new RemoveCollisionEngineMessage {Name = tileName});
                return true;
            }
            return false;
        }

        public List<WorldTile> GetTiles()
        {
            return WorldTiles.Values.ToList();
        }

        public NetWorldTile[] GetNetTiles()
        {
            var result = new NetWorldTile[WorldTiles.Count];
            var tileCount = 0;
            foreach (var tile in WorldTiles.Values)
            {
                var worldTile = new NetWorldTile
                {
                    Name = tile.Name,
                    Size = tile.Size,
                    Layers = new NetLayer[tile.WorldLayers.Count]
                };
                var layerCount = 0;
                foreach (var layer in tile.WorldLayers.Values)
                {
                    var netLayer = new NetLayer
                    {
                        Layer = layer.Layer,
                        Tiles = new NetSectionTile[layer.LayerTiles.Count]
                    };
                    var sectionCount = 0;
                    foreach (var section in layer.LayerTiles.Values)
                    {
                        netLayer.Tiles[sectionCount] =
                            new NetSectionTile {TileId = section.TileId, X = section.X, Y = section.Y};
                        sectionCount++;
                    }
                    worldTile.Layers[layerCount] = netLayer;
                    layerCount++;
                }
                result[tileCount] = worldTile;
                tileCount++;
            }
            return result;
        }

        public void ClearTiles()
        {
            WorldTiles.Clear();
            WorldTiles = new Dictionary<string, WorldTile>();
        }
    }
}