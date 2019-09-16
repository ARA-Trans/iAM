using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class RollupSegmentObject
    {
        private String m_strAttribute;
        private String m_strRollupMethod;
        private String m_strSegmentMethod;
        /// <summary>
        /// Rollup Attribute
        /// </summary>
        public String Attribute
        {
            get { return m_strAttribute; }
            set { m_strAttribute = value; }
        }
        /// <summary>
        /// Rollup Method
        /// </summary>
        public String RollupMethod
        {
            get { return m_strRollupMethod; }
            set { m_strRollupMethod = value; }
        }
        /// <summary>
        /// Segmentation Method.  Any Record, Any Change, OR -
        /// </summary>
        public String SegmentMethod
        {
            get { return m_strSegmentMethod; }
            set { m_strSegmentMethod = value; }
        }

        /// <summary>
        /// Default Rollup object
        /// </summary>
        public RollupSegmentObject()
        {
        }
        /// <summary>
        /// Constructor for a RollupSegmentObject from SELECT * FROM ROLLUP_CONTROL
        /// </summary>
        /// <param name="row"></param>
        public RollupSegmentObject(DataRow row)
        {
            this.Attribute = row["ATTRIBUTE_"].ToString();
            this.RollupMethod = row["ROLLUPTYPE"].ToString();
            if (row["SEGMENTTYPE"] != DBNull.Value)
            {
                this.SegmentMethod = row["SEGMENTTYPE"].ToString();
            }
            else
            {
                this.SegmentMethod = "-";
            }
        }
    }
}
