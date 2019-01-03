using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace AspWebApi.Models
{
    public class DetailedReportModel
    {
        [Column(TypeName="VARCHAR")]
        public string Facility { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Section { get; set; }
        [Column(TypeName = "VARCHAR")]
        public string Treatment { get; set; }
        public int NumberTreatment { get; set; }
        public bool IsCommitted { get; set; }
        public int Years { get; set; }
    }
}