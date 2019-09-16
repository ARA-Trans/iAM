using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DatabaseManager;
using PropertyGridEx;
using System.Windows.Forms;

namespace RoadCare3
{
	class AssetProperties
	{
		private String m_AssetName;
		private String m_Server;
		private String m_Database;
		private String m_Password;
		private String m_UserName;
		private String m_View;
		private List<String> m_columnNames;

		public String AssetName
		{
			get { return m_AssetName; }
			set { m_AssetName = value; }
		}

		public String Server
		{
			get { return m_Server; }
			set { m_Server = value; }
		}

		public String Database
		{
			get { return m_Database; }
			set { m_Database = value; }
		}

		public String Login
		{
			get { return m_UserName; }
			set { m_UserName = value; }
		}

		public String Password
		{
			get { return m_Password; }
			set { m_Password = value; }
		}

		public String View
		{
			get { return m_View; }
			set { m_View = value; }
		}


		public AssetProperties(String assetName)
        {
			m_AssetName = assetName;
			m_columnNames = new List<String>();
        }

		public void UpdateAssetProperties(PropertyGridEx.PropertyGridEx pgProperties)
		{
			pgProperties.Item["Asset Name"].Value = m_AssetName;
		}

		public void SetAssetProperties(PropertyGridEx.PropertyGridEx pgProperties, bool bIsModal)
		{
			pgProperties.Item.Clear();
			try
			{
				DataSet columnNamesAndTypes = DBMgr.GetTableColumnsWithTypes("ASSETS");
				foreach (DataRow columnNameAndType in columnNamesAndTypes.Tables[0].Rows)
				{
					m_columnNames.Add(columnNameAndType["column_name"].ToString());
					pgProperties.Item.Add(columnNameAndType["column_name"].ToString(), "", true, "General", "", true);
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not get ASSETS table column names or types. " + exc.Message);
			}

			string query = "SELECT ASSET, DATE_CREATED, CREATOR_NAME, CREATOR_ID, LAST_MODIFIED, SERVER, DATASOURCE, USERID, PASSWORD_, PROVIDER, NATIVE_ FROM ASSETS WHERE ASSET = '" + m_AssetName + "'";
			try
			{
				DataSet assetTableInfo = DBMgr.ExecuteQuery(query);
				pgProperties.Item["ASSET"].Value = assetTableInfo.Tables[0].Rows[0]["ASSET"].ToString();
				pgProperties.Item["DATE_CREATED"].Value = assetTableInfo.Tables[0].Rows[0]["DATE_CREATED"].ToString();
				pgProperties.Item["CREATOR_NAME"].Value = assetTableInfo.Tables[0].Rows[0]["CREATOR_NAME"].ToString();
				pgProperties.Item["CREATOR_ID"].Value = assetTableInfo.Tables[0].Rows[0]["CREATOR_ID"].ToString();
				pgProperties.Item["LAST_MODIFIED"].Value = assetTableInfo.Tables[0].Rows[0]["LAST_MODIFIED"].ToString();
				pgProperties.Item["SERVER"].Value = assetTableInfo.Tables[0].Rows[0]["SERVER"].ToString();
				pgProperties.Item["DATASOURCE"].Value = assetTableInfo.Tables[0].Rows[0]["DATASOURCE"].ToString();
				pgProperties.Item["USERID"].Value = assetTableInfo.Tables[0].Rows[0]["USERID"].ToString();
				pgProperties.Item["PASSWORD_"].Value = assetTableInfo.Tables[0].Rows[0]["PASSWORD_"].ToString();
				pgProperties.Item["PROVIDER"].Value = assetTableInfo.Tables[0].Rows[0]["PROVIDER"].ToString();
				pgProperties.Item["NATIVE_"].Value = assetTableInfo.Tables[0].Rows[0]["NATIVE_"].ToString();

			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Can not get asset information from ASSETS. " + exc.Message);
			}

			try
			{
				ConnectionParameters cp = DBMgr.GetAssetConnectionObject(m_AssetName);
				DataSet columnNamesAndTypes = DBMgr.GetTableColumnsWithTypes(m_AssetName, cp);
				foreach (DataRow columnNameAndType in columnNamesAndTypes.Tables[0].Rows)
				{
					m_columnNames.Add(columnNameAndType["column_name"].ToString());
					pgProperties.Item.Add(columnNameAndType["column_name"].ToString(), "", true, "General", "", true);
					pgProperties.Item[columnNameAndType["column_name"].ToString()].Value = columnNameAndType["data_type"].ToString();
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not get asset column names or types. " + exc.Message);
			}
			pgProperties.Refresh();
		}
	}
}
