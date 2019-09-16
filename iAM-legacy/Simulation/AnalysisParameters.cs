using System;
using System.Collections.Generic;
using System.Text;

namespace Simulation
{
    /// <summary>
    /// Parameters which go with each attribute for analsyis
    /// </summary>
    public class AnalysisParameters
    {
        private String m_sAttribute;
        private double m_dDeficient;
        private Criterias m_Criteria = new Criterias();
        private double m_dTarget;
        private double m_dPercentDeficient;

        /// <summary>
        /// Attribute for analysis parameters
        /// </summary>
        public String Attribute
        {
            get { return m_sAttribute; }
            set { m_sAttribute = value; }
        }


        /// <summary>
        /// Deficient level for Remaining Life calculation
        /// </summary>
        public double Deficient
        {
            get { return m_dDeficient; }
            set { m_dDeficient = value; }
        }

        /// <summary>
        /// Deficient level for Remaining Life calculation
        /// </summary>
        public Criterias Criteria
        {
            get { return m_Criteria; }
            set { m_Criteria = value; }
        }

        /// <summary>
        /// Target level for Until Targets Met
        /// </summary>
        public double Target
        {
            get { return m_dTarget; }
            set { m_dTarget = value; }
        }

        /// <summary>
        /// Percent Deficient allowed for Deficiency Analysis
        /// </summary>
        public double PercentDeficient
        {
            get { return m_dPercentDeficient; }
            set { m_dPercentDeficient = value; }
        }

    }
}
