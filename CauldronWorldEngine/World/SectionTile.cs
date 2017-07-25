using System;

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
    }
}