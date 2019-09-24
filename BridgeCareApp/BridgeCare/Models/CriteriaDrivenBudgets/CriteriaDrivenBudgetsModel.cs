using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;

namespace BridgeCare.Models.CriteriaDrivenBudgets
{
    public class CriteriaDrivenBudgetsModel
    {
        public int ScenarioId { get; set; }
        public int BudgetCriteriaId { get; set; }
        public string BudgetName { get; set; }
        public string Criteria { get; set; }

        public CriteriaDrivenBudgetsModel() { }

        public CriteriaDrivenBudgetsModel(CriteriaDrivenBudgetsEntity criteriaDrivenBudgetsEntity)
        {
            BudgetCriteriaId = criteriaDrivenBudgetsEntity.BUDGET_CRITERIA_ID;
            ScenarioId = criteriaDrivenBudgetsEntity.SIMULATIONID;
            BudgetName = criteriaDrivenBudgetsEntity.BUDGET_NAME;
            Criteria = criteriaDrivenBudgetsEntity.CRITERIA;
        }
    }
}