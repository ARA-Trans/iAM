using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
	public class LRSObject : RoadCareDataObject, ILRSObject
    {
        private String m_strRoute;
        protected double m_dBeginStation;
		protected double m_dEndStation;
        private String m_strDirection;
        private DateTime m_dateData;

        /// <summary>
        /// LRS Route name
        /// </summary>
        public String Route
        {
            set { m_strRoute = value; }
            get { return m_strRoute; }
        }
        /// <summary>
        /// LRS Begin Station
        /// </summary>
        public double BeginStation
        {
            set { m_dBeginStation = value; }
            get { return m_dBeginStation; }
        }

        /// <summary>
        /// LRS End Station
        /// </summary>
        public double EndStation
        {
            set { m_dEndStation = value; }
            get { return m_dEndStation; }
        }

        /// <summary>
        /// LRS Direction
        /// </summary>
        public String Direction
        {
            set { m_strDirection = value; }
            get { return m_strDirection; }
        }

        /// <summary>
        /// Date of LRS Object
        /// </summary>
        public DateTime Date
        {
            set { m_dateData = value; }
            get { return m_dateData; }
        }

        public LRSObject()
        {
        }

        /// <summary>
        /// LRS data obects
        /// </summary>
        /// <param name="strRoute">Route name</param>
        /// <param name="strBeginStation">Begin Station</param>
        /// <param name="strEndStation">End Station</param>
        /// <param name="strDirection">LRS Direction</param>
        /// <param name="strDate">Date for attribute</param>
        public LRSObject(String strRoute, String strBeginStation, String strEndStation, String strDirection,String strDate)
        {
            this.Route = strRoute;
            if (!String.IsNullOrEmpty(strBeginStation))
            {
                try
                {
                    this.BeginStation = double.Parse(strBeginStation);
                }
                catch
                {
                }
            }
            if (!String.IsNullOrEmpty(strEndStation))
            {
                try
                {
                    this.EndStation = double.Parse(strEndStation);
                }
                catch
                {
                }
            }
            this.Direction = strDirection;

            if (!String.IsNullOrEmpty(strDate))
            {
                try
                {
                    this.Date = DateTime.Parse(strDate);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// LRS data object
        /// </summary>
        /// <param name="dr">Return from LRS attribute</param>
        public LRSObject(DataRow dr)
        {
            this.Route = dr["ROUTES"].ToString() ;
            if (!String.IsNullOrEmpty(dr["BEGIN_STATION"].ToString()))
            {
                try
                {
                    this.BeginStation = double.Parse(dr["BEGIN_STATION"].ToString());
                }
                catch
                {
                }
            }
            if (!String.IsNullOrEmpty(dr["END_STATION"].ToString()))
            {
                try
                {
                    this.EndStation = double.Parse(dr["END_STATION"].ToString());
                }
                catch
                {
                }
            }
            this.Direction = dr["DIRECTION"].ToString();

        }

    }
}
