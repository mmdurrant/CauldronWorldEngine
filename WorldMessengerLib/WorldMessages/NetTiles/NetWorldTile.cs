using System;
using UnityEngine;

namespace WorldMessengerLib.WorldMessages.NetTiles
{
    [Serializable]
    public class NetWorldTile
    {
        public string Name;
        public WorldVector2 Size;
        public WorldVector2 TopLeft;
        public NetLayer[] Layers;
    }
}