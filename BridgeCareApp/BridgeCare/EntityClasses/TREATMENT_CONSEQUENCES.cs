namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    
    public partial class TREATMENT_CONSEQUENCES
    {
        public int ID_ { get; set; }
        public int CONSEQUENCEID { get; set; }
        public int TREATMENTID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string CHANGE_ { get; set; }
        public virtual Attributes ATTRIBUTES_ { get; set; }
        public virtual TREATMENT TREATMENT { get; set; }
    }
}
