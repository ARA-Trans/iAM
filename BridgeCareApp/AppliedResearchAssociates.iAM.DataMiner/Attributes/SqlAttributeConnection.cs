using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SqlAttributeConnection : AttributeConnection
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string DataSource { get; set; }

        public SqlAttributeConnection(string userName, string password, string server, string dataSource)
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
            string routeName = null;
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            string uniqueIdentifier = null;


            foreach(var datum in data)
            {
                yield return (LocationBuilder.CreateLocation
                    (
                        routeName,
                        start,
                        end,
                        direction,
                        uniqueIdentifier,
                        wellKnownText
                    ),
                    datum.Value);
            }

            
        }
    }
}
