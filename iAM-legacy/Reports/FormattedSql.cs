using System;
using System.Text.RegularExpressions;

namespace Reports
{
    /// <summary>
    ///     Represents a SQL query string formatted as a single line with all
    ///     comments removed, suitable for low-level use with ADO.NET.
    /// </summary>
    public class FormattedSql
    {
        private static readonly Regex SqlComment = new Regex("--.*?$", RegexOptions.Multiline);

        private static readonly Regex Whitespace = new Regex("\\s+");

        private readonly string Sql;

        /// <summary>
        ///     Creates an uncommented, one-line SQL query string from the
        ///     potentially formatted and commented SQL query string provided.
        /// </summary>
        /// <param name="sql"></param>
        public FormattedSql(string sql)
        {
            if (sql == null) throw new ArgumentNullException(nameof(sql));

            Sql = Whitespace.Replace(SqlComment.Replace(sql, " "), " ");
        }

        /// <summary>
        ///     Implicitly converts to a plain string object containing the SQL
        ///     query.
        /// </summary>
        /// <param name="sql"></param>
        public static implicit operator string(FormattedSql sql) => sql?.ToString();

        public override string ToString() => Sql;
    }
}
