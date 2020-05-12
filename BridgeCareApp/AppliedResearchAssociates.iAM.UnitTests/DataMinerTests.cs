using System.Collections.Generic;
using System.IO;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataMiner.NetworkDefinition;
using ExecutableForProtptype;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestClass]
    public class DataMinerTests
    {
        public List<NumericAttributeDatum> Attribute1 { get; } = new List<NumericAttributeDatum>()
        {
            new NumericAttributeDatum(iAMConfiguration.C, 100, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-0-1"), 0, 1)),
            new NumericAttributeDatum(iAMConfiguration.C, 200, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-1-2"), 1, 2)),
            new NumericAttributeDatum(iAMConfiguration.C, 300, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-2-3"), 2, 3)),
            new NumericAttributeDatum(iAMConfiguration.C, 400, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-3-4"), 3, 4)),
            new NumericAttributeDatum(iAMConfiguration.C, 500, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-4-5"), 4, 5)),

            new NumericAttributeDatum(iAMConfiguration.C, 101, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-5-4"), 4, 5)),
            new NumericAttributeDatum(iAMConfiguration.C, 201, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-4-3"), 3, 4)),
            new NumericAttributeDatum(iAMConfiguration.C, 301, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-3-2"), 2, 3)),
            new NumericAttributeDatum(iAMConfiguration.C, 401, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-2-1"), 1, 2)),
            new NumericAttributeDatum(iAMConfiguration.C, 501, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-1-0"), 0, 1)),
        };

        public List<TextAttributeDatum> Attribute2 { get; } = new List<TextAttributeDatum>()
        {
            new TextAttributeDatum(iAMConfiguration.B, "ALABAMA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-0-1"), 0, 1)),
            new TextAttributeDatum(iAMConfiguration.B, "ALASKA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-1-2"), 1, 2)),
            new TextAttributeDatum(iAMConfiguration.B, "ARIZONA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-2-3"), 2, 3)),
            new TextAttributeDatum(iAMConfiguration.B, "ARKANSAS", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-3-4"), 3, 4)),
            new TextAttributeDatum(iAMConfiguration.B, "CALIFORNIA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-4-5"), 4, 5)),

            new TextAttributeDatum(iAMConfiguration.B, "COLORADO", new SectionLocation("B-0-1")),
            new TextAttributeDatum(iAMConfiguration.B, "CONNECTICUT", new SectionLocation("B-1-2")),
            new TextAttributeDatum(iAMConfiguration.B, "DELEWARE", new SectionLocation("B-2-3")),
            new TextAttributeDatum(iAMConfiguration.B, "FLORIDA", new SectionLocation("B-3-4")),
            new TextAttributeDatum(iAMConfiguration.B, "GEORGIA", new SectionLocation("B-4-5"))
        };

        public static SqlConnection SQLConnection = new SqlConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup");
        public static SectionLocation SectionLocation = new SectionLocation("I dont know yet");

        public static NumericAttributeDatum NumericAttributeDatum = new NumericAttributeDatum(
            new NumericAttribute("INSPTYPE", new AttributeConnection(SQLConnection), 10, 100, 1), 5, SectionLocation);

        //public Network NetworkDefinition { get; } = new Network();

        public DataMinerTests()
        {
        }

        [TestMethod]
        public void CreateAttributes()
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

                        Assert.AreEqual(result.Location.ToString(), NumericAttributeDatum.Location.ToString());
                        Assert.AreEqual(result.Attribute.Name, NumericAttributeDatum.Attribute.Name);
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
