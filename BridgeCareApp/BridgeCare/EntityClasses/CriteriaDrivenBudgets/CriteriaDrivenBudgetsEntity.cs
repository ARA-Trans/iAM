using BridgeCare.Models.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BridgeCare.EntityClasses.CriteriaDrivenBudgets
{
    [Table("BUDGET_CRITERIA")]
    public class CriteriaDrivenBudgetsEntity
    {
        [Key]
        public int BUDGET_CRITERIA_ID { get; set; }
        public int SIMULATIONID { get; set; }
        public string BUDGET_NAME { get; set; }
        public string CRITERIA { get; set; }

        public CriteriaDrivenBudgetsEntity() { }
        public CriteriaDrivenBudgetsEntity(CriteriaDrivenBudgetsModel criteriaModel)
        {
            SIMULATIONID = criteriaModel.ScenarioId;
            BUDGET_NAME = criteriaModel.BudgetName;
            CRITERIA = criteriaModel.Criteria;
            CRITERIA = criteriaModel.Criteria;
        }
    }
}