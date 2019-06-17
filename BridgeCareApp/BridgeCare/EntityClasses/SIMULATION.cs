using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("SIMULATIONS")]
    public partial class SIMULATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SIMULATION()
        {
            YEARLYINVESTMENTs = new HashSet<YEARLYINVESTMENT>();
        }

        [Key]
        public int SIMULATIONID { get; set; }

        public int? NETWORKID { get; set; }

        public DateTime? DATE_CREATED { get; set; }

        public DateTime? DATE_LAST_RUN { get; set; }

        [StringLength(50)]
        public string ANALYSIS { get; set; }

        [StringLength(50)]
        public string BUDGET_CONSTRAINT { get; set; }

        [StringLength(50)]
        public string WEIGHTING { get; set; }

        public int COMMITTED_START { get; set; }

        public int COMMITTED_PERIOD { get; set; }

        public double BENEFIT_LIMIT { get; set; }

        public string JURISDICTION { get; set; }

        public string BENEFIT_VARIABLE { get; set; }

        [Column("COMMENTS")]
        [StringLength(8000)]
        public string COMMENTS { get; set; }

        [Column("SIMULATION")]
        [StringLength(50)]
        public string SIMULATION1 { get; set; }

        public virtual NETWORK NETWORK { get; set; }

        public virtual INVESTMENTS INVESTMENTS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<YEARLYINVESTMENT> YEARLYINVESTMENTs { get; set; }

        public virtual ICollection<TREATMENT> TREATMENTS { get; set; }

        public virtual ICollection<PERFORMANCE> PERFORMANCES { get; set; }
                
        public virtual ICollection<PRIORITIZEDNEED> PRIORITIZEDNEEDs { get; set; }
        public virtual ICollection<PRIORITY> PRIORITies { get; set; }
        public virtual ICollection<DEFICIENT> DEFICIENTS { get; set; }
        public virtual ICollection<TARGET_DEFICIENT> TARGET_DEFICIENT { get; set; }
        public virtual ICollection<Targets> TARGETS { get; set; }                     
        public virtual ICollection<COMMITTED_> COMMITTEDPROJECTs { get; set; }
    }
}