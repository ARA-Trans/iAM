using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class SRSObjectPCI:SRSObject
    {
        String m_strMethod;
        String m_strType;
        double m_dArea;
        double m_dPCI;
        String m_strDistress;
        String m_strSeverity;
        double m_dAmount;
        
        /// <summary>
        /// SRS PCI Method
        /// </summary>
        public String Method
        {
            set { m_strMethod = value; }
            get { return m_strMethod; }
        }

        /// <summary>
        /// SRS PCI Type
        /// </summary>
        public String Type
        {
            set { m_strType = value; }
            get { return m_strType; }
        }

        /// <summary>
        /// SRS PCI Area
        /// </summary>
        public double Area
        {
            set { m_dArea = value; }
            get { return m_dArea; }
        }

        /// <summary>
        /// SRS PCI 
        /// </summary>
        public double PCI
        {
            set { m_dPCI = value; }
            get { return m_dPCI; }
        }


        /// <summary>
        /// SRS PCI Distress
        /// </summary>
        public String Distress
        {
            set { m_strDistress = value; }
            get { return m_strDistress; }
        }

        /// <summary>
        /// SRS PCI Severity
        /// </summary>
        public String Severity
        {
            set { m_strSeverity = value; }
            get { return m_strSeverity; }
        }

        /// <summary>
        /// SRS PCI Extent
        /// </summary>
        public double Amount
        {
            set { m_dAmount = value; }
            get { return m_dAmount; }
        }
        /// <summary>
        /// SRS PCI Details attribute Data Object
        /// </summary>
        /// <param name="strFacility">SRS Facility</param>
        /// <param name="strSection">SRS Section</param>
        /// <param name="strSample">SRS Sample</param>
        /// <param name="strSample">SRS Date</param>
        public SRSObjectPCI(String strFacility, String strSection, String strSample, DateTime date, String strMethod, String strType, double dArea, String strDistress, String strSeverity, double dAmount)
        {
            this.Facility = strFacility;
            this.Section = strSection;
            this.Sample = strSample;
            this.Date = date;
            this.Method = strMethod;
            this.Type = strType;
            this.Area = dArea;
            this.Distress = strDistress;
            this.Severity = strSeverity;
            this.Amount = dAmount;
        }

		public SRSObjectPCI()
		{
 
		}

        /// <summary>
        /// SRS PCI Details attribute Data Object
        /// </summary>
        /// <param name="strFacility">SRS Facility</param>
        /// <param name="strSection">SRS Section</param>
        /// <param name="strSample">SRS Sample</param>
        /// <param name="strSample">SRS Date</param>
        public SRSObjectPCI(String strFacility, String strSection, String strSample, DateTime date, String strMethod, String strType, double dArea, double dPCI)
        {
            this.Facility = strFacility;
            this.Section = strSection;
            this.Sample = strSample;
            this.Date = date;
            this.Method = strMethod;
            this.Type = strType;
            this.Area = dArea;
            this.PCI = dPCI;
        }


        /// <summary>
        /// SRS PCI Data Object retrieved from database raw attribute row
        /// </summary>
        /// <param name="row"></param>
        public SRSObjectPCI(DataRow row)
        {
            this.Facility = row["FACILITY"].ToString();
            this.Section = row["SECTION"].ToString();
            this.Sample = row["SAMPLE"].ToString();
            this.Date = DateTime.Parse(row["DATE"].ToString());
            this.Method = row["METHOD"].ToString();
            this.Type = row["TYPE"].ToString();
            this.Area = double.Parse(row["AREA"].ToString());
            this.PCI = double.Parse(row["PCI"].ToString());
        }
	}
}
