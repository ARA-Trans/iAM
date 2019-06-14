using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BridgeCare.EntityClasses
{
    public class COMMIT_CONSEQUENCES
    {
        public COMMIT_CONSEQUENCES(string attribute_, string change_)
        {            
            ATTRIBUTE_ = attribute_;
            CHANGE_ = change_;
        }

        [Key]
        public int ID_ { get; set; }

        [ForeignKey("COMMITTED_PROJECT")]
        public int COMMITID { get; set; }

        [ForeignKey("Attributes")]
        public string ATTRIBUTE_ { get; set; }

        public string CHANGE_ { get; set; }

        public virtual COMMITTED_PROJECT COMMITTED_PROJECT { get; set; }

        public virtual Attributes Attributes { get; set; }
    }
}