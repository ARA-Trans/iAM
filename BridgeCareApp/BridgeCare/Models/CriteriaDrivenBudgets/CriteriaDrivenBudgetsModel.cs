using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.CriteriaDrivenBudgets
{
    public class CriteriaDrivenBudgetsModel
    {
        public int ScenarioId { get; set; }
        public int BudgetCriteriaId { get; set; }
        public string BudgetName { get; set; }
        public string Criteria { get; set; }
    }
}