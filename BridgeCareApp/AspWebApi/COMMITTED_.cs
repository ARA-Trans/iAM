//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AspWebApi
{
    using System;
    using System.Collections.Generic;
    
    public partial class COMMITTED_
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public COMMITTED_()
        {
            this.COMMIT_CONSEQUENCES = new HashSet<COMMIT_CONSEQUENCES>();
        }
    
        public int COMMITID { get; set; }
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
