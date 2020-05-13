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
    public class Program
    {
        public static void Main()
        {
            var rawAttributes = File.ReadAllText("config.json");
            var myJsonObject = JsonConvert.DeserializeObject<AttributeList>(rawAttributes);

            foreach (var item in myJsonObject.AttributeConfigData)
            {
                if (item.DataType.ToLower().Equals("number"))
                {
                    if (item.Location.ToLower().Equals("section"))
                    {
                        var numericAttributeData = new NumericAttributeDataCreator();
                        var result = numericAttributeData.GetNumericAttributeDatum(item);
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

    public class NumericAttributeDataCreator
    {
        public AttributeDatum<double> GetNumericAttributeDatum(ConfigFileModel item)
        {
            var sqlConnection = new SqlConnection(item.Connection.UserName, item.Connection.Password,
                            item.Connection.Server, item.Connection.DataSource);

            var numericAttribute = new NumericAttribute(item.AttributeName, sqlConnection,
                                                         Convert.ToDouble(item.DefaultValue), item.Maximum, item.Minimum);

            var sectionLocation = new SectionLocation("I dont know yet");
            var numericAttributeDatum = new AttributeDatum<double>(numericAttribute, 5, sectionLocation);

            return numericAttributeDatum;
        }
    }
}
