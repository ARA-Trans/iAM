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
    }

}