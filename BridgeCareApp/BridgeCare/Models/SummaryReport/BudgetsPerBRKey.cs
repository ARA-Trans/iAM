using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.SummaryReport
{
    public class BudgetsPerBRKey
    {
        public int SECTIONID { get; private set; }
        public string BRKey { get; private set; }
        public string BridgeId { get; private set; }
        public string Budget { get; set; } = "";
        public int YEARS { get; private set; }
        public bool IsCommitted { get; set; }
        public string Treatment { get; set; }
    }
}
