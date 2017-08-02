using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using WorldMessengerLib.WorldMessages.Characters;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace WorldMessengerLib.WorldMessages
{
    public interface IWorldMessage
    {
        WorldMessageType MessageType { get; }
    }

    public enum WorldMessageType
    {
        Collision,
        UpdatePosition,
        LoginRequest,
        LoginResult,
        LoginAttempt,
        LoginAttemptResult,
        LogutRequest,
        CharacterListRequest,
        CharacterListReply,
        ActivateCharacter,
        ActivateCharacterReply,
        CharacterRequest,
        CharacterReply,
        ServerInitialize,
        Tick,
        Exception,
        CreateCharacter,
        CreateCharacterReply,
        CreateAccount,
        CreateAccountResult,
        WorldTileRequest,
        WorldTileReply,
        UpdateCharacterWorldTile,
        Disconnect,
        Error,
        Position,
        ObjectWorldTile,
        AddWorldTile,
        RemoveWorldTile,
        SetWorldTileSize,
        SetTile,
        AddCollisionEngine,
        RemoveCollisionEngine,
        AdminLoginRequest,
        AdminLoginResult,
        AdminLoginAttempt,
        AdminLoginAttemptResult,
        AdminLogoutRequest,
        CreateAdminAccountRequest,
        CreateAdminAccountResult,
        AdminAccountsRequest,
        AdminAccountsReply,
        PlayerAccountsRequest,
        PlayerAccountsReply,
        AddWorldTileReply,
        SaveWorldTileRequest,
        SaveWorldTileReply
    }

    [Serializable]
    public class WorldMessage
    {

        public WorldMessageType MessageType { get; }
        public byte[] Data { get; private set; }

        public WorldMessage(IWorldMessage message)
        {
            MessageType = message.MessageType;
            Data = ToByteArray(message);
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);
                var data = ms.ToArray();
                ms.Close();
                return data;
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);
                var obj = formatter.Deserialize(ms);
                ms.Close();
                return (T) obj;
            }
        }

        public T ReadMessage<T>()
        {
            try
            {
                var data = Deserialize<T>(Data);
                Data = null;
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception during Receiver.ReaderMessage: {e}");
                return default(T);
            }
        }

    }

    [Serializable]
    public class ServerInitializeMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.ServerInitialize;
    }

    [Serializable]
    public class TickMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Tick;
        public DateTime CurrentDateTime { get; set; }
    }

    [Serializable]
    public class ExceptionMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Exception;
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public string From { get; set; }
    }

    [Serializable]
    public class ErrorMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Error;
        public string Message { get; set; }
    }

    [Serializable]
    public class UpdatePositionMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.UpdatePosition;
        public string PlayerId { get; set; }
        public string Character { get; set; }
        public WorldVector2 Movement { get; set; }
        public string WorldTile { get; set; }
    }

    [Serializable]
    public class LoginRequestMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.LoginRequest;
        
        public int ConnectionId { get; set; }
    }

    [Serializable]
    public class LoginResultMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.LoginResult;
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class LoginAttemptMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.LoginAttempt;
        public string PlayerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class LoginAttemptResultMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.LoginAttemptResult;
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class CreateAccountMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CreateAccount;
        public string PlayerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
    }

    [Serializable]
    public class CreateAccountResultMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CreateAccountResult;
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }

    [Serializable]
    public class WorldTileRequestMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.WorldTileRequest;
        public bool IsAdmin { get; set; }
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class WorldTileReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.WorldTileReply;
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
        public NetWorldTile[] WorldTiles { get; set; }
    }

    [Serializable]
    public class CharacterRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CharacterRequest;
        public string PlayerId { get; set; }
        public string CharacterName { get; set; }
    }

    [Serializable]
    public class CharacterReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CharacterReply;
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
        public WorldCharacter Character { get; set; }
    }

    [Serializable]
    public class CreateCharacter : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CreateCharacter;
        public string PlayerId;
        public WorldCharacter Character;
    }

    [Serializable]
    public class CreateCharacterResult : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CreateCharacterReply;
        public bool Success { get; set; }
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class ActivateCharacterRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.ActivateCharacter;
        public string PlayerId { get; set; }
        public string CharacterName { get; set; }
        public NetCollider[] Colliders { get; set; }
    }

    [Serializable]
    public class ActivateCharacterReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.ActivateCharacterReply;
        public string PlayerId { get; set; }
        public string CharacterName { get; set; }
        public int ConnectionId { get; set; }
        public bool Success { get; set; }
    }

    [Serializable]
    public class CharacterListRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CharacterListRequest;
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class CharacterListReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CharacterListReply;
        public string PlayerId { get; set; }
        public int ConnectionId { get; set; }
        public bool Success { get; set; }
        public WorldCharacter[] Characters { get; set; }
    }

    [Serializable]
    public class UpdateCharacterTile : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.UpdateCharacterWorldTile;
        public string PlayerId { get; set; }
        public string WorldTile { get; set; }
    }

    [Serializable]
    public class DiscconectMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.Disconnect;
        public int ConnectionId { get; set; }
    }

    [Serializable]
    public class SetTileMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.SetTile;
        public string WorldTile { get; set; }
        public int Layer { get; set; }
        public WorldVector2 Position { get; set; }
        public uint TileId { get; set; }
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class AddWorldTileMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AddWorldTile;
        public string TileName { get; set; }
        public WorldVector2 Size { get; set; }
    }

    [Serializable]
    public class AddWorldTileReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AddWorldTileReply;
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class RemoveWorldTileMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.RemoveWorldTile;
        public string TileName { get; set; }
    }

    [Serializable]
    public class SetWorldTileSizeMessage : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.SetWorldTileSize;
        public string TileName { get; set; }
        public WorldVector2 Size { get; set; }
    }

    [Serializable]
    public class CreateAdminAccountRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CreateAdminAccountRequest;
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class CreateAdminAccountResult : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.CreateAdminAccountResult;
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class AdminLoginRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AdminLoginRequest;
        public int ConnectionId { get; set; }
    }

    [Serializable]
    public class AdminLoginResult : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AdminLoginResult;
        public int ConnectionId { get; set; }
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class AdminLoginAttempt : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AdminLoginAttempt;
        public string Username { get; set; }
        public string Password { get; set; }
        public string PlayerId { get; set; }
    }

    [Serializable]
    public class AdminLoginAttemptResult : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AdminLoginAttemptResult;
        public int ConnectionId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public class PlayerAccountsRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.PlayerAccountsRequest;
    }

    [Serializable]
    public class PlayerAccountsReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.PlayerAccountsReply;
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AccountData> PlayerAccounts { get; set; }
    }

    [Serializable]
    public class AdminAccountsRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AdminAccountsRequest;
    }

    [Serializable]
    public class AdminAccountsReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.AdminAccountsReply;
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<AccountData> AdminAccounts { get; set; }
    }

    [Serializable]
    public class SaveWorldTileRequest : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.SaveWorldTileRequest;
        public string PlayerId { get; set; }
        public NetWorldTile WorldTile { get; set; }
    }

    [Serializable]
    public class SaveWorldTileReply : IWorldMessage
    {
        public WorldMessageType MessageType { get; } = WorldMessageType.SaveWorldTileReply;
        public int ConnectionId { get; set; }
        public string WorldTile { get; set; }
        public bool Success { get; set; }
    }

    

    

    

    
    
    
    
}
