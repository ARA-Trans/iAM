using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("CONSEQUENCES")]
    public class ConsequencesEntity
    {
        [Key]
        public int CONSEQUENCEID { get; set; }
        public int TREATMENTID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string CHANGE_ { get; set; }
        public string CRITERIA { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }
        public string EQUATION { get; set; }
        public bool? ISFUNCTION { get; set; }

        [ForeignKey("ATTRIBUTE_")]
        public virtual AttributesEntity ATTRIBUTE { get; set; }
        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }
    }
}
