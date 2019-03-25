using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    using System;

    [Table("PERFORMANCE")]
    public class PERFORMANCE
    {
        [Key]
        public int PERFORMANCEID { get; set; }

        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }

        [ForeignKey("ATTRIBUTES_")]
        public string ATTRIBUTE_ { get; set; }

        public string EQUATIONNAME { get; set; }
        public string CRITERIA { get; set; }
        public string EQUATION { get; set; }
        public Nullable<bool> SHIFT { get; set; }
        public byte[] BINARY_EQUATION { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public Nullable<bool> PIECEWISE { get; set; }
        public Nullable<bool> ISFUNCTION { get; set; }

        public virtual Attributes ATTRIBUTES_ { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }
    }
}