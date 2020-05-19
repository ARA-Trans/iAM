using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    public static class TestDataForAttribute
    {
        // Linear location data sample
        public static List<(Location location, double value)> linearLocationList
            = new List<(Location location, double value)> { (new LinearLocation(new SimpleRoute("Test route"), 0, 10), 100) };

        // Section location sample
        public static List<(Location Location, string value)> SectionLocationList
             = new List<(Location Location, string value)> { (new SectionLocation("B-0-1"), "TestValue") };

        // SQL connection sample
        public static SqlAttributeConnection SQLConnection = new SqlAttributeConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup");

        //Linear location sample
        public static LinearLocation LinearLocation = new LinearLocation(new SimpleRoute("Test route"), 0, 10);

        // Section Location sample
        public static SectionLocation SectionLocation = new SectionLocation("B-0-1");

        public static AttributeDatum<double> NumericAttributeDatumSampleOutput = new AttributeDatum<double>(
            new NumericAttribute("C", SQLConnection, 10, 100, 1), 100, LinearLocation, DateTime.Now);

        public static AttributeDatum<string> TextAttributeDatum = new AttributeDatum<string>(new TextAttribute("B", SQLConnection, "DEFAULT"), "TestValue",
            SectionLocation, DateTime.Now);
    }
}
