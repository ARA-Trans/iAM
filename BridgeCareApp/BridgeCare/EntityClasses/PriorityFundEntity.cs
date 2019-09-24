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

        public PriorityFundEntity(int priorityId, PriorityFundModel priorityFundModel)
        {
            PRIORITYID = priorityId;
            BUDGET = priorityFundModel.Budget;
            FUNDING = priorityFundModel.Funding;
        }

        public static void DeleteEntry(PriorityFundEntity priorityFundEntity, BridgeCareContext db)
        {
            db.Entry(priorityFundEntity).State = EntityState.Deleted;
        }
    }
}