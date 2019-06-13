using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BridgeCare.EntityClasses
{
    [Table("TARGETS")]
    public class Targets
    {
        [Key]
        public int Id_ { get; set; }
        [ForeignKey("SIMULATION")]
        public int SimulationID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Attribute_ { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(4000)]
        public string TargetName { get; set; }
        public double TargetMean { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(4000)]
        public string Criteria { get; set; }

        public virtual Attributes ATTRIBUTES_ { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }
    }
}