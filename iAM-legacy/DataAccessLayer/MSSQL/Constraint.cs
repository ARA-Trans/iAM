using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataAccessLayer.MSSQL
{
    public class Constraint
    {
        public string constraint_type { get; set; }
        public string constraint_name { get; set; }
        public string delete_action { get; set; }
        public string update_action { get; set; }
        public string status_enabled { get; set; }
        public string status_for_replication { get; set; }
        public string constraint_keys { get; set; }
        public string references { get; set; }

        public Constraint()
        {
        }

        public Constraint(DataRow row)
        {
            if (row["constraint_type"] != DBNull.Value) this.constraint_type = row["constraint_type"].ToString();
            if (row["constraint_name"] != DBNull.Value) this.constraint_name = row["constraint_name"].ToString();
            if (row["delete_action"] != DBNull.Value) this.delete_action = row["delete_action"].ToString();
            if (row["update_action"] != DBNull.Value) this.update_action = row["update_action"].ToString();
            if (row["status_enabled"] != DBNull.Value) this.status_enabled = row["status_enabled"].ToString();
            if (row["status_for_replication"] != DBNull.Value) this.status_for_replication = row["status_for_replication"].ToString();
            if (row["constraint_keys"] != DBNull.Value) this.constraint_keys = row["constraint_keys"].ToString();
        }

    }
}
