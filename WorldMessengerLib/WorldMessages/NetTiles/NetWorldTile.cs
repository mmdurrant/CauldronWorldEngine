using System;
using UnityEngine;

namespace WorldMessengerLib.WorldMessages.NetTiles
{
    [Serializable]
    public class NetWorldTile
    {
        public string Name;
        public WorldVector2 Size;
        public NetLayer[] Layers;
    }
}