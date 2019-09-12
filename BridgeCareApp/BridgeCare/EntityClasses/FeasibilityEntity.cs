using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("FEASIBILITY")]
    public class FeasibilityEntity
    {
        [Key]
        public int FEASIBILITYID { get; set; }
        public int TREATMENTID { get; set; }
        public string CRITERIA { get; set; }
        public byte[] BINARY_CRITERIA { get; set; }

        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }
    }
}
