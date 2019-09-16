using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataAccessLayer.MSSQL
{
    public class Column
    {
        public string Column_name { get; set; }
        public string Type { get; set; }
        public string Computed { get; set; }
        public int Length { get; set; }
        public string Prec { get; set; }
        public string Scale { get; set; }
        public string Nullable { get; set; }
        public string TrimTrailingBlanks { get; set; }
        public string FixedLenNullInSource { get; set; }
        public string Collation { get; set; }

        public Column()
        {

        }

        public Column(DataRow row)
        {
            if(row["Column_name"] != DBNull.Value)this.Column_name = row["Column_name"].ToString();
            if(row["Type"] != DBNull.Value)this.Type = row["Type"].ToString();
            if(row["Computed"] != DBNull.Value)this.Computed = row["Computed"].ToString();
            if(row["Length"] != DBNull.Value)this.Length = Convert.ToInt32(row["Length"]);
            if(row["Prec"] != DBNull.Value)this.Prec = row["Prec"].ToString();
            if(row["Scale"] != DBNull.Value)this.Scale = row["Scale"].ToString();
            if(row["Nullable"] != DBNull.Value)this.Nullable = row["Nullable"].ToString();
            if(row["TrimTrailingBlanks"] != DBNull.Value)this.TrimTrailingBlanks = row["TrimTrailingBlanks"].ToString();
            if(row["FixedLenNullInSource"] != DBNull.Value)this.FixedLenNullInSource = row["FixedLenNullInSource"].ToString();
            if (row["Collation"] != DBNull.Value) this.Collation = row["Collation"].ToString();
        }



    }
}
