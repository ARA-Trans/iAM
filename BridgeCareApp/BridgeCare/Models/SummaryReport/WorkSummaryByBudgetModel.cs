using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BridgeCare.Models.SummaryReport
{
    public class WorkSummaryByBudgetModel
    {
        public int YEARS { get; }

        public string TREATMENT { get; }

        public double CostPerTreatmentPerYear { get; }
    }
}
