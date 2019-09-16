using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    class SRSObjectString:SRSObject
    {
        private String m_strData;

        /// <summary>
        /// Date of SRS Object
        /// </summary>
        public String Data
        {
            set { m_strData = value; }
            get { return m_strData; }
        }

        /// <summary>
        /// SRS STRING attribute Data Object
        /// </summary>
        /// <param name="strFacility">SRS Facility</param>
        /// <param name="strSection">SRS Section</param>
        /// <param name="strSample">SRS Sample</param>
        /// <param name="strSample">SRS Date</param>
        public SRSObjectString(String strFacility, String strSection, String strSample,DateTime date,String data)
        {
            this.Facility = strFacility;
            this.Section = strSection;
            this.Sample = strSample;
            this.Date = date;
            this.Data = data;
        }
        /// <summary>
        /// SRS STRING Data Object retrieved from database raw attribute row
        /// </summary>
        /// <param name="row"></param>
        public SRSObjectString(DataRow row)
        {
            this.Facility = row["FACILITY"].ToString();
            this.Section = row["SECTION"].ToString();
            this.Sample = row["SAMPLE"].ToString();
            this.Date = DateTime.Parse(row["DATE"].ToString());
            this.Data = row["DATA"].ToString();
        }
    }
}
