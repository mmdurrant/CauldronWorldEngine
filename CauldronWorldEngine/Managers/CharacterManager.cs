using System.Collections.Generic;
using CauldronWorldEngine.Database;
using CauldronWorldEngine.Database.Data;
using UnityEngine;
using WorldMessengerLib.WorldMessages.Characters;

namespace CauldronWorldEngine.Managers
{
    public class CharacterManager
    {

        public CharacterManager(string defaultWorldTile, Vector2 defaultPosition)
        {
            DefaultPosition = defaultPosition;
            DefaultWorldTile = defaultWorldTile;
        }

        private Vector2 DefaultPosition;
        private string DefaultWorldTile;
        private CharacterDatabase Database = new CharacterDatabase();
        private Dictionary<string, WorldCharacter> ActiveCharacters = new Dictionary<string, WorldCharacter>();

        public CommandResponse<bool> AddCharacter(WorldCharacter character)
        {
            var result = Database.AddCharacter(character, DefaultWorldTile, DefaultPosition);
            return result.Success ? new CommandResponse<bool> { Success = true, Result = result.Result } : new CommandResponse<bool> { Success = false, Exception = result.Exception };
        }

        public CommandResponse<WorldCharacter> GetActiveCharacter(string characterName, string playerId)
        {
            if (ActiveCharacters.ContainsKey(playerId))
            {
                return ActiveCharacters[playerId].Name == characterName
                    ? new CommandResponse<WorldCharacter> {Result = ActiveCharacters[playerId], Success = true}
                    : new CommandResponse<WorldCharacter>
                    {
                        Success = false,
                        Message = $"Character {characterName} is not being played by PlayerId {playerId}"
                    };
            }
            return new CommandResponse<WorldCharacter> {Success = false, Message = $"PlayerId {playerId} is not logged in"};
        }

        public CommandResponse<WorldCharacter> SetActiveCharacter(WorldCharacter character, string playerId)
        {
            if (ActiveCharacters.ContainsKey(playerId))
            {
                ActiveCharacters[playerId] = character;
                return new CommandResponse<WorldCharacter> {Success = true, Result = ActiveCharacters[playerId]};
            }
            return new CommandResponse<WorldCharacter> {Success = false, Message = $"PlayerId {playerId} is not logged in"};
        }

        public CommandResponse<CharacterData> GetCharacterFromDatabase(string characterName, string clientId)
        {
            var result = Database.GetCharacter(characterName, clientId);
            return result.Success
                ? new CommandResponse<CharacterData> { Success = true, Result = result.Result }
                : new CommandResponse<CharacterData> { Success = false, Exception = result.Exception };

        }
        public CommandResponse<List<CharacterData>> GetAllCharactersFromDatabase(string clientId)
        {
            var result = Database.GetAllCharacters(clientId);
            return result.Success
                ? new CommandResponse<List<CharacterData>> { Success = true, Result = result.Result }
                : new CommandResponse<List<CharacterData>> { Exception = result.Exception, Success = false };
        }

        public CommandResponse<bool> DeleteCharacter(string characterName, string clientId)
        {
            var result = Database.DeleteCharacter(characterName, clientId);
            return result.Success
                ? new CommandResponse<bool> { Success = true, Result = result.Result }
                : new CommandResponse<bool> { Success = false, Exception = result.Exception };
        }

        public CommandResponse<bool> ActivateCharacter(string playerId, WorldCharacter character)
        {
            if (ActiveCharacters.ContainsKey(playerId) || ActiveCharacters.ContainsValue(character))
            {
                return new CommandResponse<bool> { Success = true, Result = false, Message = "Character is already active" };
            }
            ActiveCharacters.Add(playerId, character);
            return new CommandResponse<bool> { Success = true, Result = true };
        }

        public CommandResponse<bool> DeactivateCharacter(string playerId)
        {
            if (!ActiveCharacters.ContainsKey(playerId))
            {
                return new CommandResponse<bool>
                {
                    Success = true,
                    Result = false,
                    Message = "Player's character is not active"
                };
            }

            var result = Database.UpdateCharacter(ActiveCharacters[playerId]);
            if (result.Success)
            {
                return new CommandResponse<bool> { Result = result.Result, Success = true };
            }
            return new CommandResponse<bool>
            {
                Success = false,
                Exception = result.Exception,
                Message = "Error during UpdateCharacter - See exception"
            };
        }
    }
}