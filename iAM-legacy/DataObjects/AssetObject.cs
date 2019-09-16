using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataObjects
{
    public class AssetObject
    {
        String m_strAsset;
        DateTime m_dateCreated;
        String m_strCreator;
        String m_strCreatorID;
        DateTime m_dateLastModified;
        String m_strServer;
        String m_strDataSource;
        String m_strUserID;
        String m_strSQLView;
        String m_strPassword;
        String m_strProvider;
        Boolean m_bNative;


        /// <summary>
        /// Server Name for non-native database
        /// </summary>
        public String Server
        {
            set { m_strServer = value; }
            get { return m_strServer; }
        }

        /// <summary>
        /// Datasource for non-native database
        /// </summary>
        public String DataSource
        {
            set { m_strDataSource = value; }
            get { return m_strDataSource; }
        }


        /// <summary>
        /// UserID for non-native database
        /// </summary>
        public String UserID
        {
            set { m_strUserID = value; }
            get { return m_strUserID; }
        }

        /// <summary>
        /// Password for non-native database
        /// </summary>
        public String Password
        {
            set { m_strPassword = value; }
            get { return m_strPassword; }
        }


        /// <summary>
        /// Provider for non-native database
        /// </summary>
        public String Provider
        {
            set { m_strProvider = value; }
            get { return m_strProvider; }
        }


        /// <summary>
        /// SQL command to create view for non-native database
        /// </summary>
        public String SQLView
        {
            set { m_strSQLView = value; }
            get { return m_strSQLView; }
        }
        
        /// <summary>
        /// True if RoadCare native database
        /// </summary>
        public Boolean Native
        {
            set { m_bNative = value; }
            get { return m_bNative; }
        }

                
        
        /// <summary>
        /// Table name for RoadCare Asset
        /// </summary>
        public String Asset
        {
            set { m_strAsset = value; }
            get { return m_strAsset; }
        }

        /// <summary>
        /// Date/Time for creation of RoadCare Asset
        /// </summary>
        public DateTime CreatedOn
        {
            set { m_dateCreated = value; }
            get { return m_dateCreated; }
        }


        /// <summary>
        /// Creator for RoadCare Asset
        /// </summary>
        public String Creator
        {
            set { m_strCreator = value; }
            get { return m_strCreator; }
        }

        /// <summary>
        /// CreatorId for RoadCare Asset
        /// </summary>
        public String CreatorID
        {
            set { m_strCreatorID = value; }
            get { return m_strCreatorID; }
        }


        /// <summary>
        /// Date/Time for last modification of RoadCare Asset
        /// </summary>
        public DateTime ModifiedOn
        {
            set { m_dateLastModified = value; }
            get { return m_dateLastModified; }
        }


        public AssetObject()
        {

        }


        public AssetObject(DataRow row)
        {
            if (row["NATIVE_"].ToString() == "false")
            {
                this.Native = false;
            }
            else
            {
                this.Native = true;
            }

            this.Asset = row["ASSET"].ToString();
            this.Creator = row["CREATOR_NAME"].ToString();
            this.CreatorID = row["CREATOR_ID"].ToString();
			if (!String.IsNullOrEmpty(row["DATE_CREATED"].ToString()))
			{
				this.CreatedOn = (DateTime)row["DATE_CREATED"];
			}
			if (!String.IsNullOrEmpty(row["LAST_MODIFIED"].ToString()))
			{
				this.ModifiedOn = (DateTime)row["LAST_MODIFIED"];
			}
            this.Server = row["SERVER"].ToString();
            this.DataSource = row["DATASOURCE"].ToString();
            this.UserID = row["USERID"].ToString();
            this.SQLView = row["SQLVIEW"].ToString();
            this.Password = row["PASSWORD_"].ToString();
            this.Provider = row["PROVIDER"].ToString();
        }
    }
}
