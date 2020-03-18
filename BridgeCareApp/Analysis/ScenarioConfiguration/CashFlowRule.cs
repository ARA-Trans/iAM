using System.Collections.Generic;
using AASHTOWare;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    /// <summary>
    /// Provides a set of rules for splitting the costs of treatments across multiple years,
    /// for assets that match the given criterion
    /// </summary>
    class CashFlowRule: ICriteriaDriven
    {
        private string name; // Not needed for the analysis; remove if not needed for reports
        public Criterion Criterion { get; }
        private List<DistributionRule> distributionRules;
    }

    class DistributionRule
    {
        /// <summary>
        /// The number of years the treatment cost will be "flowed" across
        /// </summary>
        private int duration;
        /// <summary>
        /// The maximum treatment cost for which this cash flow rule may be applied.
        /// If None, it may be applied to any treatment
        /// </summary>
        private Option<double> costLimit;
        /// <summary>
        /// A list with as many entries as the duration, which sum to 100,
        /// representing the portion of the treatment cost that is paid each year
        /// </summary>
        private List<int> distribution;
    }
}
