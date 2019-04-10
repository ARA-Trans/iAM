using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("TREATMENTS")]
    public partial class TREATMENT
    {
        [Key]
        [ForeignKey("COST")]
        public int TREATMENTID { get; set; }
        public int SIMULATIONID { get; set; }
        [Column("TREATMENT")]
        public string TREATMENT1 { get; set; }
        public int BEFOREANY { get; set; }
        public int BEFORESAME { get; set; }
        public string BUDGET { get; set; }
        public string DESCRIPTION { get; set; }
        public string OMS_IS_EXCLUSIVE { get; set; }
        public string OMS_IS_REPEAT { get; set; }
        public string OMS_REPEAT_START { get; set; }
        public string OMS_REPEAT_INTERVAL { get; set; }

        public virtual ICollection<CONSEQUENCE> CONSEQUENCES { get; set; }
        public virtual COST COST { get; set; }
        public virtual ICollection<FEASIBILITY> FEASIBILITY { get; set; }
        public virtual ICollection<SCHEDULED> SCHEDULED { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }
    }
}