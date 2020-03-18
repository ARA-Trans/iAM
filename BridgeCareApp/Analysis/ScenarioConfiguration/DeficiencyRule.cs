using System.Collections.Generic;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    /// <summary>
    /// Defines, for some group of assets, how low a specified attribute can be before
    /// those assets are considered "deficient," and what proportion of assets may be
    /// deficient in that way before it is considered unacceptable
    /// </summary>
    class DeficiencyRule: ICriteriaDriven
    {
        private string attribute;
        private string name;
        /// <summary>
        /// Assets for which the specified attribute is below this level are considered deficient
        /// </summary>
        private double deficientLevel;
        /// <summary>
        /// The percentage of assets which may acceptable be deficient in this attribute
        /// </summary>
        private double allowedDeficient;
        /// <summary>
        /// A criterion defining the assets this deficiency rule applies to
        /// </summary>
        public Criterion Criterion { get; }
    }
}
