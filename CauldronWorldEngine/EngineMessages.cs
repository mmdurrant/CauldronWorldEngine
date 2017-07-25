using UnityEngine;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine
{
    public class ObjectPositionMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Position;
        public string ColliderId { get; set; }
        public string WorldTile { get; set; }
        public WorldVector2 Position { get; set; }
    }

    public class ObjectWorldTileMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.ObjectWorldTile;
        public string ColliderId { get; set; }
        public string WorldTile { get; set; }
    }

    public class AddCollisionEngineMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AddCollisionEngine;
        public string Name { get; set; }
        public WorldVector2 Size { get; set; }
    }

    public class RemoveCollisionEngineMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.RemoveCollisionEngine;
        public string Name { get; set; }
    }
}