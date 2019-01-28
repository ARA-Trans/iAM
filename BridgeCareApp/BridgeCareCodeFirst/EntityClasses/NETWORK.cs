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

        [StringLength(200)]
        public string DESCRIPTION { get; set; }

        [StringLength(50)]
        public string DESIGNER_USERID { get; set; }

        [StringLength(200)]
        public string DESIGNER_NAME { get; set; }

        public DateTime? DATE_CREATED { get; set; }

        public DateTime? DATE_LAST_ROLLUP { get; set; }

        public DateTime? DATE_LAST_EDIT { get; set; }

        public int? NUMBER_SECTIONS { get; set; }

        public bool? LOCK_ { get; set; }

        public bool? PRIVATE_ { get; set; }

        [StringLength(50)]
        public string NETWORK_DEFINITION_NAME { get; set; }

        [StringLength(4000)]
        public string NETWORK_AREA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SIMULATION> SIMULATIONS { get; set; }
    }
}
