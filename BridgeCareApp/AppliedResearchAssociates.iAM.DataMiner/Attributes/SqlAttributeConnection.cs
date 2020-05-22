using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppliedResearchAssociates.iAM.DataMiner.Attributes
{
    public class SqlAttributeConnection : AttributeConnection
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Server { get; set; }

        public string DataSource { get; set; }

        public string ConnectionString { get; set; }

        public SqlAttributeConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public override void Connect()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        ///     Maps data from a data source to routeName, start, end,
        ///     direction, and/or wellKnownText. The only required value from
        ///     the data set is a uniqueIdentifier
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override IEnumerable<(Location location, T value)> GetData<T>(string attributeName)
        {
            string routeName = null;
            double? start = null;
            double? end = null;
            Direction? direction = null;
            string wellKnownText = null;
            string uniqueIdentifeir = "";

            using (var conn = new SqlConnection(ConnectionString))
            {
                var query = $"Select * from {attributeName}";
                var command = new SqlCommand(query, conn);
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

            //foreach (var datum in data)
            //{
            //    string uniqueIdentifier = data.UniqueIdentifier;
            //    yield return (LocationBuilder.CreateLocation
            //        (
            //            uniqueIdentifier,
            //            routeName,
            //            start,
            //            end,
            //            direction,
            //            wellKnownText
            //        ),
            //        datum.Value);
            //}
        }
    }
}
