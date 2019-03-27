using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("INVESTMENTS")]
    public class INVESTMENTS
    {
        [Key]
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }

        public Nullable<int> FIRSTYEAR { get; set; }
        public Nullable<int> NUMBERYEARS { get; set; }
        public Nullable<double> INFLATIONRATE { get; set; }
        public Nullable<double> DISCOUNTRATE { get; set; }
        public string BUDGETORDER { get; set; }

        public virtual SIMULATION SIMULATION { get; set; }
    }
}