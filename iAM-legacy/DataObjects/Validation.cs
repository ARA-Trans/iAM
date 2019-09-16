using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Windows.Forms;
using System.Collections;

namespace DataObjects
{
    public static class Validation
    {
        static List<LRSObject> m_listLRS;
        static List<SRSObject> m_listSRS;
        static List<PCIMethodObject> m_listPCIMethods;
        static Hashtable m_hashMethodDistress;//Key - method, value is hash of distresses and numbers
        static List<String> m_listPCISurveyTypes;
        /// <summary>
        /// Current list of LRS objects from Network Definitions.  Set on Validate Linear 
        /// </summary>
        public static List<LRSObject> LRS
        {
            get { return m_listLRS; }
        }

        public static List<PCIMethodObject> PCIMethods
        {
            get { return m_listPCIMethods; }
        }


        /// <summary>
        /// Load LRS definiton
        /// </summary>
        public static void LoadLRS()
        {
            m_listLRS = new List<LRSObject>();
            String strSelect;
            strSelect = "SELECT ROUTES, BEGIN_STATION, END_STATION, DIRECTION FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                m_listLRS.Add(new LRSObject(dr));
            }
        }

        /// <summary>
        /// Load SRS definiton
        /// </summary>
        public static void LoadSRS()
        {
            m_listSRS = new List<SRSObject>();
            String strSelect;
            strSelect = "SELECT FACILITY,SECTION FROM NETWORK_DEFINITION WHERE FACILITY IS NOT NULL";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                m_listSRS.Add(new SRSObject(dr));
            }
        }



        /// <summary>
        /// Validates Linear Reference System Objects (against NETWORK_DEFINITION)
        /// </summary>
        /// <param name="lrsObject">Object to validate</param>
        /// <param name="strError">If Error is found, returns error</param>
        /// <returns>True if valid object.  False if error found.</returns>
        public static bool ValidateLinear(LRSObject lrsObject, out String strError)
        {
            strError = "";
            if (m_listLRS == null)
            {
                LoadLRS();
            }

            LRSObject lrsFind = LRS.Find(delegate(LRSObject lrs) { return lrs.Route==lrsObject.Route && lrs.Direction==lrsObject.Direction; });
            if (lrsFind == null)
            {
                strError = "\tWarning:" + lrsObject.Route + " " + lrsObject.BeginStation.ToString() + "-" + lrsObject.EndStation.ToString()+ "("+ lrsObject.Direction + ") ROUTE/DIRECTION not defined in NETWORK_DEFINITION.  This data row will be ignored at Rollup.";
                return false;
            }

            //Check if BEGIN_STATION less that END_STATION
            if (lrsObject.BeginStation > lrsObject.EndStation)
            {
                strError = strError = "\tWarning:" + lrsObject.Route + " " + lrsObject.BeginStation.ToString() + "-" + lrsObject.EndStation.ToString() + "(" + lrsObject.Direction + ") END_STATION is greater than BEGIN_STATION.  This data row will be ignored at Rollup.";
                return false;
            }

            
            if (lrsFind.EndStation < lrsObject.BeginStation)
            {
                strError = strError = "\tWarning:" + lrsObject.Route + " " + lrsObject.BeginStation.ToString() + "-" + lrsObject.EndStation.ToString() + "(" + lrsObject.Direction + ") ROUTES END_STATION is less than data BEGIN_STATION.  This data row will be ignored at Rollup.";
                return false;
            }

            if (lrsFind.BeginStation > lrsObject.BeginStation)
            {
                strError = strError = "\tWarning:" + lrsObject.Route + " " + lrsObject.BeginStation.ToString() + "-" + lrsObject.EndStation.ToString() + "(" + lrsObject.Direction + ") ROUTES BEGIN_STATION is greater than data BEGIN_STATION.  Some data from this row may be ignored at Rollup.";
                return false;
            }

            if (lrsFind.EndStation < lrsObject.BeginStation)
            {
                strError = strError = "\tWarning:" + lrsObject.Route + " " + lrsObject.BeginStation.ToString() + "-" + lrsObject.EndStation.ToString() + "(" + lrsObject.Direction + ") ROUTES END_STATION is less than data BEGIN_STATION.  This data row will be ignored at Rollup.";
                return false;
            }

            if (lrsFind.EndStation < lrsObject.EndStation)
            {
                strError = strError = "\tWarning:" + lrsObject.Route + " " + lrsObject.BeginStation.ToString() + "-" + lrsObject.EndStation.ToString() + "(" + lrsObject.Direction + ") ROUTES END_STATION is less than data END_STATION.  Some data from this row may be ignored at Rollup.";
                return false;
            }

            return true;//Valid LRS object
        }


        /// <summary>
        /// Validates Section Reference System Objects (against NETWORK_DEFINITION)
        /// </summary>
        /// <param name="srsObject"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static bool ValidateSection(SRSObject srsObject, out String strError)
        {
            strError = "";
            SRSObject srs = m_listSRS.Find(delegate(SRSObject s) { return s.Facility == srsObject.Facility && s.Section == srsObject.Section;});
            if(srs == null)
            {
                strError = strError = "\tWarning:" + srsObject.Facility + "/" + srsObject.Section +  " FACILITY/SECTION on defined. Data from this row will be ignored at Rollup.";
                return false;
            }
            return true;//Valid SRS object
        }







        /// <summary>
        /// Makes sure there are no NULL values in row before checking
        /// </summary>
        /// <param name="row">DataGridView Row to check</param>
        /// <returns>True if no null values</returns>
        public static bool IsValidRow(DataGridViewRow row)
        {
            if (row == null) return false;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.Value == null) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks row entry from PCI Bulkload for valid Linear PCI distress entry.
        /// </summary>
        /// <param name="pci"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static bool ValidatePCIDetail(LRSObjectPCI pci, out String strError)
        {
            if (m_hashMethodDistress == null) LoadAttributesPCI();

            strError = "";
            //Check to make sure area is greater than 0
            if (pci.Area <= 0)
            {
                strError = "Error: PCI area must be greater than 0";
                return false;
            }

            //Make sure method is included in Roadcare
            PCIMethodObject pciMethod = m_listPCIMethods.Find(delegate(PCIMethodObject method) { return method.Method == pci.Method; });
            if (pciMethod == null)
            {
                strError = "Error: PCI Method: " + pci.Method + " not defined in RoadCare.";
                return false;
            }

            //Make sure the type of survey is included
            if (!m_listPCISurveyTypes.Contains(pci.Type))
            {
                strError = "Error: Survey type: " + pci.Type.ToString() + " is not recognized. RANDOM, ADDITIONAL and IGNORE allowed.";
                return false;
            }

            //Make sure distress is entered
            if (!pciMethod.CheckDistress(pci.Distress, pci.Severity, out strError))
            {
                return false;
            }

            //Make sure distress amount is greater than 0;
            if (pci.Amount <= 0 && pci.Distress != "No Distress")
            {
                strError = "Error: Distress amount must be greater than 0.";
                return false;
            }

            if (pci.Amount > pci.Area)
            {
                strError = "Error:Distress amount must be less than or equal to Survey area.";
                return false;
            }

            return true;
        }



        /// <summary>
        /// Checks row entry from PCI Bulkload for valid Section PCI distress entry.
        /// </summary>
        /// <param name="pci"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public static bool ValidatePCIDetail(SRSObjectPCI pci, out String strError)
        {
            if (m_listPCIMethods == null) LoadAttributesPCI();

            strError = "";
            //Check to make sure area is greater than 0
            if (pci.Area <= 0)
            {
                strError = "Error: PCI area must be greater than 0";
                return false;
            }

            //Make sure method is included in Roadcare
            PCIMethodObject pciMethod = m_listPCIMethods.Find(delegate(PCIMethodObject method) { return method.Method == pci.Method; });
            if (pciMethod == null)
            {
                strError = "Error: PCI Method: " + pci.Method + " not defined in RoadCare.";
                return false;
            }

            //Make sure the type of survey is included
            if (!m_listPCISurveyTypes.Contains(pci.Type))
            {
                strError = "Error: Survey type: " + pci.Type.ToString() + " is not recognized. Random, Additional and Ignore allowed.";
                return false;
            }
            if (pci.Distress != "No Distress")
            {
                //Make sure distress is entered
                if (!pciMethod.CheckDistress(pci.Distress, pci.Severity, out strError))
                {
                    return false;
                }
            }

            //Make sure distress amount is greater than 0;
            if (pci.Amount <= 0 && pci.Distress != "No Distress")
            {
                strError = "Error: Distress amount must be greater than 0.";
                return false;
            }

            if (pci.Amount > pci.Area)
            {
                strError = "Error:Distress amount must be less than or equal to Survey area.";
                return false;
            }

            return true;
        }


        /// <summary>
        /// Load PCI Methods for validation
        /// </summary>
        public static void LoadAttributesPCI()
        {
            m_hashMethodDistress = GetPCIDistresses();
            m_listPCIMethods = new List<PCIMethodObject>();

            List<String> listSeverity = new List<String>();
            listSeverity.Add("L");
            listSeverity.Add("M");
            listSeverity.Add("H");
			listSeverity.Add("-");

            List<String> listNone = new List<String>();
            foreach (String key in m_hashMethodDistress.Keys)
            {
                PCIMethodObject method = new PCIMethodObject(key);
                Hashtable hashDistress = (Hashtable)m_hashMethodDistress[key];
                foreach (String keyDistress in hashDistress.Keys)
                {
                    method.AddDistress(new PCIDistressObject(keyDistress, int.Parse(hashDistress[keyDistress].ToString()), listSeverity));
                    
                }
                m_listPCIMethods.Add(method);
            }
            m_listPCISurveyTypes = new List<String>();
            m_listPCISurveyTypes.Add("Random");
            m_listPCISurveyTypes.Add("Additional");
            m_listPCISurveyTypes.Add("Ignore");
        }
        /// <summary>
        /// Retrieves list of PCI METHODS, DISTRESS and NUMBER pairs
        /// </summary>
        /// <returns></returns>
        public static Hashtable GetPCIDistresses()
        {
            Hashtable hashMethodDistresses = new Hashtable();
            String strMethod;
            String strName;
            String strNumber;
            List<string> listMethods = new List<string>();
            String strSelect = "SELECT DISTINCT METHOD_,DISTRESSNAME,DISTRESSNUMBER FROM PCI_DISTRESS";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                strMethod = row[0].ToString().Trim();
                strName = row[1].ToString().Trim();
                strNumber = row[2].ToString();
                Hashtable hashDistress;
                if (!hashMethodDistresses.Contains(strMethod))
                {
                    hashDistress = new Hashtable();
                    hashMethodDistresses.Add(strMethod, hashDistress);
                }
                else
                {
                    hashDistress = (Hashtable)hashMethodDistresses[strMethod];
                }

                hashDistress.Add(strName, strNumber);
            }

            return hashMethodDistresses;
        }
    }
}
