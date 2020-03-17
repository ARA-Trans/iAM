using AASHTOWare;

namespace ARA.iAM.Analysis
{
    class AnalysisConfiguration
    {
        /// <summary>
        /// The equation used to calculate the AREA attribute for all assets
        /// </summary>
        private Equation areaEquation;
        /// <summary>
        /// The criterion for selecting which assets to include in the analysis
        /// </summary>
        private Criterion jurisdiction;
        /// <summary>
        /// The first year of the analysis
        /// </summary>
        private int startYear;
        /// <summary>
        /// The attribute used for ????
        /// </summary>
        private Option<string> weightingAttribute;
        /// <summary>
        /// The strategy used for judging which treatments are 'best'
        /// </summary>
        private IOptimizationStrategy optimizationType; // Comparison function between asset-treatment pairs?
        /// <summary>
        /// The strategy used for deciding how much of the budget to spend
        /// </summary>
        private IExpenditureStrategy budgetType;
        /// <summary>
        /// The attribute by which treatments' benefits are determined
        /// </summary>
        private string benefitAttribute;
        /// <summary>
        /// The minimum value the benefit variable can have before it is considered to have no benefit
        /// </summary>
        private int benefitLimit;
    }
}
