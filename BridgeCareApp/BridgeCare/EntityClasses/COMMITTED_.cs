namespace BridgeCare.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("COMMITTED_")]
    public partial class COMMITTED_
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMMITTED_()
        {
            COMMIT_CONSEQUENCES = new List<COMMIT_CONSEQUENCES>();
        }
       
        public COMMITTED_(int simulationId, int sectionId, int years, string treatmentName, int yearSame, int yearAny, string budget, double cost_)
        {
            SIMULATIONID = simulationId;
            SECTIONID = sectionId;
            YEARS = years;
            TREATMENTNAME = treatmentName;
            YEARSAME = yearSame;
            YEARANY = yearAny;
            BUDGET = budget;
            COST_ = cost_;
            COMMIT_CONSEQUENCES = new List<COMMIT_CONSEQUENCES>();
        }

        [Key]
        public int COMMITID { get; set; }

        [ForeignKey("SIMULATION")]
        public int SIMULATIONID { get; set; }

        public int SECTIONID { get; set; }

        public int YEARS { get; set; }

        public string TREATMENTNAME { get; set; }

        public Nullable<int> YEARSAME { get; set; }

        public Nullable<int> YEARANY { get; set; }

        public string BUDGET { get; set; }

        public Nullable<double> COST_ { get; set; }

        public string OMS_IS_EXCLUSIVE { get; set; }

        public string OMS_IS_NOT_ALLOWED { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<COMMIT_CONSEQUENCES> COMMIT_CONSEQUENCES { get; set; }

        public virtual SIMULATION SIMULATION { get; set; }
    }
}
