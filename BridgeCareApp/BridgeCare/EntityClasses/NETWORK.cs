namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NETWORKS")]
    public partial class NETWORK
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NETWORK()
        {
            SIMULATIONS = new HashSet<SIMULATION>();
        }

        public int NETWORKID { get; set; }

        [Required]
        [StringLength(50)]
        public string NETWORK_NAME { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SIMULATION> SIMULATIONS { get; set; }
    }
}
