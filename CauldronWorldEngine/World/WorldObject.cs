using System;
using System.Collections.Generic;
using CollisionEngineLib.Objects;
using UnityEngine;

namespace CauldronWorldEngine
{
    public class WorldObject
    {
        public string ObjectId { get; }
        public string Tag { get; set; } = "Default";
        public WorldObject()
        {
            ObjectId = Guid.NewGuid().ToString();
        }

        public Vector2 Position { get; set; }
        public string WorldTile { get; set; }
        public List<Collidable> Colliders { get; set; }
    }
}