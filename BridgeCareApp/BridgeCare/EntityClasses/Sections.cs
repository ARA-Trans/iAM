using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BridgeCare.EntityClasses
{
    [Table("SECTION_13")]
    public class SECTIONS
    {
        [Key]
        public int SectionID { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(4000)]
        public string FACILITY { get; set; }
        public string SECTION { get; set; }


    }
}