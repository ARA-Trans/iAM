using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class SplitTreatmentLimitModel : CrudModel
    {
        public string Id { get; set; }
        public int? Rank { get; set; }
        public double? Amount { get; set; }
        public string Percentage { get; set; }
        public SplitTreatmentLimitModel() { }

        public SplitTreatmentLimitModel(SplitTreatmentLimitEntity entity)
        {
            Id = entity.SPLIT_TREATMENT_LIMIT_ID.ToString();
            Rank = entity.RANK;
            Amount = entity.AMOUNT;
            Percentage = entity.PERCENTAGE;
        }

        public void UpdateSplitTreatment(SplitTreatmentLimitEntity entity)
        {
            entity.RANK = Rank;
            entity.AMOUNT = Amount;
            entity.PERCENTAGE = Percentage;
        }
    }
}
