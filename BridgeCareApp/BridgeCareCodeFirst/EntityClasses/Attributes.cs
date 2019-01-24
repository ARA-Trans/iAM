using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BridgeCareCodeFirst.EntityClasses
{
    [Table("ATTRIBUTES_")]
    public class Attributes
    {
        [Key]
        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string Attribute_ { get; set; }
        public bool Ascending { get; set; }
    }
}