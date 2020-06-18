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
            var rawAttributes = File.ReadAllText("metaData.json");
            var attributeMetaData = JsonConvert.DeserializeAnonymousType(rawAttributes, new { AttributeMetaData = default(List<AttributeMetaDatum>)}).AttributeMetaData;

            foreach (var item in attributeMetaData)
            {
                if (item.DataType.ToLower().Equals("number"))
                {
                    if (item.Location.ToLower().Equals("linear"))
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
        public List<AttributeDatum<double>> GetNumericAttributeDatum(AttributeMetaDatum item)
        {
            var sqlConnection = new SqlAttributeConnection(item.ConnectionString, item.DataRetrievalCommand);
            var attributeData = sqlConnection.GetData<double>();

            var numericAttribute = new NumericAttribute(item.AttributeName, sqlConnection,
                                                         Convert.ToDouble(item.DefaultValue), item.Maximum, item.Minimum);
            var numericAttributeDatum = new List<AttributeDatum<double>>();
            //var linearLocation = new LinearLocation(new SimpleRoute("Test simple route"), uniqueIdentifier, 0, 10);
            foreach (var result in attributeData)
            {
                numericAttributeDatum.Add(new AttributeDatum<double>(numericAttribute, result.value, result.location, DateTime.Now));
            }

            return numericAttributeDatum;
        }
    }
}
