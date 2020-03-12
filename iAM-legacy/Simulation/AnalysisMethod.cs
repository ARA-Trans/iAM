using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Simulation
{
    public class AnalysisMethod
    {
        /// <summary>
        /// Type of Analysis (As Buget Permits,As Budget Permits with Deficient First, Until Targets Met, Until No Deficient, Until No Deficient an Targets Met, Unlimited Budget);
        /// </summary>
        public String TypeBudget { get; set; }

        public String TypeAnalysis { get; set; }
        
        /// <summary>
        /// Is this Analysis a Benefit/Cost
        /// </summary>
        public bool IsBenefitCost { get; set; }


        /// <summary>
        /// Is this Analysis a RemainingLife
        /// </summary>
        public bool IsRemainingLife { get; set; }

        /// <summary>
        /// Minimum or maximum level to consider for calculating Benefit cost
        /// </summary>
        public double BenefitLimit { get; set; }
 
        /// <summary>
        /// Attribute to perform Benefit/Cost analysis on
        /// </summary>
        public String BenefitAttribute { get; set; }

        public bool UseCumulativeCost { get; set; }

        public bool UseAcrossBudgets { get; set; }
 



    }
}
