using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using Newtonsoft.Json;

namespace ExecutableForProtptype
{
    class Program
    {
        static void Main(string[] args)
        {
            var rawAttributes = File.ReadAllText("config.json");
            var myJsonObject = JsonConvert.DeserializeObject<AttributeList>(rawAttributes);

            foreach (var item in myJsonObject.AttributeConfigData)
            {
                if (item.DataType.ToLower().Equals("number"))
                {
                    if (item.Location.ToLower().Equals("section"))
                    {
                        var sqlConnection = new SQLConnection(item.Connection.UserName, item.Connection.Password,
                            item.Connection.Server, item.Connection.DataSource);

                        var numericAttribute = new NumericAttribute(item.AttributeName, new AttributeConnection(sqlConnection),
                                                                     Convert.ToDouble(item.DefaultValue), item.Maximum, item.Minimum);

                        var sectionLocation = new SectionLocation("I dont know yet");
                        var numericAttributeDatum = new NumericAttributeDatum(numericAttribute, 5, sectionLocation);
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (item.Location.ToLower().Equals("section"))
                    {

                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
