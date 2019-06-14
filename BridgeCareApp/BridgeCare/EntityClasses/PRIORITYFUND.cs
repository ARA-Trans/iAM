namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class PRIORITYFUND
    {
        [Key]
        public int PRIORITYFUNDID { get; set; }
        public string BUDGET { get; set; }
        public Nullable<double> FUNDING { get; set; }
        [ForeignKey("PRIORITY")]
        public int PRIORITYID { get; set; }
    
        public virtual PRIORITY PRIORITY { get; set; }
    }
}
