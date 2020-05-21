using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppliedResearchAssociates.iAM.DataMiner;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using ExecutableForProtptype;
using Newtonsoft.Json;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestFixture]
    public class DataMinerTests
    {
        public List<AttributeDatum<double>> Attribute1 { get; } = new List<AttributeDatum<double>>()
        {
            new AttributeDatum<double>(iAMConfiguration.C, 100, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-0-1"), 0, 1), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 200, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-1-2"), 1, 2), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 300, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-2-3"), 2, 3), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 400, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-3-4"), 3, 4), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 500, new LinearLocation(new DirectionalRoute("B", Direction.N, "B-4-5"), 4, 5), DateTime.Now),

            new AttributeDatum<double>(iAMConfiguration.C, 101, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-5-4"), 4, 5), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 201, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-4-3"), 3, 4), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 301, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-3-2"), 2, 3), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 401, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-2-1"), 1, 2), DateTime.Now),
            new AttributeDatum<double>(iAMConfiguration.C, 501, new LinearLocation(new DirectionalRoute("B", Direction.S, "B-1-0"), 0, 1), DateTime.Now),
        };

        public List<AttributeDatum<string>> Attribute2 { get; } = new List<AttributeDatum<string>>()
        {
            new AttributeDatum<string>(iAMConfiguration.B, "ALABAMA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-0-1"), 0, 1), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "ALASKA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-1-2"), 1, 2), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "ARIZONA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-2-3"), 2, 3), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "ARKANSAS", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-3-4"), 3, 4), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "CALIFORNIA", new LinearLocation(new DirectionalRoute("B", Direction.S, "B-4-5"), 4, 5), DateTime.Now),

            new AttributeDatum<string>(iAMConfiguration.B, "COLORADO", new SectionLocation("B-0-1"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "CONNECTICUT", new SectionLocation("B-1-2"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "DELEWARE", new SectionLocation("B-2-3"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "FLORIDA", new SectionLocation("B-3-4"), DateTime.Now),
            new AttributeDatum<string>(iAMConfiguration.B, "GEORGIA", new SectionLocation("B-4-5"), DateTime.Now)
        };

        public static SqlAttributeConnection SQLConnection = new SqlAttributeConnection("sa", "20Pikachu", "40.121.5.125,1433", "DbBackup");
        public static LinearLocation LinearLocation = new LinearLocation(new SimpleRoute("Test simple route"), 0, 10);

        public static AttributeDatum<double> NumericAttributeDatum = new AttributeDatum<double>(
            new NumericAttribute("ADT", SQLConnection, 10, 100, 1), 5, LinearLocation, DateTime.Now);

        public DataMinerTests()
        {
        }

        [Test]
        public void CreateAttributes()
        {
            var rawAttributes = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\config.json");
            var myJsonObject = JsonConvert.DeserializeObject<AttributeList>(rawAttributes);

            foreach (var item in myJsonObject.AttributeConfigData)
            {
                if (item.DataType.ToLower().Equals("number"))
                {
                    if (item.Location.ToLower().Equals("linear"))
                    {
                        // Arrange/ Act
                        var numericAttributeData = new NumericAttributeDataCreator();
                        var result = numericAttributeData.GetNumericAttributeDatum(item);

                        Assert.That(result.Location.ToString(), Is.EqualTo(NumericAttributeDatum.Location.ToString()));
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

        [Test]
        // This test is meant to fail right now, because "GetData()" has not been implemented for "SqlAttributeConnection" class
        public void GetNumericDataSqlConnection()
        {
            // Arrange
            var sqlAttributeConnection = new SqlAttributeConnection("sa", "20Pikachu", "52.177.117.86,1433", "DbBackup");
            // Act
            var output = sqlAttributeConnection.GetData<double>();

            // Assert
            foreach (var item in output)
            {
                Assert.That(item.value, !Is.Null);
                Assert.That(item.value, !Is.TypeOf<double>());
            }
        }

        [Test]
        public void GetTextDataSqlConnection()
        {
            // Arrange
            var sqlAttributeConnection = new SqlAttributeConnection("sa", "20Pikachu", "52.177.117.86,1433", "DbBackup");
            // Act
            var output = sqlAttributeConnection.GetData<string>();

            // Assert
            foreach (var item in output)
            {
                Assert.That(item.value, Is.TypeOf<string>());
            }
        }

        [Test]
        public void CreateNumberAttributeForLinearLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<double>.CreateAttributeData(iAMConfiguration.C, TestDataForAttribute.LinearLocationForNumberAttribut);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<double>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.NumericAttributeDataLinearLocation.Value));
            }
        }

        [Test]
        public void CreateTextAttributeForSectionLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<string>.CreateAttributeData(iAMConfiguration.B, TestDataForAttribute.SectionLocationForTextAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<string>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.TextAttributeDataSectionLocOutput.Value));
            }
        }

        [Test]
        public void CreateTextAttributeForLinearLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<string>.CreateAttributeData(iAMConfiguration.B, TestDataForAttribute.LinearLocationForTextAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<string>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.TextAttributeDataLinearLocOutput.Value));
            }
        }

        [Test]
        public void CreateNumberAttributeForSectionLocation()
        {
            // Arrange / Act
            var output = AttributeDatumBuilder<double>.CreateAttributeData(iAMConfiguration.C, TestDataForAttribute.SectionLocationForNumberAttribute);

            foreach (var item in output)
            {
                Assert.That(item.Value, Is.TypeOf<double>());
                Assert.That(item.Value, Is.EqualTo(TestDataForAttribute.NumericAttributeSectionLocOutput.Value));
            }
        }
    }
}
