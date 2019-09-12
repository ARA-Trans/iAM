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

        public TargetModel(TargetsEntity target)
        {
            Id = target.ID_.ToString();
            Attribute = target.ATTRIBUTE_;
            Name = target.TARGETNAME;
            Year = target.YEARS ?? DateTime.Now.Year;
            TargetMean = target.TARGETMEAN ?? 0;
            Criteria = target.CRITERIA;
        }

        public void UpdateTarget(TargetsEntity target)
        {
            target.ATTRIBUTE_ = Attribute;
            target.TARGETNAME = Name;
            target.YEARS = Year;
            target.TARGETMEAN = TargetMean;
            target.CRITERIA = Criteria;
        }
    }
}