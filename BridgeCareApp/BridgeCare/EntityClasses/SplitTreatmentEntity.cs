using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("SPLIT_TREATMENT")]
    public class SplitTreatmentEntity
    {
        [Key]
        public int SPLIT_TREATMENT_ID { get; set; }
        public int SIMULATIONID { get; set; }
        public string DESCRIPTION { get; set; }
        public string CRITERIA { get; set; }
        public ICollection<SplitTreatmentLimitEntity> SPLIT_TREATMENT_LIMITS { get; set; } = new List<SplitTreatmentLimitEntity>();
        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }

        public SplitTreatmentEntity() { }

        public SplitTreatmentEntity(int simulationId, SplitTreatmentModel model)
        {
            SIMULATIONID = simulationId;
            DESCRIPTION = model.Description;
            CRITERIA = model.Criteria;

            if (model.SplitTreatmentLimits.Any())
            {
                model.SplitTreatmentLimits.ForEach(stl =>
                    SPLIT_TREATMENT_LIMITS.Add(new SplitTreatmentLimitEntity(stl))
                );
            }
        }

        public static void DeleteEntry(SplitTreatmentEntity entity, BridgeCareContext db)
        {
            if (entity.SPLIT_TREATMENT_LIMITS.Any())
                entity.SPLIT_TREATMENT_LIMITS.ToList()
                    .ForEach(stl => SplitTreatmentLimitEntity.DeleteEntry(stl, db));

            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}