using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BridgeCare.EntityClasses
{
    [Table("DEFICIENTS")]
    public class Deficients
    {
        [Key]
        public int Id_ { get; set; }
        public int SimulationID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Attribute_ { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(4000)]
        public string DeficientName { get; set; }
        public double Deficient { get; set; }
        public double PercentDeficient { get; set; }
    }
}