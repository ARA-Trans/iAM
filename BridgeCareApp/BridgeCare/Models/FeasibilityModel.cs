using System.Runtime.Serialization;
using BridgeCare.EntityClasses;

namespace BridgeCare.Models
{
    public class FeasibilityModel : CrudModel
    {
        public string Id { get; set; }
        public string Criteria { get; set; }
        public int YearsBeforeAny { get; set; }
        public int YearsBeforeSame { get; set; }

        public FeasibilityModel()
        {
            Id = null;
            Criteria = "";
            YearsBeforeAny = 1;
            YearsBeforeSame = 1;
        }

        public FeasibilityModel(FeasibilityEntity feasibilityEntity, TreatmentsEntity treatmentsEntity)
        {
            Id = feasibilityEntity.FEASIBILITYID.ToString();
            Criteria = feasibilityEntity.CRITERIA;
            YearsBeforeAny = treatmentsEntity.BEFOREANY;
            YearsBeforeSame = treatmentsEntity.BEFORESAME;
        }

        public void Aggregate(FeasibilityModel model)
        {
            YearsBeforeAny = model.YearsBeforeAny;
            YearsBeforeSame = model.YearsBeforeSame;

            if (Id == null)
                Id = model.Id;

            if (!string.IsNullOrEmpty(model.Criteria))
            {
                var modelCriteria = model.Criteria;

                if (modelCriteria.Substring(0, 1) != "(")
                    modelCriteria = $"({modelCriteria}";

                if (modelCriteria.Substring(modelCriteria.Length - 1) != ")")
                    modelCriteria = $"{modelCriteria})";

                if (Criteria == null || Criteria.Length <= 0)
                    Criteria = modelCriteria;
                else
                    Criteria += $" OR {modelCriteria}";
            }
        }
    }
}