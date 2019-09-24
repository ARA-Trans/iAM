using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using BridgeCare.EntityClasses;
using BridgeCare.Services;


namespace BridgeCare.Models
{
    public class TreatmentModel : CrudModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public FeasibilityModel Feasibility { get; set; }
        public List<CostModel> Costs { get; set; }
        public List<ConsequenceModel> Consequences { get; set; }
        public List<string> Budgets { get; set; }

        public TreatmentModel() { }

        public TreatmentModel(TreatmentsEntity treatmentsEntity)
        {
            Id = treatmentsEntity.TREATMENTID.ToString();
            Name = treatmentsEntity.TREATMENT;
            Costs = treatmentsEntity.COSTS.Any()
                ? treatmentsEntity.COSTS.Select(c => new CostModel(c)).ToList()
                : new List<CostModel>();
            Consequences = treatmentsEntity.CONSEQUENCES.Any()
                ? treatmentsEntity.CONSEQUENCES.Select(c => new ConsequenceModel(c)).ToList()
                : new List<ConsequenceModel>();
            Budgets = treatmentsEntity.BUDGET != null
                ? treatmentsEntity.BUDGET.Split(',').ToList()
                : new List<string>();

            if (!treatmentsEntity.FEASIBILITIES.Any())
                Feasibility = null;
            else
            {
                Feasibility = new FeasibilityModel();
                treatmentsEntity.FEASIBILITIES.ToList().ForEach(feasibilityEntity => {
                    var feasibilityModel = new FeasibilityModel(feasibilityEntity, treatmentsEntity);
                    Feasibility.Aggregate(feasibilityModel);
                });
            }
        }

        public void UpdateTreatment(TreatmentsEntity treatmentsEntity)
        {
            treatmentsEntity.TREATMENT = Name;
            treatmentsEntity.BUDGET = string.Join(",", Budgets);
            if (Feasibility != null)
            {
                treatmentsEntity.BEFOREANY = Feasibility.YearsBeforeAny;
                treatmentsEntity.BEFORESAME = Feasibility.YearsBeforeSame;
            }
        }
    }
}