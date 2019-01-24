using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BridgeCareCodeFirst.EntityClasses
{
    [Table("INVESTMENTS")]
    public class INVESTMENTS
    {
        [Key]
        public int SIMULATIONID { get; set; }
        public string BUDGETORDER { get; set; }
    }
}