using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;

namespace DataObjects
{
    public class ImageObject
    {
        String m_strFacility;
        String m_strSection;
        String m_strDirection;
        
        int m_nYear;
        double m_dLatitude;
        double m_dLongitude;
        double m_dMilepost;
        int m_nPrecedent;
        Hashtable m_hashViewPath;

        /// <summary>
        /// Hashtable of ViewPath
        /// </summary>
        public Hashtable ViewPath
        {
            set { m_hashViewPath = value; }
            get { return m_hashViewPath; }
        }


        /// <summary>
        /// Route/Facility
        /// </summary>
        public String Facility
        {
            set { m_strFacility = value; }
            get { return m_strFacility; }
        }

        /// <summary>
        /// Section for SRS referncing
        /// </summary>
        public String Section
        {
            set { m_strSection = value; }
            get { return m_strSection; }
        }

        /// <summary>
        /// Travel direction
        /// </summary>
        public String Direction
        {
            set { m_strDirection = value; }
            get { return m_strDirection; }
        }

        /// <summary>
        /// Year data is good for
        /// </summary>
        public int Year
        {
            set { m_nYear = value; }
            get { return m_nYear; }
        }

        /// <summary>
        /// Latitude of camera origin
        /// </summary>
        public double Latitude
        {
            set { m_dLatitude = value; }
            get { return m_dLatitude; }
        }

        /// <summary>
        /// Longitude of camera origin
        /// </summary>
        public double Longitude
        {
            set { m_dLongitude = value; }
            get { return m_dLongitude; }
        }

        /// <summary>
        /// Milepost of camera origin
        /// </summary>
        public double Milepost
        {
            set { m_dMilepost = value; }
            get { return m_dMilepost; }
        }
        /// <summary>
        /// Precendent play order of pictures
        /// </summary>
        public int Precedent
        {
            set { m_nPrecedent = value; }
            get { return m_nPrecedent; }
        }

        public ImageObject()
        {
            this.ViewPath = new Hashtable();
        }

        public ImageObject(double dLatitude, double dLongitude, double dMilepost, int nPrecedent)
        {
            Latitude = dLatitude;
            Longitude = dLongitude;
            Milepost = dMilepost;
            Precedent = nPrecedent;
            m_hashViewPath = new Hashtable();
        }

        public ImageObject(DataRow dataRowImage,List<String> listViews)
        {
            m_hashViewPath = new Hashtable();
            this.Precedent = int.Parse(dataRowImage["PRECEDENT"].ToString());
            this.Facility = dataRowImage["FACILITY"].ToString();
            this.Section = dataRowImage["SECTION"].ToString();
            if (dataRowImage["MILEPOST"].ToString() != "" && dataRowImage["MILEPOST"] != null)
            {
                String strValue = dataRowImage["MILEPOST"].ToString();
                if (!String.IsNullOrEmpty(strValue))
                {
                    this.Milepost = double.Parse(dataRowImage["MILEPOST"].ToString());
                }
            }
            this.Direction = dataRowImage["DIRECTION"].ToString();
            this.Year = int.Parse(dataRowImage["YEAR_"].ToString());
            if (!String.IsNullOrWhiteSpace(dataRowImage["LATITUDE"].ToString()))
            {
                this.Latitude = double.Parse(dataRowImage["LATITUDE"].ToString());
                this.Longitude = double.Parse(dataRowImage["LONGITUDE"].ToString());
            }
            foreach(String view in listViews)
            {
                if (dataRowImage[view] != null)
                {
                    SetPath(view, dataRowImage[view].ToString());
                }
            }
        }

        public void SetPath(String strView, String strPath)
        {
            if (!m_hashViewPath.Contains(strView))
            {
                m_hashViewPath.Add(strView, strPath);
            }
        }

        public String GetPath(String strView)
        {
            if (m_hashViewPath.Contains(strView))
            {
                return m_hashViewPath[strView].ToString();
            }
            return "";
        }
    }
}
