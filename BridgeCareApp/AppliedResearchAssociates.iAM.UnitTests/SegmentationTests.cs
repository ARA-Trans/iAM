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
        public void CreateNumericSegmentForLinearLocation()
        {
            // Arrange
            var attributeTestData = new List<AttributeDatum<double>>();
            // Act
            attributeTestData.Add(TestDataForAttribute.NumericAttributeDataLinearLocation);
            attributeTestData.Add(TestDataForAttribute.NumericAttributeDataLinearLocation_2);
            var segmentedData = Segmenter.CreateSegmentsFromAttributeDataRecords(attributeTestData);
            //Assert

            Assert.That(segmentedData.Count, Is.EqualTo(attributeTestData.Count));
            foreach (var item in segmentedData)
            {
                Assert.That(item.SegmentationAttributeDatum.Value, Is.Not.Null);
                Assert.That(item.SegmentationAttributeDatum.Value, Is.TypeOf<double>());
            }
        }

        [Test]
        public void CreateNumericSegmentForSectionLocation()
        {
            // Arrange
            var attributeTestData = new List<AttributeDatum<double>>();
            // Act
            attributeTestData.Add(TestDataForAttribute.NumericAttributeSectionLocOutput);
            attributeTestData.Add(TestDataForAttribute.NumericAttributeSectionLocOutput_2);
            var segmentedData = Segmenter.CreateSegmentsFromAttributeDataRecords(attributeTestData);
            //Assert

            Assert.That(segmentedData.Count, Is.EqualTo(attributeTestData.Count));
            foreach (var item in segmentedData)
            {
                Assert.That(item.SegmentationAttributeDatum.Value, Is.Not.Null);
                Assert.That(item.SegmentationAttributeDatum.Value, Is.TypeOf<double>());
            }
        }

        [Test]
        public void CreateTextSegmentForLinearLocation()
        {
            // Arrange
            var attributeTestData = new List<AttributeDatum<string>>();
            // Act
            attributeTestData.Add(TestDataForAttribute.TextAttributeDataLinearLocOutput);
            attributeTestData.Add(TestDataForAttribute.TextAttributeDataLinearLocOutput_2);
            var segmentedData = Segmenter.CreateSegmentsFromAttributeDataRecords(attributeTestData);
            //Assert

            Assert.That(segmentedData.Count, Is.EqualTo(attributeTestData.Count));
            foreach (var item in segmentedData)
            {
                Assert.That(item.SegmentationAttributeDatum.Value, Is.Not.Null);
                Assert.That(item.SegmentationAttributeDatum.Value, Is.TypeOf<string>());
            }
        }

        [Test]
        public void CreateTextSegmentForSectionLocation()
        {
            // Arrange
            var attributeTestData = new List<AttributeDatum<string>>();
            // Act
            attributeTestData.Add(TestDataForAttribute.TextAttributeDataSectionLocOutput);
            attributeTestData.Add(TestDataForAttribute.TextAttributeDataSectionLocOutput_2);
            var segmentedData = Segmenter.CreateSegmentsFromAttributeDataRecords(attributeTestData);
            //Assert

            Assert.That(segmentedData.Count, Is.EqualTo(attributeTestData.Count));
            foreach (var item in segmentedData)
            {
                Assert.That(item.SegmentationAttributeDatum.Value, Is.Not.Null);
                Assert.That(item.SegmentationAttributeDatum.Value, Is.TypeOf<string>());
            }
        }
    }
}
