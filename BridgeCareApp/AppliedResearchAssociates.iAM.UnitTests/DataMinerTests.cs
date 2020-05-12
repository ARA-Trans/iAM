using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.DataMiner.NetworkDefinition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        public Network NetworkDefinition { get; } = new Network();

        public DataMinerTests()
        {
        }

        [TestMethod]
        public void CreateAttributes()
        {
            
        }
    }
}
