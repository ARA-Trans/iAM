using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("COSTS")]
    public partial class COST
    {
        [Key]
        public int COSTID { get; set; }
        [ForeignKey("TREATMENT")]
        public int TREATMENTID { get; set; }
        public string COST_ { get; set; }
        public string UNIT { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public byte[] BINARY_COST { get; set; }
        public Nullable<bool> ISFUNCTION { get; set; }

        public virtual TREATMENT TREATMENT { get; set; }
    }
}