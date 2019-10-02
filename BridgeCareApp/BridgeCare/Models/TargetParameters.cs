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

        public TargetParameters(TargetsEntity entity)
        {
            Id = entity.ID_;
            Attribute = entity.ATTRIBUTE_;
            TargetMean = entity.TARGETMEAN ?? 0;
            Name = entity.TARGETNAME;
            Criteria = entity.CRITERIA;
            Row = 0;
        }
    }
}