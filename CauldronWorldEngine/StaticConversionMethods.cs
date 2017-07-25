using CauldronWorldEngine.Database.Data;
using UnityEngine;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine
{
    public static class StaticConversionMethods
    {
        public static Vector2 ToUnityVector2(Microsoft.Xna.Framework.Vector2 pos)
        {
            return new Vector2(pos.X,pos.Y);
        }

        public static Microsoft.Xna.Framework.Vector2 ToMicrosoftVector2(Vector2 pos)
        {
            return new Microsoft.Xna.Framework.Vector2(pos.x, pos.y);
        }

        public static Microsoft.Xna.Framework.Vector2 ToMicrosoftVector2(WorldVector2 vector)
        {
            return new Microsoft.Xna.Framework.Vector2(vector.X, vector.Y);
        }

        public static Microsoft.Xna.Framework.Vector2 GetColliderPosition(NetCollider collider, CharacterData character)
        {
            return new Microsoft.Xna.Framework.Vector2(collider.Offset.X + character.Position.x,collider.Offset.Y + character.Position.y);
        }
    }
}