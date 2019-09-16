using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    /// <summary>
    /// Stores Distress name and severity for given PCI Method
    /// </summary>
    public class PCIDistressObject
    {
        int m_nDistress;
        String m_strDistress;
        List<String> m_listSeverity;
        string m_strAttribute;
        string m_strMethod;

		private float m_metricRatio;

        /// <summary>
        /// PCI Distress Number that makes up Method
        /// </summary>
        public int DistressNumber
        {
            set { m_nDistress = value; }
            get { return m_nDistress; }
        }

		public float MetricRatio
		{
			get { return m_metricRatio; }
		}
        
        /// <summary>
        /// PCI Distress that makes up Method
        /// </summary>
        public String Distress
        {
            set { m_strDistress = value; }
            get { return m_strDistress; }
        }
        /// <summary>
        /// PCI Severities allowed for a given Distress
        /// </summary>
        public List<String> Severities
        {
            set { m_listSeverity = value; }
            get { return m_listSeverity; }
        }

        public string Attribute
        {
            set { m_strAttribute = value; }
            get { return m_strAttribute; }
        }
        
        public string Method
        {
            set { m_strMethod = value; }
            get { return m_strMethod; }
        }

        /// <summary>
        /// Defines PCI Distress
        /// </summary>
        /// <param name="strDistress">Name of the Distress</param>
        /// <param name="listSeverity">List of severities for given Distress.  Add Empty List if no Severity</param>
        public PCIDistressObject(String strDistress, int nDistress, List<String> listSeverity)
        {
            this.DistressNumber = nDistress;
            this.Distress = strDistress;
            this.Severities = listSeverity;
        }

		/// <summary>
		/// Defines PCI Distress
		/// </summary>
		/// <param name="strDistress">Name of the Distress</param>
		public PCIDistressObject(String strDistress, int nDistress, float metricRatio)
		{
			this.DistressNumber = nDistress;
			this.Distress = strDistress;
			m_metricRatio = metricRatio;
		}

        /// <summary>
        /// Defines PCI Distress
        /// </summary>
        /// <param name="strDistress">Name of the Distress</param>
        public PCIDistressObject(string strDistress, int nDistress, float metricRatio,string strAttribute,string strMethod)
        {
            this.DistressNumber = nDistress;
            this.Distress = strDistress;
            m_metricRatio = metricRatio;
            m_strAttribute = strAttribute;
            m_strMethod = strMethod;
        }



		public PCIDistressObject()
		{

 
		}
    }
}
