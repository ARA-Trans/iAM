using BridgeCare.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace BridgeCare.EntityClasses
{
    [Table("YEARLYINVESTMENT")]
    public class YearlyInvestmentEntity
    {
        [Key]
        public int YEARID { get; set; }
        public int SIMULATIONID { get; set; }
        public int YEAR_ { get; set; }
        [Required]
        [StringLength(50)]
        public string BUDGETNAME { get; set; }
        public double? AMOUNT { get; set; }

        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }

        public YearlyInvestmentEntity() { }

        public YearlyInvestmentEntity(int simulationId, InvestmentLibraryBudgetYearModel model)
        {
            SIMULATIONID = simulationId;
            YEAR_ = model.Year;
            BUDGETNAME = model.BudgetName;
            AMOUNT = model.BudgetAmount;
        }

        public static void DeleteEntry(YearlyInvestmentEntity entity, BridgeCareContext db)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}