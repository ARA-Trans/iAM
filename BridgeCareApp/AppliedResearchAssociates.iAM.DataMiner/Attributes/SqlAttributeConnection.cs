using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SqlConnection : AttributeConnection
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string DataSource { get; set; }

        public SqlConnection(string userName, string password, string server, string dataSource)
        {
            UserName = userName;
            Password = password;
            Server = server;
            DataSource = dataSource;
        }

        public override void Connect()
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<(Location location, T value)> GetData<T>()
        {
            throw new NotImplementedException();
        }
    }
}
