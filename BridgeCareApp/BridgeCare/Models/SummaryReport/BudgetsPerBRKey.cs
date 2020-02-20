using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.SummaryReport
{
    public class BudgetsPerBRKey
    {
        public int SECTIONID { get; set; }
        public string BRKey { get; set; }
        public string BridgeId { get; set; }
        public string Budget { get; set; } = "";
        public int YEARS { get; set; }
        public bool IsCommitted { get; set; }
        public string Treatment { get; set; }
    }
}
