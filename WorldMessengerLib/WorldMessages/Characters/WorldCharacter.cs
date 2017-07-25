using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldMessengerLib.WorldMessages.Characters
{
    [Serializable]
    public class WorldCharacter
    {
        public string ClientId;
        public string Class;
        public string Name;
        public string ObjectId;
        
        public WorldCharacter GetClientCharacter()
        {
            return new WorldCharacter
            {
                Class = Class,
                Name = Name,
                ClientId = string.Empty,
                ObjectId = string.Empty
            };

        }
    }

    public class StaticCharacterStrings
    {
        //Classes
        public static List<string> Classes = new List<string>
        {
            StaticClassStrings.Wizard
        };
    }

    public class StaticClassStrings
    {
        public const string Wizard = "Wizard";
    }
}