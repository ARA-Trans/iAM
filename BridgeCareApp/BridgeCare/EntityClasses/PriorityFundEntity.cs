using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public PriorityFundEntity(PriorityFundModel priorityFund)
        {
            BUDGET = priorityFund.Budget;
            FUNDING = priorityFund.Funding;
        }

        public PriorityFundEntity(int priorityId, PriorityFundModel priorityFund)
        {
            PRIORITYID = priorityId;
            BUDGET = priorityFund.Budget;
            FUNDING = priorityFund.Funding;
        }
    }
}