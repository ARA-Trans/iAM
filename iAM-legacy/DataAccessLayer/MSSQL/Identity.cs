using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataAccessLayer.MSSQL
{
    public class Identity
    {
        public string identity { get; set; }
        public string Seed { get; set; }
        public string Increment { get; set; }
        public string Not_For_Replication { get; set; }

        public bool IsIdentity
        {
            get
            {
                if (this.identity == "No identity column defined") return false;
                else return true;
            }
        }


        public Identity()
        {
        }

        public Identity(DataRow row)
        {
            if (row["Identity"] != DBNull.Value) this.identity = row["Identity"].ToString();
            if (row["Seed"] != DBNull.Value) this.Seed = row["Seed"].ToString();
            if (row["Increment"] != DBNull.Value) this.Increment = row["Increment"].ToString();
            if (row["Not For Replication"] != DBNull.Value) this.Not_For_Replication = row["Not For Replication"].ToString();
        }
    }
}
