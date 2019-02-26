using System.Data;
using System.Data.SqlClient;

namespace BridgeCare.Utility
{
    public class UtilityFunctions
    {
        public static DataTable NonEntitySQLQuery(string selectStatement, BridgeCareContext db)
        {
            var resultsDataTable = new DataTable();
            var connection = new SqlConnection(db.Database.Connection.ConnectionString);
            using (var cmd = new SqlCommand(selectStatement, connection))
            {
                cmd.CommandTimeout = 180;
                var dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(resultsDataTable);
                dataAdapter.Dispose();
            }
            return resultsDataTable;
        }

        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        //check for year
        public static bool IsIamYear(string str)
        {
            if (IsDigitsOnly(str) && str.Length == 4)
            {
                return true;
            }

            return false;
        }
    }
}