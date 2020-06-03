using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataMiner.Attributes;
using AppliedResearchAssociates.iAM.Segmentation;
using NUnit.Framework;

namespace AppliedResearchAssociates.iAM.UnitTests
{
    [TestFixture]
    public class SegmentationTests
    {
        [Test]
        public void CreateNUmericSegmentForLinearLocation()
        {
            // Arrange
            var attributeTestData = new List<AttributeDatum<double>>();
            // Act
            attributeTestData.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
            attributeTestData.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
            var segmentedData = Segmenter.CreateSegmentsFromAttributeDataRecords(attributeTestData);
            //Assert
            foreach (var item in segmentedData)
            {
                Assert.That(item.SegmentationAttributeDatum.Value, !Is.Null);
                Assert.That(item.SegmentationAttributeDatum.Value, Is.TypeOf<double>());
            }
        }
    }
}
