using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("FEASIBILITY")]
    public partial class FEASIBILITY
    {
        [Key]
        public int FEASIBILITYID { get; set; }
        [ForeignKey("TREATMENT")]
        public int TREATMENTID { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
    
        public virtual TREATMENT TREATMENT { get; set; }
    }
}
