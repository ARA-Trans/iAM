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
            var rawBudgets = entity.BUDGET != null
                ? entity.BUDGET.Split(',').ToList()
                : new List<string>();
            Budgets = new List<string>();
            if (rawBudgets.Count > 0)
            {
                rawBudgets.ForEach(item => {
                    Budgets.Add(item.Trim());
                });
            }
            Feasibility = new FeasibilityModel();

            if (entity.FEASIBILITIES != null && entity.FEASIBILITIES.Any())
            {
                var feasibilities = entity.FEASIBILITIES.ToList();

                feasibilities.ForEach(feasibilityEntity => {
                    var feasibilityModel = new FeasibilityModel(feasibilityEntity, entity);

                    if (feasibilities.Count > 1)
                        Feasibility.Aggregate(feasibilityModel);
                    else
                        Feasibility = feasibilityModel;
                });
            }
        }

        public void UpdateTreatment(TreatmentsEntity entity)
        {
            entity.TREATMENT = Name;
            entity.BUDGET = Budgets.Count > 0 ? string.Join(",", Budgets) : null;
            entity.BEFOREANY = Feasibility.YearsBeforeAny;
            entity.BEFORESAME = Feasibility.YearsBeforeSame;
        }
    }
}