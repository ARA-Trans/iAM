using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SqlAttributeConnection : AttributeConnection
    {
        public override string ConnectionInformation { get; }

        public override string DataRetrievalCommand { get; }

        public SqlAttributeConnection(string connectionInformation, string dataRetrievalCommand)
        {
            ConnectionInformation = connectionInformation;
            DataRetrievalCommand = dataRetrievalCommand;
        }
                
        public override IEnumerable<(Location location, T value)> GetData<T>()
        {
            string routeName = null;
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            string uniqueIdentifeir = "";

            using (var conn = new SqlConnection(ConnectionInformation))
            {
                var command = new SqlCommand(DataRetrievalCommand, conn);
                command.Connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    routeName = dataReader.GetFieldValue<string>(1);
                    start = dataReader.GetFieldValue<double>(2);
                    end = dataReader.GetFieldValue<double>(3);
                    var rawDirection = dataReader.GetFieldValue<string>(4);
                    if(rawDirection == "North")
                    {
                        direction = 0;
                    }
                    var value = dataReader.GetFieldValue<T>(5);
                    var dataTime = dataReader.GetFieldValue<DateTime>(6);

                    yield return (LocationBuilder.CreateLocation(
                        uniqueIdentifeir, routeName, start, end, direction, wellKnownText)
                        , value);
                }
            }
        }
    }
}
