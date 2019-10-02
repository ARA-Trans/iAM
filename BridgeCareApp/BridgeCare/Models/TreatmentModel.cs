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

        public TreatmentModel(TreatmentsEntity entity)
        {
            Id = entity.TREATMENTID.ToString();
            Name = entity.TREATMENT;
            Costs = entity.COSTS.Any()
                ? entity.COSTS.Select(c => new CostModel(c)).ToList()
                : new List<CostModel>();
            Consequences = entity.CONSEQUENCES.Any()
                ? entity.CONSEQUENCES.Select(c => new ConsequenceModel(c)).ToList()
                : new List<ConsequenceModel>();
            Budgets = entity.BUDGET != null
                ? entity.BUDGET.Split(',').ToList()
                : new List<string>();

            if (!entity.FEASIBILITIES.Any())
                Feasibility = null;
            else
            {
                Feasibility = new FeasibilityModel();
                entity.FEASIBILITIES.ToList().ForEach(feasibilityEntity => {
                    var feasibilityModel = new FeasibilityModel(feasibilityEntity, entity);
                    Feasibility.Aggregate(feasibilityModel);
                });
            }
        }

        public void UpdateTreatment(TreatmentsEntity entity)
        {
            entity.TREATMENT = Name;
            entity.BUDGET = string.Join(",", Budgets);
            if (Feasibility != null)
            {
                entity.BEFOREANY = Feasibility.YearsBeforeAny;
                entity.BEFORESAME = Feasibility.YearsBeforeSame;
            }
        }
    }
}