using System;
using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.Database;
using WorldMessengerLib;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.Managers
{
    public class AccountManager
    {
        private static string InvalidAccountMessage = "Account name is invalid, missing, or already exists";
        public Dictionary<string, PlayerClient> PlayerClients { get; private set; }

        private readonly AccountDatabase _database = new AccountDatabase();

        public AccountManager()
        {
            PlayerClients = new Dictionary<string, PlayerClient>();
        }

        public PlayerClient GeneratePlayerClient(int connectionId)
        {
            var client = new PlayerClient(string.Empty, Guid.NewGuid().ToString(), connectionId);
            PlayerClients.Add(client.PlayerId, client);
            return client;
        }

        public bool IsUserLoggedIn(string username)
        {
            return PlayerClients.Values.ToList().Exists(p => p.PlayerName == username);
        }

        public bool DoesUsernameExist(string username)
        {
            return _database.DoesAccountExist(username).Result;
        }

        public PlayerClient GetClientByPlayerId(string playerId)
        {
            if (PlayerClients.ContainsKey(playerId))
            {
                return PlayerClients[playerId];
            }
            return null;
        }

        public PlayerClient GetClientByConnectionId(int id)
        {
            var clients = PlayerClients.Values.ToList();
            if (clients.Exists(c => c.ConnectionId == id))
            {
                return clients.Find(p => p.ConnectionId == id);
            }
            return null;

        }

        public bool DisconnectClientByConnectionId(int connectionId)
        {
            var client = PlayerClients.Values.ToList().Find(p => p.ConnectionId == connectionId);
            if (client != null)
            {
                PlayerClients.Remove(client.PlayerId);
            }
            return false;
        }

        public CommandResponse<bool> CreateAccount(string username, string password, string email)
        {
            var result = _database.DoesAccountExist(username);
            if (result.Result)
            {
                return new CommandResponse<bool>
                {
                    Success = true,
                    Result = false,
                    Message = InvalidAccountMessage
                };
            }
            var createResult = _database.CreateAccount(username, password, email);
            if (createResult.Success)
            {
                if (createResult.Result)
                {
                    return new CommandResponse<bool>
                    {
                        Result = true,
                        Success = true,
                        Message = "Account created succesfully!"
                    };
                }
            }
            return new CommandResponse<bool> {Success = false, Result = false, Exception = createResult.Exception};
        }

        public CommandResponse<string> Login(string username, string password, string playerId)
        {
            var result = _database.LoginAccount(username, password);
            if (result.Success)
            {
                if (result.Result != null)
                {
                    PlayerClients[playerId].ClientId = result.Result;
                    PlayerClients[playerId].PlayerName = username;
                    return new CommandResponse<string>
                    {
                        Result = result.Result,
                        Success = true,
                        Message = "Login succesful!"
                    };
                }
            }
            return new CommandResponse<string>
            {
                Success = false,
                Exception = result.Exception,
                Message = string.Format("Login failed - {0}", InvalidAccountMessage)
            };
        }

        public CommandResponse<List<AccountData>> ViewAccounts()
        {
            var result = _database.GetPlayerAccounts();
            return result.Success
                ? new CommandResponse<List<AccountData>> {Success = true, Result = result.Result}
                : new CommandResponse<List<AccountData>>
                {
                    Success = false,
                    Exception = result.Exception,
                    Message = "Error during GetPlayerAccounts"
                };
        }
    }
}