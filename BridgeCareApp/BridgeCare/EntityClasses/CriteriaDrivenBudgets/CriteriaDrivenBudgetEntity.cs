using BridgeCare.Models.CriteriaDrivenBudgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BridgeCare.EntityClasses.CriteriaDrivenBudgets
{
    [Table("BUDGET_CRITERIA")]
    public class CriteriaDrivenBudgetEntity
    {
        [Key]
        public int BUDGET_CRITERIA_ID { get; set; }
        public int SIMULATIONID { get; set; }
        public string BUDGET_NAME { get; set; }
        public string CRITERIA { get; set; }
        public virtual ICollection<YearlyInvestmentEntity> YEARLYINVESTMENTS { get; set; }

        public CriteriaDrivenBudgetEntity() { }

        public CriteriaDrivenBudgetEntity(int simulationId, CriteriaDrivenBudgetModel model)
        {
            SIMULATIONID = simulationId;
            BUDGET_NAME = model.BudgetName;
            CRITERIA = model.Criteria;
        }

        public static void DeleteEntry(CriteriaDrivenBudgetEntity entity, BridgeCareContext db)
        {
            if (entity.YEARLYINVESTMENTS.Any())
            {
                entity.YEARLYINVESTMENTS.ToList().ForEach(yi =>
                {
                    YearlyInvestmentEntity.DeleteEntry(yi, db);
                });
            }

            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}
