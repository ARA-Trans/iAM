using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
    public class NavigationEvent:EventArgs
    {
        private bool m_bLinear;
        private string m_strFacility;
        private string m_strDirection;
        private double m_dMilepost;
        private string m_strSection;
        private string m_strYear;
        public bool IsLinear
        {
            get { return m_bLinear; }
        }

        public string Facility
        {
            get { return m_strFacility; }
        }

        public string Direction
        {
            get { return m_strDirection; }
        }

        public double Station
        {
            get { return m_dMilepost; }
        }

        public string Section
        {
            get { return m_strSection; }
        }

        public string Year
        {
            get { return m_strYear;}
        }


        public NavigationEvent(String strFacility, String strDirection, double dMilePost)
        {
            m_bLinear = true;
            m_strFacility = strFacility;
            m_strDirection = strDirection;
            m_dMilepost = dMilePost;
        }

        
        public NavigationEvent(String strFacility, String strSection)
        {
            m_bLinear = false;
            m_strFacility = strFacility;
            m_strSection = strSection;
        }

        public NavigationEvent(String strFacility, String strDirection, double dMilePost,String strYear)
        {
            m_bLinear = true;
            m_strFacility = strFacility;
            m_strDirection = strDirection;
            m_dMilepost = dMilePost;
            m_strYear = strYear;
        }

        
        public NavigationEvent(String strFacility, String strSection,String strYear)
        {
            m_bLinear = false;
            m_strFacility = strFacility;
            m_strSection = strSection;
            m_strYear = strYear;
        }
    
    }
}
