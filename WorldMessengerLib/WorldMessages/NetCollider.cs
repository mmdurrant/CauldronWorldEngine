using System;
using UnityEngine;

namespace WorldMessengerLib.WorldMessages
{
    [Serializable]
    public class NetCollider
    {
        public WorldVector2 Offset { get; set; }
        public WorldVector2 Size { get; set; }
    }
}