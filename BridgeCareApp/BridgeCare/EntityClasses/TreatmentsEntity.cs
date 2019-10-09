using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("TREATMENTS")]
    public class TreatmentsEntity
    {
        [Key]
        public int TREATMENTID { get; set; }
        public int SIMULATIONID { get; set; }
        public string TREATMENT { get; set; }
        public int BEFOREANY { get; set; }
        public int BEFORESAME { get; set; }
        public string BUDGET { get; set; }
        public string DESCRIPTION { get; set; }
        public string OMS_IS_EXCLUSIVE { get; set; }
        public string OMS_IS_REPEAT { get; set; }
        public string OMS_REPEAT_START { get; set; }
        public string OMS_REPEAT_INTERVAL { get; set; }

        public virtual ICollection<ConsequencesEntity> CONSEQUENCES { get; set; }
        public virtual ICollection<CostsEntity> COSTS { get; set; }
        public virtual ICollection<FeasibilityEntity> FEASIBILITIES { get; set; }
        public virtual ICollection<ScheduledEntity> SCHEDULEDS { get; set; }
        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }

        public static void DeleteEntry(TreatmentsEntity treatment, BridgeCareContext db)
        {
            foreach (CostsEntity cost in treatment.COSTS.ToList())
            {
                db.Entry(cost).State = EntityState.Deleted;
            }
            foreach (ConsequencesEntity consequence in treatment.CONSEQUENCES.ToList())
            {
                db.Entry(consequence).State = EntityState.Deleted;
            }
            foreach (FeasibilityEntity feasibility in treatment.FEASIBILITIES.ToList())
            {
                db.Entry(feasibility).State = EntityState.Deleted;
            }

            db.Entry(treatment).State = EntityState.Deleted;
        }

        public TreatmentsEntity() { }

        public TreatmentsEntity(int simulationId, TreatmentModel treatmentModel)
        {
            SIMULATIONID = simulationId;
            TREATMENT = treatmentModel.Name;
            BUDGET = string.Join(",", treatmentModel.Budgets);
            if (treatmentModel.Feasibility != null)
            {
                BEFOREANY = treatmentModel.Feasibility.YearsBeforeAny;
                BEFORESAME = treatmentModel.Feasibility.YearsBeforeSame;
            }
        }
    }
}