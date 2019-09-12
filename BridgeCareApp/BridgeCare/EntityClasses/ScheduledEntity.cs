using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{  
    [Table("SCHEDULED")]
    public class ScheduledEntity
    {
        [Key]
        public int SCHEDULEDID { get; set; }
        public int TREATMENTID { get; set; }
        public int SCHEDULEDYEAR { get; set; }
        public int SCHEDULEDTREATMENTID { get; set; }

        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }
    }
}
