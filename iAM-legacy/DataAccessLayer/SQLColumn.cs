using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DataAccessLayer
{
    public class SQLColumn
    {
        public int object_id { get; set; }
        public string name { get; set; }
        public int column_id { get; set; }
        public int system_type_id { get; set; }
        public int user_type_id { get; set; }
        public int max_length { get; set; }
        public int precision { get; set; }
        public int scale { get; set; }
        public string collation_name { get; set; }
        public bool is_nullable { get; set; }
        public bool is_ansi_padded { get; set; }
        public bool is_rowguidcol { get; set; }
        public bool is_identity { get; set; }
        public bool is_computed { get; set; }
        public bool is_filestream { get; set; }
        public bool is_replicated { get; set; }
        public bool is_non_sql_subscribed { get; set; }
        public bool is_merge_published { get; set; }
        public bool is_dts_replicated { get; set; }
        public bool is_xml_document { get; set; }
        public int xml_collection_id { get; set; }
        public int default_object_id { get; set; }
        public int rule_object_id { get; set; }
        public bool is_sparse { get; set; }
        public bool is_column_set { get; set; }
        public string column_type_string { get; set; } // This is an added type for altering.

        public SQLColumn()
        {
        }

        public SQLColumn(string columnName, string columnType, bool nullable)
        {
            name = columnName;
            column_type_string = columnType;
            is_nullable = nullable;
        }


        public SQLColumn(string columnName, string columnType, bool nullable,bool isIdentity)
        {
            name = columnName;
            column_type_string = columnType;
            is_nullable = nullable;
            is_identity = isIdentity;
        }

        public SQLColumn(SqlDataReader dr)
        {
            object_id = Convert.ToInt32(dr["object_id"]);
            name = Convert.ToString(dr["name"]);
            column_id = Convert.ToInt32(dr["column_id"]);
            system_type_id = Convert.ToInt32(dr["system_type_id"]);
            user_type_id = Convert.ToInt32(dr["user_type_id"]);
            max_length = Convert.ToInt32(dr["max_length"]);
            precision = Convert.ToInt32(dr["precision"]);
            scale = Convert.ToInt32(dr["scale"]);
            collation_name = Convert.ToString(dr["collation_name"]);
            is_ansi_padded = Convert.ToBoolean(dr["is_ansi_padded"]);
            is_rowguidcol = Convert.ToBoolean(dr["is_rowguidcol"]);
            is_identity = Convert.ToBoolean(dr["is_identity"]);
            is_computed = Convert.ToBoolean(dr["is_computed"]);
            is_filestream = Convert.ToBoolean(dr["is_filestream"]);
            is_replicated = Convert.ToBoolean(dr["is_replicated"]);
            is_non_sql_subscribed = Convert.ToBoolean(dr["is_non_sql_subscribed"]);
            is_merge_published = Convert.ToBoolean(dr["is_merge_published"]);
            is_dts_replicated = Convert.ToBoolean(dr["is_dts_replicated"]);
            is_xml_document = Convert.ToBoolean(dr["is_xml_document"]);
            xml_collection_id = Convert.ToInt32(dr["xml_collection_id"]);
            default_object_id = Convert.ToInt32(dr["default_object_id"]);
            rule_object_id = Convert.ToInt32(dr["rule_object_id"]);
            is_sparse = Convert.ToBoolean(dr["is_sparse"]);
            is_column_set = Convert.ToBoolean(dr["is_column_set"]);
            column_type_string = DB.GetSqlColumnType(system_type_id);
            is_nullable = Convert.ToBoolean(dr["is_nullable"]);
        }
    }
}
