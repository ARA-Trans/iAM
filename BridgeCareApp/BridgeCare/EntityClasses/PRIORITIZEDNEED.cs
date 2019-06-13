namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PRIORITIZEDNEED
    {
        [Key]
        public int ID_ { get; set; }
        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public Nullable<int> PRIORITY { get; set; }
    
        public virtual SIMULATION SIMULATION { get; set; }
    }
}
