using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{  
    public partial class SCHEDULED
    {
        [Key]
        public int SCHEDULEDID { get; set; }
        [ForeignKey("TREATMENT")]
        public int TREATMENTID { get; set; }
        public int SCHEDULEDYEAR { get; set; }
        public int SCHEDULEDTREATMENTID { get; set; }
    
        public virtual TREATMENT TREATMENT { get; set; }
    }
}
