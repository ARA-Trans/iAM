using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class SplitTreatmentModel : CrudModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public List<SplitTreatmentLimitModel> SplitTreatmentLimits { get; set; }

        public SplitTreatmentModel() { }

        public SplitTreatmentModel(SplitTreatmentEntity entity)
        {
            Id = entity.SPLIT_TREATMENT_ID.ToString();
            Description = entity.DESCRIPTION;
            Criteria = entity.CRITERIA;
            SplitTreatmentLimits = entity.SPLIT_TREATMENT_LIMITS.Any()
                ? entity.SPLIT_TREATMENT_LIMITS.Select(stl => new SplitTreatmentLimitModel(stl)).ToList()
                : new List<SplitTreatmentLimitModel>();
        }

        public void UpdateSplitTreatment(SplitTreatmentEntity entity)
        {
            entity.DESCRIPTION = Description;
            entity.CRITERIA = Criteria;
        }
    }
}