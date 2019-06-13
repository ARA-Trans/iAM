namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class TARGET_DEFICIENT
    {
        [Key]
        public int ID_ { get; set; }
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public Nullable<int> YEARS { get; set; }
        public Nullable<double> TARGETMEAN { get; set; }
        public Nullable<double> DEFICIENT { get; set; }
        public Nullable<double> TARGETPERCENTDEFICIENT { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
    
        public virtual Attributes ATTRIBUTES_ { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }
    }
}
