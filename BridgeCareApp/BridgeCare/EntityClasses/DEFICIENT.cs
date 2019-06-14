namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class DEFICIENT
    {
        [Key]
        public int ID_ { get; set; }
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string DEFICIENTNAME { get; set; }
        public Nullable<double> DEFICIENT1 { get; set; }
        public Nullable<double> PERCENTDEFICIENT { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
    
        public virtual SIMULATION SIMULATION { get; set; }
    }
}
