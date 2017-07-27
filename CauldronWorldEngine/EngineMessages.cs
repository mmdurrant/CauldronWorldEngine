using System;
using UnityEngine;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine
{
    [Serializable]
    public class ObjectPositionMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Position;
        public string ColliderId { get; set; }
        public string WorldTile { get; set; }
        public WorldVector2 Position { get; set; }
    }

    [Serializable]
    public class ObjectWorldTileMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.ObjectWorldTile;
        public string ColliderId { get; set; }
        public string WorldTile { get; set; }
    }

    [Serializable]
    public class AddCollisionEngineMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AddCollisionEngine;
        public string Name { get; set; }
        public WorldVector2 Size { get; set; }
    }

    [Serializable]
    public class RemoveCollisionEngineMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.RemoveCollisionEngine;
        public string Name { get; set; }
    }
}