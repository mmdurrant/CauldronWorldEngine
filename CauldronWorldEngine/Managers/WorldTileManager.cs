using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.World;
using WorldMessengerLib;

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

        public void ClearTiles()
        {
            WorldTiles.Clear();
            WorldTiles = new Dictionary<string, WorldTile>();
        }
    }
}