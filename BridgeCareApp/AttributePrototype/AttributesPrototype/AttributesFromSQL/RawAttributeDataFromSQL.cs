using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace AttributesPrototype
{
    class RawAttributeDataFromSQL : IAttributeData
    {

        public void GetAttributeDataChuncks(IConnectionDetails connectionDetails)
        {
            var rawAttributes = File.ReadAllText("Test.json");
            var myJsonObject = JsonConvert.DeserializeObject<AttributeModel>(rawAttributes);

            var attributeNames = new List<string>();
            foreach (var item in myJsonObject.AttributeList)
            {
                attributeNames.Add(item.attributeName);

                //your connection string 
                string connString = @"Data Source=" + connectionDetails.Server + ";Initial Catalog="
                            + connectionDetails.DataSource + ";Persist Security Info=True;User ID=" + connectionDetails.Id + ";Password=" + connectionDetails.Password;

                //create instanace of database connection
                var connection = new System.Data.SqlClient.SqlConnection(connString);

                Console.WriteLine("Openning Connection ...");

                var queryString = $"SELECT {item.attributeName} from PennDOT_Report_A";
                var command = new System.Data.SqlClient.SqlCommand(queryString, connection);

                //open connection
                connection.Open();

                var reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}",
                        reader[0]));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                Console.WriteLine("Connection successful!");
            }
            Console.Read();
        }
    }
}
