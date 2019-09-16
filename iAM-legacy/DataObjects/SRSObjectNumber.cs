using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class SRSObjectNumber:SRSObject
    {
        private double m_dData;

        /// <summary>
        /// Date of SRS Object
        /// </summary>
        public double Data
        {
            set { m_dData = value; }
            get { return m_dData; }
        }

        /// <summary>
        /// SRS NUMBER attribute Data Object
        /// </summary>
        /// <param name="strFacility">SRS Facility</param>
        /// <param name="strSection">SRS Section</param>
        /// <param name="strSample">SRS Sample</param>
        /// <param name="strSample">SRS Date</param>
        public SRSObjectNumber(String strFacility, String strSection, String strSample,DateTime date,double data)
        {
            this.Facility = strFacility;
            this.Section = strSection;
            this.Sample = strSample;
            this.Date = date;
            this.Data = data;
        }
        /// <summary>
        /// SRS NUMBER Data Object retrieved from database raw attribute row
        /// </summary>
        /// <param name="row"></param>
        public SRSObjectNumber(DataRow row)
        {
            this.Facility = row["FACILITY"].ToString();
            this.Section = row["SECTION"].ToString();
            this.Sample = row["SAMPLE"].ToString();
            this.Date = DateTime.Parse(row["DATE"].ToString());
            this.Data = double.Parse(row["DATA"].ToString());
        }
    }
}
