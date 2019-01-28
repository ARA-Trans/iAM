namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SIMULATIONS")]
    public partial class SIMULATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SIMULATION()
        {
            YEARLYINVESTMENTs = new HashSet<YEARLYINVESTMENT>();
        }

        public int SIMULATIONID { get; set; }

        public int? NETWORKID { get; set; }

        [Column("SIMULATION")]
        [StringLength(50)]
        public string SIMULATION1 { get; set; }

        public string COMMENTS { get; set; }

        public DateTime? DATE_CREATED { get; set; }

        public DateTime? DATE_LAST_RUN { get; set; }

        [StringLength(50)]
        public string CREATOR_ID { get; set; }

        [StringLength(200)]
        public string USERNAME { get; set; }

        public int? PERMISSIONS { get; set; }

        public string JURISDICTION { get; set; }

        [StringLength(50)]
        public string ANALYSIS { get; set; }

        [StringLength(50)]
        public string BUDGET_CONSTRAINT { get; set; }

        [StringLength(50)]
        public string WEIGHTING { get; set; }

        [StringLength(50)]
        public string BENEFIT_VARIABLE { get; set; }

        public double? BENEFIT_LIMIT { get; set; }

        public int? COMMITTED_START { get; set; }

        public int? COMMITTED_PERIOD { get; set; }

        public string SIMULATION_VARIABLES { get; set; }

        [StringLength(50)]
        public string RUN_TIME { get; set; }

        public bool? USE_CONDITIONAL_RSL { get; set; }

        public bool? USE_CUMULATIVE_COST { get; set; }

        public virtual NETWORK NETWORK { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YEARLYINVESTMENT> YEARLYINVESTMENTs { get; set; }
    }
}
