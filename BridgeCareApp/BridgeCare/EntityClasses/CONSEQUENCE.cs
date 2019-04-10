using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BridgeCare.EntityClasses
{
    
    public partial class CONSEQUENCE
    {
        [Key]
        public int CONSEQUENCEID { get; set; }
        [ForeignKey("TREATMENT")]
        public int TREATMENTID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string CHANGE_ { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public string EQUATION { get; set; }
        public Nullable<bool> ISFUNCTION { get; set; }
    
        public virtual Attributes ATTRIBUTES_ { get; set; }
        public virtual TREATMENT TREATMENT { get; set; }
    }
}
