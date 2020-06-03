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
        [Test]
        public void AverageAggregationRuleTest()
        {
            var averageRule = new AverageAggregationRule();
            var datumList = new List<AttributeDatum<double>>();
            // Arrange
            datumList.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
            datumList.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
            // Act
            var resultSet = averageRule.Apply(datumList);
            // Assert
            foreach (var item in resultSet)
            {
                // Item1 is year and Item2 is average value
                Assert.That(item.Item1, Is.TypeOf<int>());
                Assert.That(item.Item2, Is.TypeOf<double>());
            }
        }
    }
}
