namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("COMMIT_CONSEQUENCES")]
    public partial class COMMIT_CONSEQUENCES
    {
        [Key]
        public int ID_ { get; set; }
        [ForeignKey("COMMITTED_")]
        public int COMMITID { get; set; }
        public string ATTRIBUTE_ { get; set; }
        public string CHANGE_ { get; set; }
    
        public virtual Attributes ATTRIBUTES_ { get; set; }
        public virtual COMMITTED_ COMMITTED_ { get; set; }
    }
}
