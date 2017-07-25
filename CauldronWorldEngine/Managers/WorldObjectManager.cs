using System.Collections.Generic;
using UnityEngine;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.Managers
{
    public class WorldObjectManager
    {
        private Dictionary<string, WorldObject> WorldObjects { get; set; } = new Dictionary<string, WorldObject>();
        private Sender PositionSender { get; } = new Sender(StaticChannelNames.CollisionChannel);

        public WorldObjectManager()
        {
            PositionSender.Connect();
        }

        public WorldObject SpawnWorldObject()
        {
            var obj = new WorldObject();
            WorldObjects.Add(obj.ObjectId, obj);
            return obj;
        }

        public bool RemoveWorldObject(string objectId)
        {
            return WorldObjects.Remove(objectId);
        }

        public WorldObject GetWorldObject(string objectId)
        {
            return WorldObjects.ContainsKey(objectId) ? WorldObjects[objectId] : null;
        }

        public bool SetWorldObject(WorldObject obj)
        {
            if (WorldObjects.ContainsKey(obj.ObjectId))
            {
                WorldObjects[obj.ObjectId] = obj;
                return true;
            }
            return false;
        }

        public bool UpdateObjectPosition(string objectId, Vector2 movement)
        {
            if (WorldObjects.ContainsKey(objectId))
            {
                WorldObjects[objectId].Position += movement;
                foreach (var collider in WorldObjects[objectId].Colliders)
                {
                    PositionSender.SendMessage(new ObjectPositionMessage
                    {
                        ColliderId = collider.ColliderId,
                        Position = WorldVector2.ConvertUnityVector2(movement),
                        WorldTile = WorldObjects[objectId].WorldTile
                    });
                }
                return true;
            }
            return false;
        }

        public bool UpdateObjectTile(string objectId, string worldTile)
        {
            if (WorldObjects.ContainsKey(objectId))
            {
                WorldObjects[objectId].WorldTile = worldTile;
                foreach (var collider in WorldObjects[objectId].Colliders)
                {
                    PositionSender.SendMessage(new ObjectWorldTileMessage {ColliderId = collider.ColliderId, WorldTile = worldTile});
                }
                return true;
            }
            return false;
        }
    }
}