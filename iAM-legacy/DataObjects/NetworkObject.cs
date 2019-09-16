using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class NetworkObject
    {
        private String m_strNetworkID;
        private String m_strNetwork;
        private String m_strDescription;
        private String m_strDesignerUserID;
        private String m_strDesignerName;
        private DateTime m_dateCreated;
        private DateTime m_dateLastRollup;
        private DateTime m_dateLastEdit;
        private int m_nNumberSection;
        private bool m_bLocked;//Deprecated
        private bool m_bPrivate;//Deprecated

        /// <summary>
        /// Date/Time NETWORK created
        /// </summary>
        public DateTime Created
        {
            get { return m_dateCreated; }
            set { m_dateCreated = value; }
        }
        /// <summary>
        /// Date/Time of last NETWORK rollup
        /// </summary>
        public DateTime LastRollup
        {
            get { return m_dateLastRollup; }
            set { m_dateLastRollup = value; }
        }
        /// <summary>
        /// Date/Time of last edit of network settings
        /// </summary>
        public DateTime LastEdit
        {
            get { return m_dateLastEdit; }
            set { m_dateLastEdit = value; }
        }

        /// <summary>
        /// Number of Sections in current NETWORK
        /// </summary>
        public int NumberSections
        {
            get { return m_nNumberSection; }
            set { m_nNumberSection = value; }
        }

        /// <summary>
        /// Network is viewable only by creator.  DEPRECATED
        /// </summary>
        public bool Private
        {
            get { return m_bPrivate; }
            set { m_bPrivate = value; }
        }

        /// <summary>
        /// Network locked for editing.  DEPRECATED
        /// </summary>
        public bool Locked
        {
            get { return m_bLocked; }
            set { m_bLocked = value; }
        }        
        
        /// <summary>
        /// RoadCare NetworkID - IDENTITY
        /// </summary>
        public String NetworkID
        {
            get { return m_strNetworkID; }
            set { m_strNetworkID = value; }
        }
        /// <summary>
        /// Network Display Name
        /// </summary>
        public String Network
        {
            get { return m_strNetwork; }
            set { m_strNetwork = value; }
        }
        /// <summary>
        /// Network Description
        /// </summary>
        public String Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        /// <summary>
        /// USER ID for creator of NETWORK
        /// </summary>
        public String DesignerID
        {
            get { return m_strDesignerUserID; }
            set { m_strDesignerUserID = value; }
        }

        /// <summary>
        /// Display name for creator of NETWORK
        /// </summary>
        public String DesignerName
        {
            get { return m_strDesignerName; }
            set { m_strDesignerName = value; }
        }


        /// <summary>
        /// Pass in the return of a SELECT * FROM NETWORKS
        /// </summary>
        /// <param name="dataRowNetwork"></param>
        public NetworkObject(DataRow dataRowNetwork)
        {
            this.NetworkID = dataRowNetwork["NETWORKID"].ToString();
            this.Network = dataRowNetwork["NETWORK_NAME"].ToString();
            this.Description = dataRowNetwork["DESCRIPTION"].ToString();
            this.DesignerName = dataRowNetwork["DESIGNER_NAME"].ToString();
            this.DesignerID = dataRowNetwork["DESIGNER_USERID"].ToString();
            this.Created = DateTime.Now;
            this.LastEdit = DateTime.Now;
            this.LastRollup = DateTime.Now;

            DateTime.TryParse(dataRowNetwork["DATE_CREATED"].ToString(),out m_dateCreated);
            DateTime.TryParse(dataRowNetwork["DATE_LAST_ROLLUP"].ToString(),out m_dateLastRollup);
            DateTime.TryParse(dataRowNetwork["DATE_LAST_EDIT"].ToString(), out m_dateLastEdit);

            this.NumberSections = 0;
            int.TryParse(dataRowNetwork["NUMBER_SECTIONS"].ToString(), out m_nNumberSection);

            this.Private = false;
            this.Locked = false;

        }
    }
}
