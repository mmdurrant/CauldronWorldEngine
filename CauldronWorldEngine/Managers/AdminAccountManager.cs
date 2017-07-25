using System;
using System.Collections.Generic;
using System.Linq;
using CauldronWorldEngine.Database;
using WorldMessengerLib.WorldMessages;

namespace CauldronWorldEngine.Managers
{
    public class AdminAccountManager
    {
        private Dictionary<string, PlayerClient> AdminAccounts { get; set; } = new Dictionary<string, PlayerClient>();
        private AccountDatabase Database { get; set; } = new AccountDatabase();

        public PlayerClient GeneratePlayerClient(int connectionId)
        {
            var client = new PlayerClient("",Guid.NewGuid().ToString(),connectionId);
            AdminAccounts.Add(client.PlayerId, client);
            return client;
        }
        
        public CommandResponse<bool> LoginAdmin(string usernname, string password, string playerId)
        {
            if (AdminAccounts.ContainsKey(playerId))
            {
                if (string.IsNullOrEmpty(AdminAccounts[playerId].ClientId))
                {
                    var result = Database.LoginAdminAccount(usernname, password);

                    if (result.Success)
                    {
                        AdminAccounts[playerId].ClientId = result.Result;
                        AdminAccounts[playerId].PlayerName = usernname;
                        return new CommandResponse<bool>
                        {
                            Success = true,
                            Result = !string.IsNullOrEmpty(result.Result)
                        };
                    }
                    else
                    {
                        return new CommandResponse<bool>
                        {
                            Success = false,
                            Exception = result.Exception,
                            Message = "Error while logging in"
                        };
                    }

                }
                return new CommandResponse<bool>{Success = true, Result = false, Message = "User is already logged in"};
            }
            return new CommandResponse<bool>
            {
                Success = false,
                Message = "Id does not exist",
                Exception = new Exception("Id does not exist")
            };
        }

        public CommandResponse<bool> LogoutAdmin(string playerId)
        {
            return AdminAccounts.Remove(playerId) ? new CommandResponse<bool> {Success = true, Result = true, Message = "Logout succesful!"} : new CommandResponse<bool> {Success = true, Result = false, Message = "User does not exist"};
        }

        public CommandResponse<bool> LogoutAdmin(int connectionId)
        {
            var admins = AdminAccounts.Values.ToList();
            var user = admins.Find(u => u.ConnectionId == connectionId);
            return user != null
                ? LogoutAdmin(user.PlayerId)
                : new CommandResponse<bool>
                {
                    Success = false,
                    Result = false,
                    Message = "Unable to find admin with that connection id",
                    Exception = new Exception("Unable to find connection id")
                };
        }
        public CommandResponse<bool> CreateAdminAccount(string username, string password)
        {
            var result = Database.CreateAdminAccount(username, password);
            return result.Success
                ? new CommandResponse<bool> {Success = true, Result = result.Result}
                : new CommandResponse<bool>
                {
                    Success = false,
                    Result = false,
                    Exception = result.Exception,
                    Message = "Error while creating admin account"
                };
        }

        public PlayerClient GetClientByPlayerId(string playerId)
        {
            return AdminAccounts.ContainsKey(playerId) ? AdminAccounts[playerId] : null;
        }

        public PlayerClient GetClientByConnectionId(int connectionId)
        {
            var admins = AdminAccounts.Values.ToList();
            var client = admins.Find(a => a.ConnectionId == connectionId);
            return client;
        }
        
    }
}