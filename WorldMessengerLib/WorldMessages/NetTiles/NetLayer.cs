using System;

namespace WorldMessengerLib.WorldMessages.NetTiles
{
    [Serializable]
    public class NetLayer
    {
        public int Layer;
        public NetSectionTile[] Tiles;
    }
}