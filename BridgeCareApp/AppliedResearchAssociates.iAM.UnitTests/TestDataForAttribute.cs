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
        public static List<(Location location, double value)> LinearLocationForNumberAttribut
            = new List<(Location location, double value)> { (new LinearLocation(new SimpleRoute("Test route"), 0, 10), 100) };

        public static List<(Location location, string value)> LinearLocationForTextAttribute
            = new List<(Location location, string value)> { (new LinearLocation(new SimpleRoute("Test simple route"), 0, 10), "Test linear location text attribute") };

        // Section location sample
        public static List<(Location Location, string value)> SectionLocationForTextAttribute
             = new List<(Location Location, string value)> { (new SectionLocation("B-0-1"), "TestValue") };

        public static List<(Location Location, double value)> SectionLocationForNumberAttribute
            = new List<(Location Location, double value)> { (new LinearLocation(new SimpleRoute("Test simple route"), 0, 10), 100) };

        // SQL connection sample
        public static SqlAttributeConnection SQLConnection = new SqlAttributeConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup");

        //Linear location sample
        public static LinearLocation LinearLocation = new LinearLocation(new SimpleRoute("Test route"), 0, 10);

        // Section Location sample
        public static SectionLocation SectionLocation = new SectionLocation("B-0-1");

        public static AttributeDatum<double> NumericAttributeDataLinearLocation = new AttributeDatum<double>(
            new NumericAttribute("C", SQLConnection, 10, 100, 1), 100, LinearLocation, DateTime.Now);

        public static AttributeDatum<string> TextAttributeDataSectionLocOutput = new AttributeDatum<string>(new TextAttribute("B", SQLConnection, "DEFAULT"), "TestValue",
            SectionLocation, DateTime.Now);

        public static AttributeDatum<string> TextAttributeDataLinearLocOutput = new AttributeDatum<string>(new TextAttribute("B", SQLConnection, "DEFAULT"), "Test linear location text attribute",
            LinearLocation, DateTime.Now);

        public static AttributeDatum<double> NumericAttributeSectionLocOutput = new AttributeDatum<double>(
            new NumericAttribute("C", SQLConnection, 10, 100, 1), 100, SectionLocation, DateTime.Now);
    }
}
