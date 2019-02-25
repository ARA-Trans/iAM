using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using BridgeCare.EntityClasses;
using System.Data.SqlClient;

namespace BridgeCare.Utility
{
    public class UtilityFunctions
    {
        public static DataTable NonEntitySQLQuery(string selectStatement,BridgeCareContext db)
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

        //funny numbers in here assumes we know all data occurs after 1900 and the simulation will never go out farther than 50 years

        public static bool IsIamYear(string str)
        {
            if (IsDigitsOnly(str))
            {
                int number = Convert.ToInt32(str);
                if (number > 1900 && number < (DateTime.Now.Year + 50))
                {
                    return true;
                }
            }

            return false;
        }
    }
      
}