using System;
using UnityEngine;

namespace WorldMessengerLib.WorldMessages
{
    [Serializable]
    public struct WorldVector2
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

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public static bool operator ==(WorldVector2 left, WorldVector2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(WorldVector2 left, WorldVector2 right)
        {
            return left.X != right.X && left.Y != right.Y;
        }

        public bool Equals(WorldVector2 other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is WorldVector2 && Equals((WorldVector2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }
    }
}