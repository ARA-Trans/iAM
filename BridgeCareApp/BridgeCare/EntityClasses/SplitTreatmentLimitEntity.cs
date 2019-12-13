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
    [Table("SPLIT_TREATMENT_LIMIT")]
    public class SplitTreatmentLimitEntity
    {
        [Key]
        public int SPLIT_TREATMENT_LIMIT_ID { get; set; }
        public int SPLIT_TREATMENT_ID { get; set; }
        public int? RANK { get; set; }
        public double? AMOUNT { get; set; }
        public string PERCENTAGE { get; set; }
        [ForeignKey("SPLIT_TREATMENT_ID")]
        public virtual SplitTreatmentEntity SPLIT_TREATMENT { get; set; }

        public SplitTreatmentLimitEntity() { }

        public SplitTreatmentLimitEntity(int splitTreatmentId, SplitTreatmentLimitModel model)
        {
            SPLIT_TREATMENT_ID = splitTreatmentId;
            RANK = model.Rank;
            AMOUNT = model.Amount;
            PERCENTAGE = model.Percentage;
        }

        public SplitTreatmentLimitEntity(SplitTreatmentLimitModel model)
        {
            RANK = model.Rank;
            AMOUNT = model.Amount;
            PERCENTAGE = model.Percentage;
        }

        public static void DeleteEntry(SplitTreatmentLimitEntity entity, BridgeCareContext db)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}