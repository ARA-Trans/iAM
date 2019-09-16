using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class LRSObjectPCI:LRSObject
    {
        
        String m_strMethod;
        String m_strType;
        double m_dArea;
        double m_dPCI;
		string m_sample;

        String m_strDistress;
        String m_strSeverity;
        double m_dAmount;

        /// <summary>
        /// LRS PCI Method
        /// </summary>
        public String Method
        {
            set { m_strMethod = value; }
            get { return m_strMethod; }
        }

		public string Sample
		{
			get { return m_sample; }
		}

        /// <summary>
        /// LRS PCI Type
        /// </summary>
        public String Type
        {
            set { m_strType = value; }
            get { return m_strType; }
        }

        /// <summary>
        /// LRS PCI Area
        /// </summary>
        public double Area
        {
            set { m_dArea = value; }
            get { return m_dArea; }
        }

        /// <summary>
        /// LRS PCI 
        /// </summary>
        public double PCI
        {
            set { m_dPCI = value; }
            get { return m_dPCI; }
        }


        /// <summary>
        /// LRS PCI Distress
        /// </summary>
        public String Distress
        {
            set { m_strDistress = value; }
            get { return m_strDistress; }
        }

        /// <summary>
        /// LRS PCI Severity
        /// </summary>
        public String Severity
        {
            set { m_strSeverity = value; }
            get { return m_strSeverity; }
        }

        /// <summary>
        /// LRS PCI Extent
        /// </summary>
        public double Amount
        {
            set { m_dAmount = value; }
            get { return m_dAmount; }
        }

        /// <summary>
        /// LRS PCI Details data obects (input as proper data types)
        /// </summary>
        public LRSObjectPCI(String strRoute, double dBeginStation, double dEndStation, String strDirection, string sample, DateTime dateTime, String strMethod, String strType, double dArea, String strDistress,String strSeverity, double dAmount)
        {
            this.Route = strRoute;
            this.BeginStation = dBeginStation;
            this.EndStation = dEndStation;
            this.Direction = strDirection;
			this.m_sample = sample;
            this.Date = dateTime;
            this.Method = strMethod;
            this.Type = strType;
            this.Area = dArea;
            this.Distress = strDistress;
            this.Severity = strSeverity;
            this.Amount = dAmount;
        }


        /// <summary>
        /// LRS PCI data obects (input as proper data types)
        /// </summary>
        public LRSObjectPCI(String strRoute, double dBeginStation, double dEndStation, String strDirection, DateTime dateTime, String strMethod, String strType, double dArea, double dPCI)
        {
            this.Route = strRoute;
            this.BeginStation = dBeginStation;
            this.EndStation = dEndStation;
            this.Direction = strDirection;
            this.Date = dateTime;
            this.Method = strMethod;
            this.Type = strType;
            this.Area = dArea;
            this.PCI = dPCI;
        }


        /// <summary>
        /// LRS PCI data obects (input from Database PCI return row)
        /// </summary>
        public LRSObjectPCI(DataRow row)
        {
            this.Route = row["ROUTES"].ToString() ;
            this.BeginStation = double.Parse(row["BEGIN_STATION"].ToString());
            this.EndStation = double.Parse(row["END_STATION"].ToString());
            this.Direction = row["DIRECTION"].ToString();
            this.Date = DateTime.Parse(row["DATE"].ToString());
            this.Method = row["METHOD"].ToString();
            this.Type = row["TYPE"].ToString();
            this.Area =  double.Parse(row["AREA"].ToString());
            this.PCI =  double.Parse(row["PCI"].ToString());
        }



    }
}
