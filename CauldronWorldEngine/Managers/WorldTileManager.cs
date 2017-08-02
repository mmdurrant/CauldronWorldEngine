using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.World;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages;
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

        public bool SetWorldTile(WorldTile tile)
        {
            if (WorldTiles.ContainsKey(tile.Name))
            {
                if (WorldTiles[tile.Name].Size.X != tile.Size.X || WorldTiles[tile.Name].Size.Y != tile.Size.Y)
                {
                    CollisionSender.SendMessage(new SetWorldTileSizeMessage
                    {
                        TileName = tile.Name,
                        Size = tile.Size
                    });
                }
                WorldTiles[tile.Name] = tile;
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool SetSectionTile(SectionTile tile, string world, int layer, WorldVector2 pos)
        {
            if (WorldTiles.ContainsKey(world))
            {
                var worldTile = WorldTiles[world];
                if (worldTile.WorldLayers.ContainsKey(layer))
                {
                    var worldLayer = worldTile.WorldLayers[layer];
                    if (worldLayer.LayerTiles.ContainsKey(pos))
                    {
                        worldLayer.LayerTiles[pos] = tile;
                    }
                    else
                    {
                        worldLayer.LayerTiles.Add(pos, tile);
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetLayer(string world, int layer, TilesetName tileset)
        {
            if (WorldTiles.ContainsKey(world))
            {
                var worldtile = WorldTiles[world];
                if (worldtile.WorldLayers.ContainsKey(layer))
                {
                    worldtile.WorldLayers[layer].TilesetName = tileset;
                }
                else
                {
                    worldtile.WorldLayers.Add(layer, new WorldLayer{Layer = layer, TilesetName = tileset, LayerTiles = new Dictionary<WorldVector2, SectionTile>()});
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<WorldTile> GetTiles()
        {
            return WorldTiles.Values.ToList();
        }

        public NetWorldTile[] GetNetTiles()
        {
            var result = new NetWorldTile[WorldTiles.Count];
            var tileCount = 0;
            foreach (var worldTile in WorldTiles.Values)
            {
                result[tileCount] = WorldTile.ToNetWorldTile(worldTile);
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