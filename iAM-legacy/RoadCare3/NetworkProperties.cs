using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using OrderedPropertyGrid;
//using CustomControls.ApplicationBlocks;
using System.Data.SqlClient;
using System.Collections;

using System.Data;
using DatabaseManager;
using System.Drawing.Design;

namespace RoadCare3
{
    public partial class NetworkProperties
    {
        private int m_nNumSections;
        private int m_nNetworkID;

        private String m_strNetworkName;
        private String m_strDesignerID = DBMgr.NativeConnectionParameters.UserName;
        private String m_strDesignerName;
        private String m_strDescription;

        private DateTime m_dateCreated = DateTime.Now;
        private DateTime m_dateLastRollup;
        private DateTime m_dateLastEdit;

        private bool m_bLock = false;
        private bool m_bPrivate = false;

        public NetworkProperties(String strNetworkName)
        {
            m_strNetworkName = strNetworkName;
        }

        #region Attributes
        public String Network_Name
        {
            get { return m_strNetworkName; }
            set { m_strNetworkName = value; }
        }


        public String Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public int Network_ID
        {
            get { return m_nNetworkID; }
            set { m_nNetworkID = value; }
        }

        public String Designer_ID
        {
            get { return m_strDesignerID; }
            set { m_strDesignerID = value; }
        }

        public String Designer_Name
        {
            get { return m_strDesignerName; }
            set { m_strDesignerName = value; }
        }

        public DateTime Date_Created
        {
            get { return m_dateCreated; }
            set { m_dateCreated = value; }
        }

        public DateTime Date_Last_Rollup
        {
            get { return m_dateLastRollup; }
            set { m_dateLastRollup = value; }
        }

        public DateTime Date_Last_Edit
        {
            get { return m_dateLastEdit; }
            set { m_dateLastEdit = value; }
        }

        public int Number_Sections
        {
            get { return m_nNumSections; }
            set { m_nNumSections = value; }
        }

        public bool Lock
        {
            get { return m_bLock; }
            set { m_bLock = value; }
        }

        public bool Private
        {
            get { return m_bPrivate; }
            set { m_bPrivate = value; }
        }
        #endregion

        public void UpdateDatabase(String strPropertyName, String strValue)
        {
            String strUpdate = "Update NETWORKS Set " + Global.NetworkHash[strPropertyName].ToString() + " = '" + strValue + "' Where NETWORK_NAME = '" + m_strNetworkName + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Unable to update NETWORKS table. " + sqlE.Message);
            }
        }

        public void SetNetworkProperties(PropertyGridEx.PropertyGridEx pgProperties, bool bIsModal)
        {
            // Initialize the property grid for attributes.
            pgProperties.Item.Clear();
			pgProperties.Item.Add("Network Name", "", !bIsModal, "Network Information", "Display name of the network.  (Do not use escape characters in network name i.e. \"\\,'", true);
            pgProperties.Item.Add("Description", "", false, "Network Information", "Network description", true);
            pgProperties.Item.Add("Designer UserID", "", false, "Network Information", "User ID of network creator.", true);
            pgProperties.Item.Add("Designer Name", "", false, "Network Information", "Name of network creator.", true);
            pgProperties.Item.Add("Date Created", "", true, "Network Information", "Network creation date.", true);
            pgProperties.Item.Add("Date Last Edit", "", true, "Network Information", "The last date changes were made to the network.", true);
            pgProperties.Item.Add("Number Sections", "", true, "Network Information", "Number of sections in the network.", true);

			//pgProperties.Item.Add("Lock", false, false, "Network Information", "True if only the creator and those with specific permissions can modify the network.", true);
			//pgProperties.Item.Add("Private", false, false, "Network Information", "True if only the creator and those with specific permissions can view the newtwork in the network list.", true);
        }

		public String SavePropertiesToDatabase( PropertyGridEx.PropertyGridEx pgProperties )
		{
			m_strNetworkName = pgProperties.Item[0].Value.ToString();
			String strInsert;
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					//strInsert = "INSERT INTO NETWORKS (NETWORK_NAME,DESCRIPTION,DESIGNER_USERID,DESIGNER_NAME,DATE_CREATED,DATE_LAST_EDIT,NUMBER_SECTIONS,LOCK_,PRIVATE_) VALUES ('"
					strInsert = "INSERT INTO NETWORKS (NETWORK_NAME, DESCRIPTION, DESIGNER_USERID, DESIGNER_NAME, DATE_CREATED, DATE_LAST_EDIT, NUMBER_SECTIONS) VALUES ('"
										+ pgProperties.Item[0].Value + "', '"
										+ pgProperties.Item[1].Value + "', '"
										+ pgProperties.Item[2].Value + "', '"
										+ pgProperties.Item[3].Value + "', '"
										+ pgProperties.Item[4].Value + "', '"
										+ pgProperties.Item[5].Value + "', '"
										+ pgProperties.Item[6].Value + "')";
					break;
				case "ORACLE":
					//strInsert = "INSERT INTO NETWORKS (NETWORK_NAME,DESCRIPTION,DESIGNER_USERID,DESIGNER_NAME,DATE_CREATED,DATE_LAST_EDIT,NUMBER_SECTIONS,LOCK_,PRIVATE_) VALUES (";
					strInsert = "INSERT INTO NETWORKS (NETWORK_NAME,DESCRIPTION,DESIGNER_USERID,DESIGNER_NAME,DATE_CREATED,DATE_LAST_EDIT,NUMBER_SECTIONS) VALUES (";
					foreach( PropertyGridEx.CustomProperty networkProperty in pgProperties.Item )
					{
						if( networkProperty.Value.ToString().ToLower() == "true" )
						{
							strInsert += "'1', ";
						}
						else if( networkProperty.Value.ToString().ToLower() == "false" )
						{
							strInsert += "'0', ";
						}
						else
						{
							strInsert += "'" + networkProperty.Value.ToString() + "', ";
						}
					}
					strInsert = strInsert.Remove( strInsert.Length - 2 );
					strInsert += ")";
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//break;
			}
			try
			{
				DBMgr.ExecuteNonQuery( strInsert );
			}
			catch( Exception sqlE )
			{
				Global.WriteOutput( "Error: Insert new network into database failed with, " + sqlE.Message );
				return "";
			}
			return m_strNetworkName;
		}

        public void UpdatePropertyGrid(PropertyGridEx.PropertyGridEx pgProperties)
        {
            // Get Attribute information from the database regarding this attribute.
            //String strQuery = "Select NETWORK_NAME, DESCRIPTION, DESIGNER_USERID, DESIGNER_NAME, DATE_CREATED, DATE_LAST_EDIT, NUMBER_SECTIONS, LOCK_, PRIVATE_ From NETWORKS Where NETWORK_NAME = '" + m_strNetworkName + "'";
			String strQuery = "Select NETWORK_NAME, DESCRIPTION, DESIGNER_USERID, DESIGNER_NAME, DATE_CREATED, DATE_LAST_EDIT, NUMBER_SECTIONS From NETWORKS Where NETWORK_NAME = '" + m_strNetworkName + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery);

                // More random error checking
                if (ds.Tables[0].Rows.Count != 1)
                {
                    throw (new Exception());
                }
                else
                { 
                    pgProperties.Item[0].Value = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    pgProperties.Item[1].Value = ds.Tables[0].Rows[0].ItemArray[1].ToString();
                    pgProperties.Item[2].Value = ds.Tables[0].Rows[0].ItemArray[2].ToString();
                    pgProperties.Item[3].Value = ds.Tables[0].Rows[0].ItemArray[3].ToString();
                    pgProperties.Item[4].Value = ds.Tables[0].Rows[0].ItemArray[4].ToString();
                    pgProperties.Item[5].Value = ds.Tables[0].Rows[0].ItemArray[5].ToString();
                    pgProperties.Item[6].Value = ds.Tables[0].Rows[0].ItemArray[6].ToString();
                    pgProperties.Refresh();
                }
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Cannot update property tool window.  " + sqlE.Message);
            }
        }
    }
}
