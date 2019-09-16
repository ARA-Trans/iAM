using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class SRSObject
    {
        private String m_strFacility;
        private String  m_strSection;
        private String m_strSample;
        private DateTime m_dateData;
        /// <summary>
        /// Section Reference System (SRS) Facility
        /// </summary>
        public String Facility
        {
            set { m_strFacility = value; }
            get { return m_strFacility; }
        }

        /// <summary>
        /// Section Reference System (SRS) Section
        /// </summary>
        public String Section
        {
            set { m_strSection = value; }
            get { return m_strSection; }
        }

        /// <summary>
        /// Section Reference System (SRS) Sample
        /// </summary>
        public String Sample
        {
            set { m_strSample = value; }
            get { return m_strSample; }
        }

        /// <summary>
        /// Date of SRS Object
        /// </summary>
        public DateTime Date
        {
            set { m_dateData = value; }
            get { return m_dateData; }
        }

        public SRSObject()
        {
        }


        /// <summary>
        /// SRS Data Object
        /// </summary>
        /// <param name="strFacility">SRS Facility</param>
        /// <param name="strSection">SRS Section</param>
        /// <param name="strSample">SRS Sample</param>
        /// <param name="strSample">SRS Date</param>
        public SRSObject(String strFacility, String strSection, String strSample,DateTime date)
        {
            this.Facility = strFacility;
            this.Section = strSection;
            this.Sample = strSample;
            this.Date = date;
        }
        /// <summary>
        /// SRS Data Object retrieved from database raw attribute row
        /// </summary>
        /// <param name="row"></param>
        public SRSObject(DataRow row)
        {
            this.Facility = row["FACILITY"].ToString();
            this.Section = row["SECTION"].ToString();
        }
    
    
    
    }
}
