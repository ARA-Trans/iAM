using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace BridgeCare.ApplicationLog
{
    public static class ThrowError
    {
        public static void SqlError(SqlException ex, string table)
        {
            WriteSqlEntry(ex, " error ", table);
        }
        public static void OutOfMemoryError(OutOfMemoryException ex)
        {
            WriteOutOfMemoryEntry(ex);
        }
        public static void GeneralError(Exception ex)
        {
            WriteGeneralEntry(ex);
        }

        private static void WriteGeneralEntry(Exception ex)
        {
            throw new Exception($"{ex}");
        }

        private static void WriteSqlEntry(SqlException ex, string type, string table)
        {
            if (ex.Number == -2 || ex.Number == 11)
            {
                throw new TimeoutException("The server has timed out. Please try after some time");
            }
            if (ex.Number == 208)
            {
                throw new InvalidOperationException($"{table} table does not exist in the database");
            }
        }

        private static void WriteOutOfMemoryEntry(OutOfMemoryException ex)
        {
            throw new OutOfMemoryException($"{ex} The server is out of memory. Please try after some time");
        }
    }
}