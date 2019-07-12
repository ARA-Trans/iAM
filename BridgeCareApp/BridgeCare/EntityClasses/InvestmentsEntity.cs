using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("INVESTMENTS")]
    public class InvestmentsEntity
    {
        [Key]
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public int? FIRSTYEAR { get; set; }
        public int? NUMBERYEARS { get; set; }
        public double? INFLATIONRATE { get; set; }
        public double? DISCOUNTRATE { get; set; }
        public string BUDGETORDER { get; set; }

        public virtual SimulationEntity SIMULATION { get; set; }
    }
}