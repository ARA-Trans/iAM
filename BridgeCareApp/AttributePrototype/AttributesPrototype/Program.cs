using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace AttributesPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            var rawAttributes = File.ReadAllText("Test.json");
            var myJsonObject = JsonConvert.DeserializeObject<AttributeModel>(rawAttributes);

            var attributeNames = new List<string>();
            foreach (var item in myJsonObject.AttributeList)
            {
                attributeNames.Add(item.attributeName);

                //your connection string 
                string connString = @"Data Source=" + item.server + ";Initial Catalog="
                            + item.DataSource + ";Persist Security Info=True;User ID=" + item.userName + ";Password=" + item.password;

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

            //var datasource = @"40.121.5.125,1433";//your server
            //var database = "DbBackup"; //your database name
            //var username = "sa"; //username of server to connect
            //var password = "20Pikachu^"; //password

            //your connection string 
            //string connString = @"Data Source=" + datasource + ";Initial Catalog="
            //            + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            ////create instanace of database connection
            //var conn = new System.Data.SqlClient.SqlConnection(connString);


            //try
            //{
            //    Console.WriteLine("Openning Connection ...");

            //    //open connection
            //    conn.Open();

            //    Console.WriteLine("Connection successful!");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Error: " + e.Message);
            //}

            Console.Read();
            //Console.WriteLine(myJsonObject.AttributeList[0].attributeName);
            //Console.ReadLine();
        }
    }
}
