using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using BridgeCare.Models;

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
        public string EQUATION { get; set; }
        public bool? ISFUNCTION { get; set; }
        public byte?[] BINARY_CRITERIA { get; set; }

        [ForeignKey("ATTRIBUTE_")]
        public virtual AttributesEntity ATTRIBUTE { get; set; }
        [ForeignKey("TREATMENTID")]
        public virtual TreatmentsEntity TREATMENT { get; set; }

        public ConsequencesEntity() { }

        public ConsequencesEntity(int treatmentId, ConsequenceModel consequenceModel)
        {
            TREATMENTID = treatmentId;
            ATTRIBUTE_ = consequenceModel.Attribute;
            CHANGE_ = consequenceModel.Change;
            CRITERIA = consequenceModel.Criteria;
            EQUATION = consequenceModel.Equation;
            ISFUNCTION = consequenceModel.IsFunction ?? false;
        }

        public ConsequencesEntity(ConsequenceModel consequenceModel)
        {
            ATTRIBUTE_ = consequenceModel.Attribute;
            CHANGE_ = consequenceModel.Change;
            CRITERIA = consequenceModel.Criteria;
            EQUATION = consequenceModel.Equation;
            ISFUNCTION = consequenceModel.IsFunction ?? false;
        }

        public static void DeleteEntry(ConsequencesEntity consequencesEntity, BridgeCareContext db)
        {
            db.Entry(consequencesEntity).State = EntityState.Deleted;
        }
    }
}
