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

        public SqlAttributeConnection(string userName, string password, string server, string dataSource)
        {
            UserName = userName;
            Password = password;
            Server = server;
            DataSource = dataSource;
        }

        public override void Connect()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<(Location location, T value)> GetData<T>(string attributeName)
        {
            var connectionString = $"data source={Server};initial catalog={DataSource};persist security info=True;" +
                $"user id={UserName};password={Password};MultipleActiveResultSets=True";

            IEnumerable<(Location location, T value)> data = new List<(Location location, T value)>();

            using (var conn = new SqlConnection(connectionString))
            {
                var query = $"Select * from {attributeName}";
                var command = new SqlCommand(query, conn);
                command.Connection.Open();
                var dataReader = command.ExecuteReader();
                while (dataReader.Read())
                {
                    var route = dataReader.GetFieldValue<string>(1);
                    var beginMilePost = dataReader.GetFieldValue<double>(2);
                    var endMilePost = dataReader.GetFieldValue<double>(3);
                    var direction = dataReader.GetFieldValue<string>(4);
                    var value = dataReader.GetFieldValue<double>(5);
                    var dataTime = dataReader.GetFieldValue<DateTime>(6);
                }
            }
            return data;
        }
    }
}
