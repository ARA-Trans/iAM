using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCareCodeFirst.Models
{
    public class BudgetReportModel
    {
        public int Years { get; set; }
        public string Budget { get; set; }
        public double? Cost_ { get; set; }
    }
}