using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("TARGET_DEFICIENT")]
    public class TargetDeficientEntity
    {
        [Key]
        public int ID_ { get; set; }
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public int? YEARS { get; set; }
        public double? TARGETMEAN { get; set; }
        public double? DEFICIENT { get; set; }
        public double? TARGETPERCENTDEFICIENT { get; set; }
        public string CRITERIA { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }

        [ForeignKey("ATTRIBUTE_")]
        public virtual AttributeEntity ATTRIBUTE { get; set; }
        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }
    }
}
