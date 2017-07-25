using System.Data.SqlClient;

namespace CauldronWorldEngine.Database
{
    public class DatabaseController
    {
        private static string DataSource = "cheroes-staging.cgzckquvfaxc.us-west-2.rds.amazonaws.com";
        private static string InitialCatalog = "CauldronHeroes";
        private static string u = "chAdmin";
        private static string _pd = @"graveyard13";

        internal SqlConnection dbConnection = new SqlConnection($"Server={DataSource};Database={InitialCatalog};User Id={u};Password={_pd};MultipleActiveResultSets=True;");
    }
}