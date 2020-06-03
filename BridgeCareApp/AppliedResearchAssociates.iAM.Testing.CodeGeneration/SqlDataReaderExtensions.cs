using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Testing.CodeGeneration
{
    internal static class SqlDataReaderExtensions
    {
        public static bool? GetNullableBoolean(this SqlDataReader reader, int i)
        {
            var sqlValue = reader.GetSqlBoolean(i);
            return sqlValue.IsNull ? null : sqlValue.Value.AsNullable();
        }

        public static double? GetNullableDouble(this SqlDataReader reader, int i)
        {
            var sqlValue = reader.GetSqlDouble(i);
            return sqlValue.IsNull ? null : sqlValue.Value.AsNullable();
        }

        public static int? GetNullableInt32(this SqlDataReader reader, int i)
        {
            var sqlValue = reader.GetSqlInt32(i);
            return sqlValue.IsNull ? null : sqlValue.Value.AsNullable();
        }

        public static string GetNullableString(this SqlDataReader reader, int i)
        {
            var sqlValue = reader.GetSqlString(i);
            return sqlValue.IsNull ? null : sqlValue.Value;
        }
    }
}
