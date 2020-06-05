using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;

namespace BridgeCare.Models.CriteriaDrivenBudgets
{
    public class CriteriaDrivenBudgetModel : CrudModel
    {
        public string Id { get; set; }
        public string BudgetName { get; set; }
        public string Criteria { get; set; }

        public CriteriaDrivenBudgetModel() { }

        public CriteriaDrivenBudgetModel(CriteriaDrivenBudgetEntity criteriaDrivenBudgetEntity)
        {
            Id = criteriaDrivenBudgetEntity.BUDGET_CRITERIA_ID.ToString();
            BudgetName = criteriaDrivenBudgetEntity.BUDGET_NAME;
            Criteria = criteriaDrivenBudgetEntity.CRITERIA;
        }

        public void UpdateCriteriaDrivenBudget(CriteriaDrivenBudgetEntity entity)
        {
            entity.BUDGET_NAME = BudgetName;
            entity.CRITERIA = Criteria;
        }
    }
}
