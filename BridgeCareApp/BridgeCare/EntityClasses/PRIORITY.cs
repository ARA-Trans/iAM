namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PRIORITY
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRIORITY()
        {
            this.PRIORITYFUNDs = new HashSet<PRIORITYFUND>();
        }

        [Key]
        public int PRIORITYID { get; set; }
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public Nullable<int> PRIORITYLEVEL { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public Nullable<int> YEARS { get; set; }
    
        public virtual SIMULATION SIMULATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRIORITYFUND> PRIORITYFUNDs { get; set; }
    }
}
