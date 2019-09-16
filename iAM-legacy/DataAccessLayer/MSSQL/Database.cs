using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataAccessLayer.MSSQL
{
    public class Database
    {
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Type { get; set; }
        public DateTime Created_datetime { get; set; }

        public Database()
        {
        }

        public Database(DataTable table)
        {
            DataRow row = table.Rows[0];
            this.Name = (string) row["Name"];
            this.Owner = (string)row["Owner"];
            this.Type = (string)row["Type"];
            this.Created_datetime = Convert.ToDateTime(row["Created_datetime"]);
        }
    }
}
