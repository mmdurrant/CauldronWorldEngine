using System;
using System.Collections.Generic;
using CollisionEngineLib;

namespace CauldronWorldEngine.World
{
    [Serializable]
    public class WorldCollisionData
    {
        public List<CollisionData> CollisionData { get; set; }
    }
}