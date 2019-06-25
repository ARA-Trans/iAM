using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("NETWORKS")]
    public class NetworkEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NetworkEntity()
        {
            SIMULATIONS = new HashSet<SimulationEntity>();
        }

        [Key]
        public int NETWORKID { get; set; }
        [Required]
        [StringLength(50)]
        public string NETWORK_NAME { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SimulationEntity> SIMULATIONS { get; set; }
    }
}
