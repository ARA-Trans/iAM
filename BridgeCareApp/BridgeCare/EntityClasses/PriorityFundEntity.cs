using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;

namespace BridgeCare.EntityClasses
{
    [Table("PRIORITYFUND")]
    public class PriorityFundEntity
    {
        [Key]
        public int PRIORITYFUNDID { get; set; }
        public string BUDGET { get; set; }
        public double? FUNDING { get; set; }
        public int PRIORITYID { get; set; }

        [ForeignKey("PRIORITYID")]
        public virtual PriorityEntity PRIORITY { get; set; }

        public PriorityFundEntity() { }

        public PriorityFundEntity(PriorityFundModel priorityFundModel)
        {
            BUDGET = priorityFundModel.Budget;
            FUNDING = priorityFundModel.Funding;
        }

        public PriorityFundEntity(int priorityId, PriorityFundModel model)
        {
            PRIORITYID = priorityId;
            BUDGET = model.Budget;
            FUNDING = model.Funding;
        }

        public static void DeleteEntry(PriorityFundEntity entity, BridgeCareContext db)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }
    }
}