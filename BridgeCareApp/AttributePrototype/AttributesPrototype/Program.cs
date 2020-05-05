using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System.Data.SqlClient;
using AttributesPrototype.AttributesFromWebAPI;

namespace AttributesPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter connection type - SQL or WebAPI");
            string connectionType = Console.ReadLine();
            // connection type - SQL, or Web API etc will come from the client
            // GetAttributeData() will have different implementation details for SQL, and Web API

            switch (connectionType)
            {
                case "SQL":
                    IConnectionDetails connectionDetails = new SQLConnectionDetails("sa", "20Pikachu^", "40.121.5.125,1433", "DbBackup");
                    IAttributeData attributes = new RawAttributeDataFromSQL();
                    attributes.GetAttributeDataChuncks(connectionDetails);
                    break;
                case "WebAPI":
                    // If a call comes from Web API

                    connectionDetails = new WebAPIConnectionDetails("tokenID", "bearer");
                    attributes = new RawAttributeDataFromWebAPI();
                    attributes.GetAttributeDataChuncks(connectionDetails);
                    break;
            }
        }
    }
}
