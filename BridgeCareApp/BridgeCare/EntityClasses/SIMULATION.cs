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

        public int SIMULATIONID { get; set; }

        public int? NETWORKID { get; set; }

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
    }
}