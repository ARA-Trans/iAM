namespace BridgeCareCodeFirst.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class REPORT_13_9
    {
        [Key]
        [Column(Order = 0)]
        public int ID_ { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SECTIONID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int YEARS { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string TREATMENT { get; set; }

        public int? YEARSANY { get; set; }

        public int? YEARSSAME { get; set; }

        [StringLength(512)]
        public string BUDGET { get; set; }

        public double? COST_ { get; set; }

        public double? REMAINING_LIFE { get; set; }

        public double? BENEFIT { get; set; }

        public double? BC_RATIO { get; set; }

        public int? CONSEQUENCEID { get; set; }

        public int? PRIORITY { get; set; }

        [StringLength(4000)]
        public string RLHASH { get; set; }

        public int? COMMITORDER { get; set; }

        public bool? ISCOMMITTED { get; set; }

        public int? NUMBERTREATMENT { get; set; }

        [StringLength(4000)]
        public string CHANGEHASH { get; set; }

        public double? AREA { get; set; }

        public int? RESULT_TYPE { get; set; }
    }
}
