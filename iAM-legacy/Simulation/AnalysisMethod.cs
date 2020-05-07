using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Simulation
{
    public class AnalysisMethod
    {

        private bool m_bBenefitCost;
        static private String m_sBenefitAttribute = "";
        static private double m_dBenefitLimit = 0;
        //private String m_strType;
        private bool m_bRemainingLife;
   //     private Hashtable m_hashRemainingLife = new Hashtable();
        private String m_sTypeBudget;
        private String m_sTypeAnalysis;
        private bool _isOMSUnlimited;
        private bool _isOMSTargetEnforced = true;
        public bool IsConditionalRSL { get; set; }
        public bool IsUseReasons { get; set; }
        
        public AnalysisMethod()
        {
            this.IsConditionalRSL = false;
            IsUseReasons = true;
        }

        /// <summary>
        /// Type of Analysis (As Buget Permits,As Budget Permits with Deficient First, Until Targets Met, Until No Deficient, Until No Deficient an Targets Met, Unlimited Budget);
        /// </summary>
        public String TypeBudget
        {
            get { return m_sTypeBudget; }
            set { m_sTypeBudget = value; }
        }

        public String TypeAnalysis
        {
            get { return m_sTypeAnalysis; }
            set { m_sTypeAnalysis = value; }
        }        
        
        /// <summary>
        /// Is this Analysis a Benefit/Cost
        /// </summary>
        public bool IsBenefitCost
        {
            get { return m_bBenefitCost; }
            set { m_bBenefitCost = value; }
        }


        /// <summary>
        /// Is this Analysis a RemainingLife
        /// </summary>
        public bool IsRemainingLife
        {
            get { return m_bRemainingLife; }
            set { m_bRemainingLife = value; }
        }

        /// <summary>
        /// Minimum or maximum level to consider for calculating Benefit cost
        /// </summary>
        public double BenefitLimit
        {
            get { return m_dBenefitLimit; }
            set { m_dBenefitLimit = value; }

        }

        /// <summary>
        /// Attribute to perform Benefit/Cost analysis on
        /// </summary>
        public String BenefitAttribute
        {
            get { return m_sBenefitAttribute; }
            set { m_sBenefitAttribute = value; }

        }

        public bool IsOMSUnlimited
        {
            get { return _isOMSUnlimited; }
            set { _isOMSUnlimited = value; }
        }

        public bool IsOMSTargetEnforced
        {
            get { return _isOMSTargetEnforced; }
            set { _isOMSTargetEnforced = value; }
        }

        public bool UseCumulativeCost { get; set; }

        public bool UseAcrossBudgets { get; set; }
        ///// <summary>
        ///// Attributes to consider Remaining Life for.
        ///// </summary>
        //public Hashtable RemainingLifeAttributes
        //{
        //    get { return m_hashRemainingLife; }
        //    set { m_hashRemainingLife = value; }

        //}



    }
}
