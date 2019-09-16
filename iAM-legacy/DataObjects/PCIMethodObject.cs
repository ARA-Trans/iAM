using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    /// <summary>
    /// Stores information for which distress and severities are available for each type
    /// </summary>
    public class PCIMethodObject
    {
        List<PCIDistressObject> m_listDistresses;
        String m_strMethod;

        /// <summary>
        /// PCI Method
        /// </summary>
        public String Method
        {
            get { return m_strMethod; }
            set { m_strMethod = value; }
        }
        /// <summary>
        /// Create a new PCI method of input type
        /// </summary>
        /// <param name="strMethod"></param>
        public PCIMethodObject(String strMethod)
        {
            this.Method = strMethod;
            m_listDistresses = new List<PCIDistressObject>();
        }
       
        /// <summary>
        /// Add distress and severities to method
        /// </summary>
        /// <param name="pciDistress"></param>
        public void AddDistress(PCIDistressObject pciDistress)
        {
            m_listDistresses.Add(pciDistress);
        }

        /// <summary>
        /// Checks if input Distress is in METHOD
        /// </summary>
        /// <param name="strDistress">PCI Distress</param>
        /// <param name="strSeverity">PCI Distress Severity</param>
        /// <returns></returns>
        public bool CheckDistress(String strDistress, String strSeverity,out String strError)
        {
            strError = "";
            if (strDistress == "No Distress") return true;
            PCIDistressObject pciDistress = m_listDistresses.Find(delegate(PCIDistressObject pci) { return pci.Distress == strDistress; });
            if (pciDistress == null)
            {
                strError = "Error: Distress:" + strDistress + " not included for PCI Method:" + this.Method;
                return false;//Distress not found
            }
            if (pciDistress.Severities.Count == 0) return true;

            if (!pciDistress.Severities.Contains(strSeverity.ToUpper()))
            {
                strError = "Error: Severity: " + strSeverity + " not included for PCI Distress:" + strDistress;
                return false;
            }
            return true;
        }

        public int GetDistress(String strDistress)
        {
            PCIDistressObject distress = m_listDistresses.Find(delegate(PCIDistressObject pciDistress) { return pciDistress.Distress == strDistress; });
            return distress.DistressNumber;
        }
    }
}
