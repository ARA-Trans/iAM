using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.SummaryReport
{
    public class UnfundedRecommendationModel
    {
        public int SECTIONID { get; private set; }
        public string BRKey { get; private set; }
        public int YEARS { get; private set; }
        public string Treatment { get; set; }
        public string Reason { get; set; }
        public string Budget { get; set; }
        public string Budget_Hash { get; set; }
        public double RiskScore { get; set; }
        public double TotalProjectCost { get; set; }
    }
}
