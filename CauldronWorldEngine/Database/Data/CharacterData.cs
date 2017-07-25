using UnityEngine;
using WorldMessengerLib.WorldMessages.Characters;

namespace CauldronWorldEngine.Database.Data
{
    public class CharacterData
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public Vector2 Position { get; set; }
        public string WorldTile { get; set; }
        public string ClientId { get; set; }

        public static WorldCharacter ToClientCharacter(CharacterData data)
        {
            return new WorldCharacter {Name = data.Name, Class = data.Class};
        }

        public static WorldCharacter ToServerCharacter(CharacterData data)
        {
            return new WorldCharacter{Name = data.Name, Class = data.Class, ClientId = data.ClientId};
        }
    }

    
}