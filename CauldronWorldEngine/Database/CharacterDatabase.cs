using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CauldronWorldEngine;
using CauldronWorldEngine.Database.Data;
using UnityEngine;
using WorldMessengerLib.WorldMessages.Characters;

namespace CauldronWorldEngine.Database
{
    public class CharacterDatabase : DatabaseController
    {
        public DbResponse<bool> DoesCharacterNameExist(string name)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand(
                        string.Format("select top 1 CharacterName from CharacterData where CharacterName = '{0}'", name), dbConnection);
                var reader = dbCommand.ExecuteReader();
                return new DbResponse<bool> { Result = reader.HasRows, Success = true };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }


        }
        public DbResponse<bool> AddCharacter(WorldCharacter character, string startingTile, Vector2 startingPosition)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand(string.Format(
                        "insert into CharacterData (ClientId, CharacterName, Class, CreatedOn) VALUES ('{0}', '{1}', '{2}', '{3}')", character.ClientId, character.Name, character.Class, DateTime.UtcNow), dbConnection);
                var result = dbCommand.ExecuteNonQuery() == 1;
                var posResult = false;
                if (result)
                {
                    var posCommand = new SqlCommand(string.Format("insert into CharacterPosition (ClientId, CharacterName, X, Y, WorldTile, ModifiedOn) VALUES ('{0}', '{1}', {2}, {3}, '{4}', '{5}')", character.ClientId, character.Name, startingPosition.x, startingPosition.y, startingTile, DateTime.UtcNow), dbConnection);
                    posResult = posCommand.ExecuteNonQuery() == 1;
                }
                return new DbResponse<bool> { Result = result && posResult, Success = true };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public DbResponse<CharacterData> GetCharacter(string characterName, string clientId)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand(string.Format(
                        "select top 1 * from CharacterData where CharacterName = '{0}' and ClientId = '{1}'",
                        characterName, clientId), dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var character = new CharacterData
                        {
                            ClientId = clientId,
                            Class = (string)reader["Class"],
                            Name = characterName
                        };
                        var posCommand = new SqlCommand(string.Format("Select top 1 * from CharacterPosition where ClientId = '{0}' and CharacterName = '{1}'", clientId, characterName), dbConnection);
                        var posReader = posCommand.ExecuteReader();
                        while (posReader.Read())
                        {
                            if (posReader.HasRows)
                            {
                                character.WorldTile = (string)posReader["WorldTile"];
                                character.Position = new Vector2((float)posReader["X"], (float)posReader["Y"]);
                            }
                        }
                        return new DbResponse<CharacterData> { Result = character, Success = true };
                    }
                }
                return new DbResponse<CharacterData>
                {
                    Success = false,
                    Exception = new Exception("Unable to get character")
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<CharacterData> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }


        }

        public DbResponse<List<CharacterData>> GetAllCharacters(string clientId)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand(string.Format("Select * from CharacterData where ClientId = '{0}'", clientId),
                        dbConnection);
                var reader = dbCommand.ExecuteReader();
                var result = new DbResponse<List<CharacterData>> { Success = true, Result = new List<CharacterData>() };
                while (reader.Read())
                {
                    result.Result.Add(new CharacterData
                    {
                        Name = (string)reader["CharacterName"],
                        ClientId = clientId,
                        Class = (string)reader["Class"]
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                return new DbResponse<List<CharacterData>> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public DbResponse<bool> UpdateCharacter(WorldCharacter character)
        {
            try
            {
                //dbConnection.Open();

                return new DbResponse<bool> { Success = true, Result = false };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Exception = ex };
            }
            finally
            {
                //dbConnection.Close();
            }
        }

        public DbResponse<bool> DeleteCharacter(string characterName, string clientId)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand(
                        string.Format("select 1 * from CharacterData where CharacterName = '{0}' and ClientId = '{1}'",
                            characterName, clientId), dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        var removeCommand =
                            new SqlCommand(string.Format(
                                "update Character set IsDeleted = 1 where Character = '{0}' and ClientId = {1}",
                                characterName, clientId));
                        var result = removeCommand.ExecuteNonQuery();
                        return new DbResponse<bool> { Success = true, Result = result == 1 };
                    }
                }
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
            return new DbResponse<bool> { Success = true, Result = false };
        }

        public DbResponse<CharacterPosition> GetCharacterPosition(string clientId, string characterName)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand(
                        string.Format(
                            "Select top 1 * from CharacterPosition where ClientId = '{0}' and CharacterName = '{1}'",
                            clientId, characterName), dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        return new DbResponse<CharacterPosition>
                        {
                            Success = true,
                            Result = new CharacterPosition
                            {
                                Position = new Vector2((float)reader["X"], (float)reader["Y"]),
                                WorldTile = (string)reader["WorldTile"]
                            }
                        };
                    }
                }
                return new DbResponse<CharacterPosition> { Success = true, Result = null };
            }
            catch (Exception ex)
            {
                return new DbResponse<CharacterPosition> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public DbResponse<bool> UpdateCharacterPosition(string clientId, string characterName, Vector2 position, string worldTile)
        {
            try
            {
                dbConnection.Open();
                var result = UpdateCharacterPosition(clientId, characterName, position, worldTile, dbConnection);
                return new DbResponse<bool> { Success = true, Result = result };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
        }


        public DbResponse<Dictionary<string, bool>> UpdateCharacterPositions(Dictionary<WorldCharacter, CharacterPosition> input)
        {
            var returnValue = new Dictionary<string, bool>();
            try
            {
                dbConnection.Open();
                foreach (var c in input)
                {
                    var result = UpdateCharacterPosition(c.Key.ClientId, c.Key.Name, c.Value.Position,
                        c.Value.WorldTile, dbConnection);
                    returnValue.Add(c.Key.Name, result);
                }
                return new DbResponse<Dictionary<string, bool>> { Success = true, Result = returnValue };
            }
            catch (Exception ex)
            {
                return new DbResponse<Dictionary<string, bool>> { Success = false, Exception = ex, Result = returnValue };
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private static bool UpdateCharacterPosition(string clientId, string characterName, Vector2 position, string worldTile, SqlConnection conn)
        {
            var dbCommand =
                new SqlCommand(string.Format(
                    "update CharacterPosition set X = {0}, Y = {1}, WorldTile = {2} where ClientId = '{3}' and CharacterName = '{4}'", position.x, position.y, worldTile, clientId, characterName), conn);
            var result = dbCommand.ExecuteNonQuery();
            return result == 1;
        }
    }
}