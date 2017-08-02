using System;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class SectionTile
    {
        public uint TileId;
        public int X;
        public int Y;

        public SectionTile()
        {

        }
        public SectionTile(uint tileId)
        {
            TileId = tileId;
        }

        public static NetSectionTile ToNetWorldTile(SectionTile tile)
        {
            return new NetSectionTile
            {
                TileId = tile.TileId,
                X = tile.X,
                Y = tile.Y
            };
        }

        public static SectionTile ConvertToSectionTile(NetSectionTile tile)
        {
            return new SectionTile{X = tile.X, Y = tile.Y, TileId = tile.TileId};
        }
    }
}