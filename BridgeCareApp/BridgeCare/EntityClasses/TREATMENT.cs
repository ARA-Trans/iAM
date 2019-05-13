using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace BridgeCare.EntityClasses
{
    [Table("TREATMENTS")]
    public partial class TREATMENT
    {
        [Key]
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

        public virtual ICollection<COST> COST { get; set; }

        public virtual ICollection<FEASIBILITY> FEASIBILITY { get; set; }

        public virtual ICollection<SCHEDULED> SCHEDULED { get; set; }
        public virtual SIMULATION SIMULATION { get; set; }

        public static void delete(TREATMENT treatment, BridgeCareContext db)
        {
            foreach (COST cost in treatment.COST.ToList())
            {
                db.Entry(cost).State = EntityState.Deleted;
            }
            foreach (CONSEQUENCE consequence in treatment.CONSEQUENCES.ToList())
            {
                db.Entry(consequence).State = EntityState.Deleted;
            }
            foreach (FEASIBILITY feasibility in treatment.FEASIBILITY.ToList())
            {
                db.Entry(feasibility).State = EntityState.Deleted;
            }

            db.Entry(treatment).State = EntityState.Deleted;
        }
    }
}