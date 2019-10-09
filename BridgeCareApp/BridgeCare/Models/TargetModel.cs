using System;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class TargetModel : CrudModel
    {
        public string Id { get; set; }
        public string Attribute { get; set; }
        public string Name { get; set; }
        public int? Year { get; set; }
        public double? TargetMean { get; set; }
        public string Criteria { get; set; }

        public TargetModel() { }

        public TargetModel(TargetsEntity entity)
        {
            Id = entity.ID_.ToString();
            Attribute = entity.ATTRIBUTE_;
            Name = entity.TARGETNAME;
            Year = entity.YEARS ?? DateTime.Now.Year;
            TargetMean = entity.TARGETMEAN ?? 0;
            Criteria = entity.CRITERIA;
        }

        public void UpdateTarget(TargetsEntity entity)
        {
            entity.ATTRIBUTE_ = Attribute;
            entity.TARGETNAME = Name;
            entity.YEARS = Year;
            entity.TARGETMEAN = TargetMean;
            entity.CRITERIA = Criteria;
        }
    }
}