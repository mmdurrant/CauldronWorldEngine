using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.World;
using UnityEngine;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.Managers
{
    public class WorldObjectManager
    {
        private List<WorldObjectType> ObjectTypes { get; set; } = new List<WorldObjectType>();
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
                
                //TODO: Add message for updating position on clients in the same tile or in vision range
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

        public void SetObjectTypes(List<WorldObjectType> objectTypes)
        {
            ObjectTypes = objectTypes;
        }

        public List<WorldObjectType> GetObjectTypes()
        {
            return ObjectTypes;
        }

        public bool AddObjectType(WorldObjectType worldObjectType)
        {
            if (!ObjectTypes.Exists(o => o.Type == worldObjectType.Type))
            {
                ObjectTypes.Add(worldObjectType);
                return true;
            }
            return false;
        }

        public bool RemoveObjectType(string objectType)
        {
            var obj = ObjectTypes.Find(o => o.Type == objectType);
            if (obj != null)
            {
                return ObjectTypes.Remove(obj);
            }
            return false;
        }

        public bool SetObjectType(WorldObjectType worldObjectType)
        {
            var index = ObjectTypes.FindIndex(o => o.Type == worldObjectType.Type);
            if (index > -1)
            {
                ObjectTypes[index] = worldObjectType;
                return true;
            }
            return false;
        }

        public WorldObjectData GetData()
        {
            var data = new WorldObjectData {WorldObjectTypes = ObjectTypes, WorldObjects = new List<WorldObject>()};
            foreach (var obj in WorldObjects.Values)
            {
                if (obj.IsData)
                {
                    data.WorldObjects.Add(obj);
                }
            }
            return data;
        }

        public void LoadData(WorldObjectData data)
        {
            ObjectTypes = data.WorldObjectTypes;
            WorldObjects = new Dictionary<string, WorldObject>();
            foreach (var obj in data.WorldObjects)
            {
                WorldObjects.Add(obj.ObjectId, obj);
            }
        }
    }
}