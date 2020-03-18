using System.Collections.Generic;
using ARA.iAM.Analysis.Interfaces;

namespace ARA.iAM.Analysis
{
    class PerformanceRule: ICriteriaDriven
    {
        /// <summary>
        /// The equation that determines the rate of deterioration of the specified attribute
        /// </summary>
        private Equation deteriorationEquation;
        private string attribute;
        private string name;
        /// <summary>
        /// Defines the assets for which this performance rule is applied
        /// </summary>
        public Criterion Criterion { get; }
    }
}
