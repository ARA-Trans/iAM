namespace BridgeCareCodeFirst.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DetailedReportModel")]
    public partial class DetailedReportModel
    {
        public int ID { get; set; }

        [Required]
        [StringLength(4000)]
        [Column(TypeName ="VARCHAR")]
        public string FACILITY { get; set; }

        [Required]
        [StringLength(4000)]
        [Column(TypeName = "VARCHAR")]
        public string SECTION { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "VARCHAR")]
        public string TREATMENT { get; set; }

        public int YEARS { get; set; }

        public bool? ISCOMMITTED { get; set; }

        public int? NUMBERTREATMENT { get; set; }
    }
}
