using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CauldronWorldEngine.Database
{
    public class AccountDatabase : DatabaseController
    {
        public DbResponse<bool> DoesAccountExist(string username)
        {
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand($"select top 1 Username from ClientLogin where Username = '{username}'",
                        dbConnection);
                var reader = dbCommand.ExecuteReader();
                return new DbResponse<bool> { Success = true, Result = reader.HasRows };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Exception = ex, Success = false };
            }
            finally
            {
                dbConnection.Close();
            }
        }
        public DbResponse<bool> CreateAccount(string username, string password, string email)
        {
            DbResponse<bool> returnResponse;
            try
            {
                dbConnection.Open();
                var clientId = Guid.NewGuid().ToString();

                var dbCommand =
                    new SqlCommand(
                        $"insert into ClientLogin (ClientId,Username,ClientPassword,CreatedOn) VALUES ('{clientId}', '{username}', '{SecurePasswordHasher.Hash(password)}', '{DateTime.UtcNow}')", dbConnection);
                var result = dbCommand.ExecuteNonQuery();
                returnResponse = new DbResponse<bool> { Success = true, Result = result == 1 };
            }
            catch (Exception ex)
            {
                returnResponse = new DbResponse<bool> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
            return returnResponse;
        }
        public DbResponse<string> LoginAccount(string username, string password)
        {
            DbResponse<string> returnValue = null;
            try
            {
                dbConnection.Open();
                var dbCommand =
                    new SqlCommand($"Select top 1 * from ClientLogin where Username = '{username}'",
                        dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows && SecurePasswordHasher.Verify(password, reader["ClientPassword"].ToString()))
                    {
                        var id = reader["ClientId"].ToString();
                        returnValue = new DbResponse<string> { Success = true, Result = id };
                    }
                }
                if (returnValue == null)
                {
                    returnValue = new DbResponse<string> { Success = true, Result = null };
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                returnValue = new DbResponse<string> { Exception = ex, Success = false };
            }
            finally
            {
                dbConnection.Close();
            }
            return returnValue;
        }

        public DbResponse<List<string>> ViewAccounts()
        {
            try
            {
                var dbCommand = new SqlCommand("Select Username from ClientLogin");
                var reader = dbCommand.ExecuteReader();
                var returnValue = new DbResponse<List<string>> { Success = true, Result = new List<string>() };
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        returnValue.Result.Add((string)reader["Username"]);
                    }
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                return new DbResponse<List<string>> { Success = false, Exception = ex };
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public DbResponse<bool> CreateAdminAccount(string username, string password)
        {
            try
            {
                var dbCommand = new SqlCommand($"Select top 1 Username from AdminLogin where Username = {username}", dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.HasRows) continue;
                    var clientId = Guid.NewGuid().ToString();
                    var create =
                        new SqlCommand(
                            $"insert into ClientLogin (ClientId,Username,ClientPassword,CreatedOn) VALUES ('{clientId}', '{username}', '{SecurePasswordHasher.Hash(password)}', '{DateTime.UtcNow}')",
                            dbConnection);
                    return create.ExecuteNonQuery() == 1
                        ? new DbResponse<bool> {Success = true, Result = true}
                        : new DbResponse<bool> {Success = true, Result = false};
                }
                return new DbResponse<bool> { Success = true, Result = false };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> {Success = false, Exception = ex};
            }
        }

        public DbResponse<string> LoginAdminAccount(string username, string password)
        {
            try
            {
                var dbCommand = new SqlCommand($"Select top 1 * from AdminLogin where Username = {username}",
                    dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    return reader.HasRows
                        ? new DbResponse<string> {Success = true, Result = (string) reader["ClientId"]}
                        : new DbResponse<string> {Success = true, Result = null};
                }
                return new DbResponse<string> {Success = true, Result = null};
            }
            catch(Exception ex)
            {
                return new DbResponse<string> {Exception = ex, Success = false};
            }
        }

        public DbResponse<bool> DoesAdminUserExist(string username)
        {
            try
            {
                var dbCommand = new SqlCommand($"Select top 1 * from AdminLogin where Username = {username}", dbConnection);
                var reader = dbCommand.ExecuteReader();
                while (reader.Read())
                {
                    return new DbResponse<bool> {Success = true, Result = reader.HasRows};
                }
                return new DbResponse<bool>() {Success = true, Result = false};
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>{Success = false, Exception = ex};
            }

        }
    }
}