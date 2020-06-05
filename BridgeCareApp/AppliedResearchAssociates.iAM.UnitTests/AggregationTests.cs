using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Aggregation;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestFixture]
    public class AggregationTests
    {
        private static List<(int, double)> ExpectedResult = new List<(int, double)> { (2020, 101.5), (2019, 115) };
        [Test]
        public void AverageAggregationLinearRuleTest()
        {
            var averageRule = new AverageAggregationRule();
            var data = new List<AttributeDatum<double>>();
            // Arrange
            data.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
            data.Add(TestDataForAttribute.NumericAttributeDataLinearLocation_2);
            data.Add(TestDataForAttribute.NumericAttributeDataLinearLocation_a);
            // Act
            var distinctYears = data.Select(_ => _.TimeStamp.Year).Distinct();
            var resultSet = averageRule.Apply(data);
            // Assert
            foreach (var item in resultSet)
            {
                // Item1 is year and Item2 is average value
                var expectedAverage = ExpectedResult.Where(_ => _.Item1 == item.Item1).Select(s => s.Item2).FirstOrDefault();
                Assert.That(item.Item2, Is.EqualTo(expectedAverage));
                // make sure the actual average is expected attribute
            }
            Assert.That(resultSet.Count(), Is.EqualTo(distinctYears.Count()));
        }

        [Test]
        public void AverageAggregationSectionRuleTest()
        {
            var averageRule = new AverageAggregationRule();
            var data = new List<AttributeDatum<double>>();
            // Arrange
            data.Add(TestDataForAttribute.NumericAttributeSectionLocOutput);
            data.Add(TestDataForAttribute.NumericAttributeSectionLocOutput_2);
            data.Add(TestDataForAttribute.NumericAttributeSectionLocOutput_a);
            // Act
            var distinctYears = data.Select(_ => _.TimeStamp.Year).Distinct();
            var resultSet = averageRule.Apply(data);
            // Assert
            foreach (var item in resultSet)
            {
                // Item1 is year and Item2 is average value
                var expectedAverage = ExpectedResult.Where(_ => _.Item1 == item.Item1).Select(s => s.Item2).FirstOrDefault();
                Assert.That(item.Item2, Is.EqualTo(expectedAverage));
            }
            Assert.That(resultSet.Count(), Is.EqualTo(distinctYears.Count()));
        }

        [Test]
        public void PredominantAggregationLinearRuleTest()
        {
            var predominantRule = new PredominantAggregationRule();
            // Arrange
            var data = new List<AttributeDatum<string>>
            {
                TestDataForAttribute.TextAttributeDataLinearLocOutput,
                TestDataForAttribute.TextAttributeDataLinearLocOutput_2
            };
            // Act
            var resultSet = predominantRule.Apply(data);

            // Assert
            Assert.That(resultSet.Count(), Is.EqualTo(data.Count));
        }

        [Test]
        public void PredominantAggregationSectionRuleTest()
        {
            var predominantRule = new PredominantAggregationRule();
            // Arrange
            var data = new List<AttributeDatum<string>>
            {
                TestDataForAttribute.TextAttributeDataSectionLocOutput,
                TestDataForAttribute.TextAttributeDataSectionLocOutput_2
            };
            // Act
            var resultSet = predominantRule.Apply(data);

            // Assert
            Assert.That(resultSet.Count(), Is.EqualTo(data.Count));
        }

    }
}
