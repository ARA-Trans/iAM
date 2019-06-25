using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("PRIORITIZEDNEED")]
    public class PrioritizedNeedEntity
    {
        [Key]
        public int ID_ { get; set; }
        public int SIMULATIONID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public int? PRIORITY { get; set; }

        [ForeignKey("SIMULATIONID")]
        public virtual SimulationEntity SIMULATION { get; set; }
    }
}
