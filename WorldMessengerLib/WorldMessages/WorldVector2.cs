using System;
using UnityEngine;

namespace WorldMessengerLib.WorldMessages
{
    [Serializable]
    public class WorldVector2
    {
        public float X;
        public float Y;

        public static Vector2 ToUnityVector2(WorldVector2 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static WorldVector2 ConvertUnityVector2(Vector2 vector)
        {
            return new WorldVector2 {X = vector.x, Y = vector.y};
        }
    }
}