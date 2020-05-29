using Microsoft.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.Testing.CodeGeneration
{
    internal static class SqlDataReaderExtensions
    {
        public static double? GetNullableDouble(this SqlDataReader reader, int i)
        {
            var sqlDouble = reader.GetSqlDouble(i);
            return sqlDouble.IsNull ? null : sqlDouble.Value.AsNullable();
        }

        public static string GetNullableString(this SqlDataReader reader, int i)
        {
            var sqlString = reader.GetSqlString(i);
            return sqlString.IsNull ? null : sqlString.Value;
        }
    }
}
