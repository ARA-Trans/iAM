using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class LRSObjectString:LRSObject
    {
        private String m_strData;

        /// <summary>
        /// LRS Data for NUMBER Attributes
        /// </summary>
        public String Data
        {
            set { m_strData = value; }
            get { return m_strData; }
        }

        /// <summary>
        ///  LRS Attribute String data obects (input as proper data types)
        /// </summary>
        /// <param name="strRoute"></param>
        /// <param name="dBeginStation"></param>
        /// <param name="dEndStation"></param>
        /// <param name="strDirection"></param>
        /// <param name="dateTime"></param>
        /// <param name="dData"></param>
        public LRSObjectString(String strRoute, double dBeginStation, double dEndStation, String strDirection, DateTime dateTime, String strData)
        {
            this.Route = strRoute;
            this.BeginStation = dBeginStation;
            this.EndStation = dEndStation;
            this.Direction = strDirection;
            this.Date = dateTime;
            this.Data = strData;
        }

        /// <summary>
        /// LRS Data Object String created with return from Raw Attribute table
        /// </summary>
        /// <param name="row">Raw Attribute SELECT * datarow</param>
        public LRSObjectString(DataRow row)
        {
            this.Route = row["ROUTES"].ToString();
            this.BeginStation = double.Parse(row["BEGIN_STATION"].ToString());
            this.EndStation = double.Parse(row["END_STATION"].ToString());
            this.Direction = row["DIRECTION"].ToString();
            this.Date = DateTime.Parse(row["DATE"].ToString());
            this.Data = row["DATA"].ToString();
        }
    }
}
