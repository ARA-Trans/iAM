using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace AttributesPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            var myJsonString = File.ReadAllText("Test.json");
            var myJsonObject = JsonConvert.DeserializeObject<AttributeModel>(myJsonString);

            Console.WriteLine(myJsonObject.AttributeList[0].attributeName);
            Console.ReadLine();
        }
    }
}
