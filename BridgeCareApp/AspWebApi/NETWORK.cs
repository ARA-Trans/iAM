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
    
    public partial class NETWORK
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NETWORK()
        {
            this.DYNAMIC_SEGMENTATION = new HashSet<DYNAMIC_SEGMENTATION>();
            this.DYNAMIC_SEGMENTATION_RESULT = new HashSet<DYNAMIC_SEGMENTATION_RESULT>();
            this.NETWORK_TREE = new HashSet<NETWORK_TREE>();
            this.ROLLUP_CONTROL = new HashSet<ROLLUP_CONTROL>();
            this.SEGMENT_CONTROL = new HashSet<SEGMENT_CONTROL>();
            this.SIMULATIONS = new HashSet<SIMULATION>();
        }
    
        public int NETWORKID { get; set; }
        public string NETWORK_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string DESIGNER_USERID { get; set; }
        public string DESIGNER_NAME { get; set; }
        public Nullable<System.DateTime> DATE_CREATED { get; set; }
        public Nullable<System.DateTime> DATE_LAST_ROLLUP { get; set; }
        public Nullable<System.DateTime> DATE_LAST_EDIT { get; set; }
        public Nullable<int> NUMBER_SECTIONS { get; set; }
        public Nullable<bool> LOCK_ { get; set; }
        public Nullable<bool> PRIVATE_ { get; set; }
        public string NETWORK_DEFINITION_NAME { get; set; }
        public string NETWORK_AREA { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DYNAMIC_SEGMENTATION> DYNAMIC_SEGMENTATION { get; set; }
        public virtual NETWORK_DEFINITIONS NETWORK_DEFINITIONS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DYNAMIC_SEGMENTATION_RESULT> DYNAMIC_SEGMENTATION_RESULT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NETWORK_TREE> NETWORK_TREE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ROLLUP_CONTROL> ROLLUP_CONTROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SEGMENT_CONTROL> SEGMENT_CONTROL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SIMULATION> SIMULATIONS { get; set; }
    }
}
