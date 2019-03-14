namespace BridgeCare.EntityClasses
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("YEARLYINVESTMENT")]
    public partial class YEARLYINVESTMENT
    {
        public YEARLYINVESTMENT(int simulationID, int year, string budgetName, double? amount)
        {
            AMOUNT = amount;
            BUDGETNAME = budgetName;
            YEAR_ = year;
            SIMULATIONID = simulationID;
        }

        [Key]
        public int YEARID { get; set; }

        public int SIMULATIONID { get; set; }

        public int YEAR_ { get; set; }

        [Required]
        [StringLength(50)]
        public string BUDGETNAME { get; set; }

        public double? AMOUNT { get; set; }

        public virtual SIMULATION SIMULATION { get; set; }
    }
}