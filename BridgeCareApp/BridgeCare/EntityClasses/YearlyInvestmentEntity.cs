namespace BridgeCare.EntityClasses
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("YEARLYINVESTMENT")]
    public class YearlyInvestmentEntity
    {
        public YearlyInvestmentEntity() { }

        public YearlyInvestmentEntity(int simulationId, int year, string budgetName, double? amount)
        {
            AMOUNT = amount;
            BUDGETNAME = budgetName;
            YEAR_ = year;
            SIMULATIONID = simulationId;
        }

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
    }
}