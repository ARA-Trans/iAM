using System;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class TargetParameters
    {
        public int Id;
        public string Attribute;
        public double TargetMean;
        public string Name;
        public string Criteria;
        public int Row;

        public TargetParameters() { }

        public TargetParameters(TargetsEntity targetsEntity)
        {
            Id = targetsEntity.ID_;
            Attribute = targetsEntity.ATTRIBUTE_;
            TargetMean = targetsEntity.TARGETMEAN ?? 0;
            Name = targetsEntity.TARGETNAME;
            Criteria = targetsEntity.CRITERIA;
            Row = 0;
        }
    }
}