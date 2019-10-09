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

        public TreatmentModel()
        {
            Costs = new List<CostModel>();
            Consequences = new List<ConsequenceModel>();
            Budgets = new List<string>();
        }

        public TreatmentModel(TreatmentsEntity entity)
        {
            Id = entity.TREATMENTID.ToString();
            Name = entity.TREATMENT;
            Costs = entity.COSTS != null && entity.COSTS.Any()
                ? entity.COSTS.ToList().Select(c => new CostModel(c)).ToList()
                : new List<CostModel>();
            Consequences = entity.CONSEQUENCES != null && entity.CONSEQUENCES.Any()
                ? entity.CONSEQUENCES.ToList().Select(c => new ConsequenceModel(c)).ToList()
                : new List<ConsequenceModel>();
            Budgets = entity.BUDGET != null
                ? entity.BUDGET.Split(',').ToList()
                : new List<string>();
            Feasibility = null;

            if (entity.FEASIBILITIES != null && entity.FEASIBILITIES.Any())
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