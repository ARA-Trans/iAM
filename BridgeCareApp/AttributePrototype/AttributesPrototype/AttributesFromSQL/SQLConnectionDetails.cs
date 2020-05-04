using System;
using System.Collections.Generic;
using System.Text;

namespace AttributesPrototype
{
    class SQLConnectionDetails : IConnectionDetails
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }

        public string Bearer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DataSource { get; set; }

        public SQLConnectionDetails(string id, string password, string server, string dataSource)
        {
            Id = id;
            Password = password;
            Server = server;
            DataSource = dataSource;
        }
    }
}
