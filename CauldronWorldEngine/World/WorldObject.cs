using System;
using System.Collections.Generic;
using System.Linq;
using CollisionEngineLib.Objects;
using UnityEngine;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldObject : WorldObjectType
    {
        public string ObjectId { get; }
        public WorldObject()
        {
            ObjectId = Guid.NewGuid().ToString();
        }

        public Vector2 Position { get; set; }
        public string WorldTile { get; set; }
        public List<Collidable> Colliders { get; set; }

        public static WorldObject FromWorldObjectType(WorldObjectType worldObjectType)
        {
            var obj = new WorldObject
            {
                Type = worldObjectType.Type
            };
            var props = worldObjectType.GetProperties();

            foreach (var prop in props)
            {
                obj.AddProperty(prop.Key, prop.Value);
            }
            return obj;
        }
    }
}