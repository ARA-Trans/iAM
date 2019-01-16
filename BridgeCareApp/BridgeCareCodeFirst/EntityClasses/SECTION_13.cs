namespace BridgeCareCodeFirst.EntityClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SECTION_13
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SECTIONID { get; set; }

        [StringLength(4000)]
        public string FACILITY { get; set; }

        public double? BEGIN_STATION { get; set; }

        public double? END_STATION { get; set; }

        [StringLength(50)]
        public string DIRECTION { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(4000)]
        public string SECTION { get; set; }

        public double? AREA { get; set; }

        [StringLength(50)]
        public string UNITS { get; set; }

        [StringLength(4000)]
        public string GEOMETRY { get; set; }

        public double? ENVELOPE_MINX { get; set; }

        public double? ENVELOPE_MAXX { get; set; }

        public double? ENVELOPE_MINY { get; set; }

        public double? ENVELOPE_MAXY { get; set; }
    }
}
