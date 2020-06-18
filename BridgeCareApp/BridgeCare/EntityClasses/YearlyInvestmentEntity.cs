using BridgeCare.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.EntityClasses.CriteriaDrivenBudgets;
using Simulation;

namespace BridgeCare.EntityClasses
{
    [Table("YEARLYINVESTMENT")]
    public class YearlyInvestmentEntity
    {
        [Key]
        public int YEARID { get; set; }
        public int SIMULATIONID { get; set; }
        public int YEAR_ { get; set; }
        public int BUDGET_CRITERIA_ID { get; set; }
        public double? AMOUNT { get; set; }

        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }
        [ForeignKey("BUDGET_CRITERIA_ID")]
        public virtual CriteriaDrivenBudgetEntity BUDGET_CRITERIA { get; set; }

        public YearlyInvestmentEntity() { }

        public YearlyInvestmentEntity(int simulationId, InvestmentLibraryBudgetYearModel model)
        {
            SIMULATIONID = simulationId;
            YEAR_ = model.Year;
            AMOUNT = model.BudgetAmount;
        }

        public static void DeleteEntry(YearlyInvestmentEntity entity, BridgeCareContext db)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}
