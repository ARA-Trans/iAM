using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("COSTS")]
    public partial class CostsEntity
    {
        [Key]
        public int COSTID { get; set; }
        public int TREATMENTID { get; set; }
        public string COST_ { get; set; }
        public string UNIT { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }
        public byte[] BINARY_COST { get; set; }
        public bool? ISFUNCTION { get; set; }

        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }
    }
}