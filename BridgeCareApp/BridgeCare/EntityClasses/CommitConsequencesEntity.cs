using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    [Table("COMMIT_CONSEQUENCES")]
    public class CommitConsequencesEntity
    {
        public CommitConsequencesEntity() { }

        public CommitConsequencesEntity(string attribute, string change)
        {            
            ATTRIBUTE_ = attribute;
            CHANGE_ = change;
        }

        [Key]
        public int ID_ { get; set; }
        public int COMMITID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string CHANGE_ { get; set; }

        [ForeignKey("COMMITID")]
        public virtual CommittedEntity COMMITTED { get; set; }
        [ForeignKey("ATTRIBUTE_")]
        public virtual AttributesEntity ATTRIBUTE{ get; set; }
    }
}