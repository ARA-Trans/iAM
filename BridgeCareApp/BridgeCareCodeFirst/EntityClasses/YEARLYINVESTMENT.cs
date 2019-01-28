namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("YEARLYINVESTMENT")]
    public partial class YEARLYINVESTMENT
    {
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
