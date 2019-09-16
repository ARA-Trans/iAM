using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class SectionObject
    {
        private String m_strSectionID;
        private String m_strFacility;
        private double m_dBeginStation;
        private double m_dEndStation;
        private String m_strDirection;
        private String m_strSection;
        private double m_dArea;
        private String m_strUnits;
        private String m_strGeometry;
        private double m_dMinX;
        private double m_dMaxX;
        private double m_dMinY;
        private double m_dMaxY;
        
        /// <summary>
        /// SectionID
        /// </summary>
        public String SectionID
        {
            set { m_strSectionID = value; }
            get { return m_strSectionID; }
        }
        /// <summary>
        /// Facility
        /// </summary>
        public String Facility
        {
            set { m_strFacility = value; }
            get { return m_strFacility; }
        }
        /// <summary>
        /// Direction
        /// </summary>
        public String Direction
        {
            set { m_strDirection = value; }
            get { return m_strDirection; }
        }
        /// <summary>
        /// Section
        /// </summary>
        public String Section
        {
            set { m_strSection = value; }
            get { return m_strSection; }
        }

        /// <summary>
        /// Units
        /// </summary>
        public String Units
        {
            set { m_strUnits = value; }
            get { return m_strUnits; }
        }
        /// <summary>
        /// Geometry
        /// </summary>
        public String Geometry
        {
            set { m_strGeometry = value; }
            get { return m_strGeometry; }
        }




        /// <summary>
        /// Begin Station (milepost) 
        /// </summary>
        public double BeginStation
        {
            set { m_dBeginStation = value; }
            get { return m_dBeginStation; }
        }
        /// <summary>
        /// End Station (milepost) 
        /// </summary>
        public double EndStation
        {
            set { m_dEndStation = value; }
            get { return m_dEndStation; }
        }
        /// <summary>
        /// Area 
        /// </summary>
        public double Area
        {
            set { m_dArea = value; }
            get { return m_dArea; }
        }
        /// <summary>
        /// Envelope MinimumX 
        /// </summary>
        public double MinX
        {
            set { m_dMinX = value; }
            get { return m_dMinX; }
        }
        /// <summary>
        /// Envelope MaximumX 
        /// </summary>
        public double MaxX
        {
            set { m_dMaxX = value; }
            get { return m_dMaxX; }
        }
        /// <summary>
        /// Envelope MinimumY 
        /// </summary>
        public double MinY
        {
            set { m_dMinY = value; }
            get { return m_dMinY; }
        }
        /// <summary>
        /// Envelope MaximumY 
        /// </summary>
        public double MaxY
        {
            set { m_dMaxY = value; }
            get { return m_dMaxY; }
        }
        public SectionObject()
        {
        }

        /// <summary>
        /// Create Section object with a return from SELECT* from SECTION_networkid table.
        /// </summary>
        /// <param name="row"></param>
        public SectionObject(DataRow row)
        {
            this.SectionID = row["SECTIONID"].ToString();
            this.Facility = row["FACILITY"].ToString();
            this.Direction = row["DIRECTION"].ToString();
            this.Section = row["SECTION"].ToString();
            this.Units = row["UNITS"].ToString();
            this.Geometry = row["GEOMETRY"].ToString();

            if (row["BEGIN_STATION"] != DBNull.Value)
            {
                this.BeginStation = double.Parse(row["BEGIN_STATION"].ToString());
            }

            if (row["END_STATION"] != DBNull.Value)
            {
                this.EndStation = double.Parse(row["END_STATION"].ToString());
            }
            if (row["AREA"] != DBNull.Value)
            {
                this.Area = double.Parse(row["AREA"].ToString());
            }
            if (row["Envelope_MinX"] != DBNull.Value)
            {
                this.MinX = double.Parse(row["Envelope_MinX"].ToString());
            }

            if (row["Envelope_MaxX"] != DBNull.Value)
            {
                this.MaxX = double.Parse(row["Envelope_MaxX"].ToString());
            }

            if (row["Envelope_MinY"] != DBNull.Value)
            {
                this.MinY = double.Parse(row["Envelope_MinY"].ToString());
            }

            if (row["Envelope_MaxY"] != DBNull.Value)
            {
                this.MaxY = double.Parse(row["Envelope_MaxY"].ToString());
            }
        }
    }
}
