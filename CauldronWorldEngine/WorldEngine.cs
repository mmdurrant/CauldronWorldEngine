using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CauldronWorldEngine.Database.Data;
using CauldronWorldEngine.Managers;
using CauldronWorldEngine.World;
using CollisionEngineLib.Objects;
using UnityEngine;
using UnityEngine.Assertions.Must;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages;
using WorldMessengerLib.WorldMessages.Characters;
using WorldMessengerLib.WorldMessages.NetTiles;

namespace CauldronWorldEngine
{
    public class WorldEngine
    {
        private DateTime _lastTick { get; set; }
        private TimeSpan _betweenTicks { get; set; } = new TimeSpan(0,0,1);
        private DateTime _currentTime { get; set; }

        private Receiver UnityReceiver { get; set; }
        private Sender UnitySender { get; set; }
        private Sender ExceptionSender { get; set; }
        private Sender ManagerSender { get; set; }
        private Receiver ManagerReceiver { get; set; }
        private WorldSettings Settings { get; set; }

        private AccountManager AccountManager { get; set; } = new AccountManager();
        private AdminAccountManager AdminAccountManager { get; set; } = new AdminAccountManager();
        private CharacterManager CharacterManager { get; set; } = new CharacterManager("The Void", Vector2.zero);
        private WorldObjectManager WorldObjectManager { get; set; } = new WorldObjectManager();
        private CollisionManager CollisionManager { get; set; } = new CollisionManager();
        private WorldTileManager WorldManager { get; set; } = new WorldTileManager();
        

        private bool _readingUnityMessages = false;
        private readonly object _unityMessageLock = new object();

        private bool _readingManagerMessages = false;
        private readonly object _managerMessageLock = new object();

        public string DefaultSavePath => Settings != null ? Settings.SaveDataPath : @"F:\CauldronData";

        public WorldEngine()
        {
            ExceptionSender = new Sender(StaticChannelNames.Exception);
            ExceptionSender.Connect();
        }
        public void Start()
        {
            SubscribeToExchanges();
            Console.WriteLine("Starting Server");
            while (true)
            {
                ReadMessages();
                Tick();
                Thread.Sleep(100);
            }
        }

        public void SaveData(string path)
        {
            var data = new WorldData { Timestamp = _currentTime, WorldTiles = WorldManager.GetTiles() };
            try
            {
                var savePath = $@"{path}\WorldData";
                FileManager.WriteToBinaryFile(savePath, data);
                if (Settings != null)
                {
                    Settings.SaveDataPath = path;
                }
                else
                {
                    Settings = new WorldSettings{SaveDataPath = path};
                }
                
                FileManager.WriteToBinaryFile($@"{path}\WorldSettings", Settings);
            }
            catch (Exception ex)
            {
                ExceptionSender.SendMessage(new ExceptionMessage
                {
                    Exception = ex,
                    From = "FileManager",
                    Message = "Exception during SaveData"
                });
            }
        }

        public void LoadData(string path)
        {
            try
            {
                var data = FileManager.ReadFromBinaryFile<WorldData>($@"{path}\WorldData");
                if (data != null)
                {
                    WorldManager.ClearTiles();
                    _currentTime = data.Timestamp;
                    foreach (var tile in data.WorldTiles)
                    {
                        WorldManager.AddNewTile(tile);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionSender.SendMessage(new ExceptionMessage
                {
                    Exception = ex,
                    From = "FileManager",
                    Message = "Exception during LoadData"
                });
            }
        }

        public void LoadSettings(string path)
        {
            try
            {
                var settings = FileManager.ReadFromBinaryFile<WorldSettings>($@"{path}\WorldSettings");
                Settings = settings ?? new WorldSettings{SaveDataPath = DefaultSavePath};
            }
            catch (Exception ex)
            {
                ExceptionSender.SendMessage(new ExceptionMessage
                {
                    Exception = ex,
                    From = "FileManager",
                    Message = "Exception during LoadSettings"
                });
            }
        }

        internal void Tick()
        {
            var diff = DateTime.Now - _lastTick;
            if (diff >= _betweenTicks)
            {
                _currentTime = _currentTime.Add(new TimeSpan(0, 0, 1));
                _lastTick = DateTime.Now;
                UnitySender.SendMessage(new TickMessage {CurrentDateTime = _currentTime});
            }
        }

        private void SubscribeToExchanges()
        {
            UnityReceiver = new Receiver(StaticChannelNames.WorldChannel);
            UnitySender = new Sender(StaticChannelNames.UnityChannel);
            UnityReceiver.Connect();
            UnitySender.Connect();

            ManagerSender = new Sender(StaticChannelNames.Manager, StaticChannelNames.Manager);
            ManagerReceiver = new Receiver(StaticChannelNames.Manager, StaticChannelNames.WorldChannel);
            ManagerSender.Connect();
            ManagerReceiver.Connect();
        }

        private void ReadMessages()
        {
            ReadUnityMessages();
            ReadManagerMessages();
        }

        private void ReadUnityMessages()
        {
            if (!_readingUnityMessages && UnityReceiver.Messages.Count > 0)
            {
                _readingUnityMessages = true;
                lock (_unityMessageLock)
                {
                    var thread = new Thread(() =>
                    {
                        while (UnityReceiver.Messages.Count > 0)
                        {
                            var msg = UnityReceiver.Messages.Dequeue();
                            //Console.WriteLine(msg.MessageType.ToString());
                            switch (msg.MessageType)
                            {
                                case WorldMessageType.LoginRequest:
                                    ReadLoginRequest(msg.ReadMessage<LoginRequestMessage>());
                                    break;
                                case WorldMessageType.LoginAttempt:
                                    ReadLoginAttempt(msg.ReadMessage<LoginAttemptMessage>());
                                    break;
                                case WorldMessageType.ServerInitialize:
                                    UnitySender.SendMessage(new ServerInitializeMessage());
                                    Console.WriteLine("Unity Server Connected");
                                    break;
                                case WorldMessageType.CreateAccount:
                                    ReadCreateAccount(msg.ReadMessage<CreateAccountMessage>());
                                    break;
                                case WorldMessageType.CreateCharacter:
                                    ReadCreateCharacter(msg.ReadMessage<CreateCharacter>());
                                    break;
                                case WorldMessageType.CharacterRequest:
                                    ReadCharacterRequest(msg.ReadMessage<CharacterRequest>());                                
                                    break;
                                case WorldMessageType.CharacterListRequest:
                                    ReadCharacterListRequest(msg.ReadMessage<CharacterListRequest>());
                                    break;
                                case WorldMessageType.ActivateCharacter:
                                    ReadActivateCharacterRequest(msg.ReadMessage<ActivateCharacterRequest>());
                                    break;
                                case WorldMessageType.UpdateCharacterWorldTile:
                                    break;
                                case WorldMessageType.UpdatePosition:
                                    ReadUpdatePosition(msg.ReadMessage<UpdatePositionMessage>());
                                    break;
                                case WorldMessageType.Disconnect:
                                    break;
                                case WorldMessageType.AdminLoginRequest:
                                    ReadAdminLoginRequest(msg.ReadMessage<AdminLoginRequest>());
                                    break;
                                case WorldMessageType.AdminLoginAttempt:
                                    ReadAdminLoginAttempt(msg.ReadMessage<AdminLoginAttempt>());
                                    break;
                                case WorldMessageType.WorldTileRequest:
                                    ReadWorldTileRequest(msg.ReadMessage<WorldTileRequestMessage>(), false);
                                    break;
                                case WorldMessageType.SaveWorldTileRequest:
                                    ReadSaveWorldTileMessage(msg.ReadMessage<SaveWorldTileRequest>());
                                    break;
                                case WorldMessageType.SetTile:
                                    ReadSetTileMessage(msg.ReadMessage<SetTileMessage>());
                                    break;

                            }
                        }
                        _readingUnityMessages = false;
                    });
                    thread.Start();
                }

            }
        }

        private void ReadManagerMessages()
        {
            if (!_readingManagerMessages && ManagerReceiver.Messages.Count > 0)
            {
                _readingManagerMessages = true;
                lock (_managerMessageLock)
                {
                    var thread = new Thread(() =>
                    {
                        while (ManagerReceiver.Messages.Count > 0)
                        {
                            var msg = ManagerReceiver.Messages.Dequeue();
                            switch (msg.MessageType)
                            {
                                case WorldMessageType.CreateAdminAccountRequest:
                                    ReadCreateAdminAccountRequest(msg.ReadMessage<CreateAdminAccountRequest>());
                                    break;
                                case WorldMessageType.CreateAccount:
                                    ReadCreateAccount(msg.ReadMessage<CreateAccountMessage>());
                                    break;
                                case WorldMessageType.PlayerAccountsRequest:
                                    ReadPlayerAccountsRequest(msg.ReadMessage<PlayerAccountsRequest>());
                                    break;
                                case WorldMessageType.AdminAccountsRequest:
                                    ReadAdminAccountsRequest(msg.ReadMessage<AdminAccountsRequest>());
                                    break;
                                case WorldMessageType.WorldTileRequest:
                                    ReadWorldTileRequest(msg.ReadMessage<WorldTileRequestMessage>(), true);
                                    break;
                                case WorldMessageType.AddWorldTile:
                                    ReadAddWorldTileMessage(msg.ReadMessage<AddWorldTileMessage>());
                                    break;
                            }
                        }
                        _readingManagerMessages = false;
                    });
                    thread.Start();
                }
            }
        }

        private void ReadLoginRequest(LoginRequestMessage message)
        {
            var playerClient = AccountManager.GeneratePlayerClient(message.ConnectionId);
            UnitySender.SendMessage(new LoginResultMessage { PlayerId = playerClient.PlayerId, ConnectionId = playerClient.ConnectionId});
        }

        private void ReadLoginAttempt(LoginAttemptMessage message)
        {
            var result = new LoginAttemptResultMessage
            {
                PlayerId = message.PlayerId,
                Success = false
            };
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                result.ConnectionId = client.ConnectionId;
                if (AccountManager.IsUserLoggedIn(message.Username))
                {
                    result.Message = $"User {message.Username} is already logged in";
                }
                else
                {
                    var login = AccountManager.Login(message.Username, message.Password, message.PlayerId);
                    if (login.Success && login.Result != null)
                    {
                        result.Success = true;
                        result.Message = "Login succesful!";                        
                    }
                    else
                    {
                        result.Message = "Login failed. Please try again.";
                    }
                }
            }
            UnitySender.SendMessage(result);
        }

        private void ReadCreateAccount(CreateAccountMessage message)
        {
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            //TODO: Add error message if Client doesn't exist
            if (client != null || message.IsManager)
            {
                var result = AccountManager.CreateAccount(message.Username, message.Password, message.Email);
                var resultMessage = new CreateAccountResultMessage { PlayerId = message.PlayerId, ConnectionId = message.IsManager ? 0 : client.ConnectionId};
                if (result.Success)
                {
                    resultMessage.Success = result.Result;
                    resultMessage.Message = result.Message;
                }
                else
                {
                    resultMessage.Success = false;
                    resultMessage.Message = "Error during Account creation. Please try again later.";
                }

                if (message.IsManager)
                {
                    ManagerSender.SendMessage(resultMessage);
                }
                else
                {
                    UnitySender.SendMessage(resultMessage);
                }
            }
        }

        private void ReadCreateCharacter(CreateCharacter message)
        {
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                message.Character.ClientId = client.ClientId;
                var result = CharacterManager.AddCharacter(message.Character);
                UnitySender.SendMessage(new CreateCharacterResult
                {
                    ConnectionId = client.ConnectionId,
                    PlayerId = message.PlayerId,
                    Success = result.Success
                });
            }
        }

        private void ReadCharacterRequest(CharacterRequest message)
        {
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                var character = CharacterManager.GetCharacterFromDatabase(message.CharacterName, client.ClientId);
                var result = new CharacterReply {ConnectionId = client.ConnectionId};
                if (character.Success && character.Result != null)
                {
                    result.Character = CharacterData.ToClientCharacter(character.Result);
                }
                UnitySender.SendMessage(result);
            }
        }

        private void ReadCharacterListRequest(CharacterListRequest message)
        {
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                var characters = CharacterManager.GetAllCharactersFromDatabase(client.ClientId);
                if (characters.Success)
                {
                    var charData = characters.Result.Select(CharacterData.ToClientCharacter).ToArray();
                    UnitySender.SendMessage(new CharacterListReply
                    {
                        PlayerId = message.PlayerId,
                        Characters = charData,
                        ConnectionId = client.ConnectionId,
                        Success = true
                    });
                }
            }
            //TODO: Send error
        }

        private void ReadActivateCharacterRequest(ActivateCharacterRequest message)
        {
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                var reply = new ActivateCharacterReply
                {
                    Success = false,
                    ConnectionId = client.ConnectionId,
                    PlayerId = message.PlayerId,
                    CharacterName = message.CharacterName
                };
                var character = CharacterManager.GetCharacterFromDatabase(message.CharacterName, client.ClientId);
                if (character.Success)
                {
                    var serverChar = CharacterData.ToServerCharacter(character.Result);
                    var result = CharacterManager.ActivateCharacter(message.PlayerId, serverChar);
                    reply.Success = result.Success;
                    if (result.Success)
                    {   
                        var obj = WorldObjectManager.SpawnWorldObject();
                        obj.Position = character.Result.Position;
                        obj.WorldTile = character.Result.WorldTile;
                        //Set objectId for character in CharacterManager
                        serverChar.ObjectId = obj.ObjectId;
                        CharacterManager.SetActiveCharacter(serverChar, message.PlayerId);

                        foreach (var collider in message.Colliders)
                        {
                            var collidable = new Collidable();
                            CollisionManager.AddObject(collidable,
                                StaticConversionMethods.ToMicrosoftVector2(collider.Size),
                                StaticConversionMethods.GetColliderPosition(collider, character.Result),
                                character.Result.WorldTile);
                            obj.Colliders.Add(collidable);
                        }
                        WorldObjectManager.SetWorldObject(obj);

                    }
                }
                UnitySender.SendMessage(reply);
            }
        }

        private void ReadUpdatePosition(UpdatePositionMessage message)
        {
            var client = AccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                var character = CharacterManager.GetActiveCharacter(message.Character, client.ClientId);
                if (character.Success)
                {
                    WorldObjectManager.UpdateObjectPosition(character.Result.ObjectId, WorldVector2.ToUnityVector2(message.Movement));
                }
            }
        }

        private void ReadAdminLoginRequest(AdminLoginRequest message)
        {
            var client = AdminAccountManager.GeneratePlayerClient(message.ConnectionId);
            UnitySender.SendMessage(new AdminLoginResult
            {
                ConnectionId = client.ConnectionId,
                PlayerId = client.PlayerId
            });
        }

        private void ReadAdminLoginAttempt(AdminLoginAttempt message)
        {
            var client = AdminAccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                var result = AdminAccountManager.LoginAdmin(message.Username, message.Password, message.PlayerId);
                if (result.Success)
                {

                    UnitySender.SendMessage(new AdminLoginAttemptResult
                    {
                        Success = true,
                        ConnectionId = client.ConnectionId,
                        Message = result.Message
                    });
                }
                else
                {
                    UnitySender.SendMessage(new AdminLoginAttemptResult
                    {
                        Success = false,
                        ConnectionId = client.ConnectionId,
                        Message = result.Message
                    });
                }
            }
        }

        private void ReadCreateAdminAccountRequest(CreateAdminAccountRequest message)
        {
            var result = AdminAccountManager.CreateAdminAccount(message.Username, message.Password);
            ManagerSender.SendMessage(
                result.Success
                    ? new CreateAdminAccountResult {Success = true, Message = "Account created succesfully!"}
                    : new CreateAdminAccountResult {Success = false, Message = "Unable to create account"});
        }

        private void ReadPlayerAccountsRequest(PlayerAccountsRequest message)
        {
            var result = AccountManager.ViewAccounts();
            ManagerSender.SendMessage(result.Success
                ? new PlayerAccountsReply {PlayerAccounts = result.Result, Success = true}
                : new PlayerAccountsReply {Success = false, Message = $"{result.Message}: {result.Exception}"});
        }

        private void ReadAdminAccountsRequest(AdminAccountsRequest message)
        {
            var result = AdminAccountManager.ViewAccounts();
            ManagerSender.SendMessage(result.Success
                ? new AdminAccountsReply {Success = true, AdminAccounts = result.Result}
                : new AdminAccountsReply {Success = false, Message = $"{result.Message}: {result.Exception}"});
        }

        private void ReadWorldTileRequest(WorldTileRequestMessage message, bool isManager)
        {
            var tiles = WorldManager.GetNetTiles();
            if (isManager)
            {
                ManagerSender.SendMessage(new WorldTileReply {WorldTiles = tiles});
            }
            else
            {
                var client = message.IsAdmin ? AdminAccountManager.GetClientByPlayerId(message.PlayerId) : AccountManager.GetClientByPlayerId(message.PlayerId);
                if (client != null)
                {
                    UnitySender.SendMessage(new WorldTileReply
                    {
                        WorldTiles = tiles,
                        ConnectionId = client.ConnectionId,
                        PlayerId = message.PlayerId
                    });
                }
            }
        }

        private void ReadAddWorldTileMessage(AddWorldTileMessage message)
        {
            var result = WorldManager.AddNewTile(new WorldTile{Name = message.TileName, Size = message.Size});
            ManagerSender.SendMessage(new AddWorldTileReply {Success = result, Message = result ? "Tile added succesfully!" : "Failed to add tile"});
        }

        private void ReadSaveWorldTileMessage(SaveWorldTileRequest message)
        {
            var client = AdminAccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                var result = WorldManager.SetWorldTile(WorldTile.ConvertToWorldTile(message.WorldTile));
                UnitySender.SendMessage(new SaveWorldTileReply
                {
                    ConnectionId = client.ConnectionId,
                    Success = result,
                    WorldTile = message.WorldTile.Name
                });
            }
            SaveData(Settings.SaveDataPath);
            
        }

        private void ReadSetTileMessage(SetTileMessage message)
        {
            var client = AdminAccountManager.GetClientByPlayerId(message.PlayerId);
            if (client != null)
            {
                WorldManager.SetSectionTile(new SectionTile{X = (int)message.Position.X, Y = (int)message.Position.Y, TileId =  message.TileId}, message.WorldTile, message.Layer, message.Position);
            }
        }
    }


}