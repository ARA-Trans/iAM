using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoadCareGlobalOperations
{
    public class LinearObject
    {
        private String m_strRoutes;
        private String m_strDirection;
        private String m_strBegin;
        private String m_strEnd;
        private String m_strDate;
        private String m_strData;
        private double m_dBegin;
        private double m_dEnd;
        private double m_dData;
        private bool m_bIsNumberAttribute;
        private DateTime m_DateTime;


        public String Routes
        {
            set { m_strRoutes = value; }
            get { return m_strRoutes; }
        }

        public String Direction
        {
            set { m_strDirection = value; }
            get { return m_strDirection; }
        }

        public double Begin
        {
            set { m_dBegin = value; }
            get { return m_dBegin; }
        }

        public double End
        {
            set { m_dEnd = value; }
            get { return m_dEnd; }
        }

        public String StringDate
        {
            set { m_strDate = value; }
            get { return m_strDate; }
        }

        public DateTime Date
        {
            set { m_DateTime = value; }
            get { return m_DateTime; }
        }

        public String StringData
        {
            set { m_strData = value; }
            get { return m_strData; }
        }

        public double NumberData
        {
            set { m_dData = value; }
            get { return m_dData; }
        }

        /// <summary>
        /// Stores data from a LRS entry.
        /// </summary>
        /// <param name="strRoutes">Routes</param>
        /// <param name="strDirection">Direction of road asset</param>
        /// <param name="strBegin">Begin Station</param>
        /// <param name="strEnd">End Station</param>
        /// <param name="strDate">Date</param>
        /// <param name="strData">Data for specific tabl</param>
        /// <param name="bIsNumberAttribute"></param>
        public LinearObject(String strRoutes, String strDirection, String strBegin, String strEnd, String strDate, String strData, bool bIsNumberAttribute)
        {
            m_strRoutes = strRoutes;
            m_strDirection = strDirection;
            m_strEnd = strEnd;
            m_strBegin = strBegin;
            this.StringData = strData;
            m_strDate = strDate;
            try
            {
                m_dBegin = double.Parse(strBegin);
            }
            catch
            {
        
            }

            try
            {
                m_dEnd = double.Parse(strEnd);
            }
            catch
            {
                
            }

            m_bIsNumberAttribute = bIsNumberAttribute;
            if (bIsNumberAttribute)
            {
                try
                {
                    this.NumberData = double.Parse(strData);
                }
                catch
                {
                }
            }

        }


        /// <summary>
        /// Checks linear object for common errors
        /// </summary>
        /// <param name="strRow">Row on screen for user feedback</param>
        /// <returns>True if no error.  False if error</returns>
        public bool Check(String strRow, out String strError)
        {
            strError = "";
            if (String.IsNullOrEmpty(this.Routes.Trim()))
            {
                strError += "Error: Route may not be null or empty. Row:" + strRow + "\n";
            }

            if (String.IsNullOrEmpty(this.Direction.Trim()))
            {
                strError += "Error: Direction may not be null or empty. Row:" + strRow + "\n"; 
            }

            if (Routes.Contains("'"))
            {
                strError += "Error: Routes may not contain single quote or apostrophe. Row:" + strRow + "\n"; 
            }
            if (Direction.Contains("'"))
            {
                strError += "Error: Direction may not contain single quote or apostrophe. Row:" + strRow + "\n";
            }
            if (this.StringData.Contains("'"))
            {
                strError += "Error: Data may not contain single quote or apostrophe. Row:" + strRow + "\n";
            }

            if(String.IsNullOrEmpty(m_strBegin.Trim()) && String.IsNullOrEmpty(m_strEnd.Trim()))
            {
                strError += "Error: BEGIN_STATION and END_STATION may not both be blank. Row:" + strRow;
            }
            
            if (this.Begin > this.End)
            {
                strError += "Error: The END_STATION may not be less than the BEGIN_STATION. Row:" + strRow + "\n";
            }

            try
            {
                DateTime dateTime = DateTime.Parse(this.StringDate);
            }
            catch(Exception exc)
            {
                strError += "Error: Date format is not recognized. Row:" + strRow + " " + exc.Message + "\n"; 
            }

            if (m_bIsNumberAttribute)
            {
                if(String.IsNullOrEmpty(this.StringData.Trim()))
                {
                    strError += "Error: NULL number data not allowed. Row:" + strRow + "\n"; 
                }
                try
                {
                    this.NumberData = double.Parse(this.StringData);
                }
                catch(Exception exc)
                {
                    strError += "Error: Unable to parse DATA. Row:" + strRow + " " + exc.Message + "\n"; 
                }
            }

            if (strError.Length > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
