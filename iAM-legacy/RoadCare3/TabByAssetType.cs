using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RoadCareDatabaseOperations;
using DatabaseManager;
using Validation_;

namespace RoadCare3
{
	public partial class TabByAssetType : BaseForm
	{
		private AssetTab m_assetTab;
		private FormAssetView m_formAssetView;

		public TabByAssetType(AssetTab assetTab, FormAssetView formAsssetView)
		{
			InitializeComponent();
			m_assetTab = assetTab;
			m_formAssetView = formAsssetView;
		}

		public void LoadDataGridView()
		{
			String assetType = cbAssetTypes.Text;
			String query = "";
			String geoID;
			if (assetType != "")
			{
				ConnectionParameters cp = DBMgr.GetAssetConnectionObject(assetType);
				dgvAssetsByType.Rows.Clear();
				dgvAssetsByType.Columns.Clear();
				List<String> assetAttributes = DBOp.GetAssetAttributes(assetType, cp);
				DataGridViewColumn dgvColumnToAdd;
				DataGridViewCell dgvCellTemplate = new DataGridViewTextBoxCell();

				//need to add GEO_ID or the columns don't match when we do the 
				//SELECT * below
				dgvColumnToAdd = new DataGridViewColumn(dgvCellTemplate);
				dgvColumnToAdd.Name = "GEO_ID";
				dgvColumnToAdd.HeaderText = "GEO_ID";
				dgvAssetsByType.Columns.Add(dgvColumnToAdd);

				foreach (String assetAttribute in assetAttributes)
				{
					dgvColumnToAdd = new DataGridViewColumn(dgvCellTemplate);
					dgvColumnToAdd.Name = assetAttribute;
					dgvColumnToAdd.HeaderText = assetAttribute;
					dgvAssetsByType.Columns.Add(dgvColumnToAdd);
				}
				List<string> facilities = new List<string>();
				if (m_formAssetView.AssetSectionInfo != null)
				{
					foreach (DataRow assetSectionRow in m_formAssetView.AssetSectionInfo.Tables[0].Rows)
					{
						string facility = assetSectionRow["FACILITY"].ToString();
						if (!facilities.Contains(facility))
						{
							facilities.Add(facility);
						}
					}

					query = "SELECT * FROM " + assetType + " WHERE (FACILITY = '";
					string whereFacilities = "";
					foreach (string facility in facilities)
					{
						whereFacilities += facility + "' OR FACILITY = '";
					}
					if (whereFacilities != "")
					{
						whereFacilities = whereFacilities.Substring(0, whereFacilities.Length - 17);
						query += whereFacilities + "') AND (GEO_ID = ";
					}

					String whereClause = "";
					if (m_formAssetView.AssetSectionInfo != null)
					{
						foreach (DataRow assetSectionRow in m_formAssetView.AssetSectionInfo.Tables[0].Rows)
						{
							geoID = assetSectionRow["GEO_ID"].ToString();
							whereClause += geoID + " OR GEO_ID = ";
						}
					}
					if (whereClause != "")
					{
						whereClause = whereClause.Substring(0, whereClause.Length - 13);
						query += whereClause + ")";
						try
						{
							DataSet assetInfo = DBMgr.ExecuteQuery(query, cp);
							foreach (DataRow assetInfoRow in assetInfo.Tables[0].Rows)
							{
								dgvAssetsByType.Rows.Add(assetInfoRow.ItemArray);
							}
						}
						catch (Exception exc)
						{
							Global.WriteOutput("Error: Could not query asset table " + assetType + ". " + exc.Message);
						}
					}
				}
			}
		}

		private void TabByAssetType_Load(object sender, EventArgs e)
		{
			List<String> assetTypes = DBOp.GetRawAssetNames();
			cbAssetTypes.Items.Clear();
			foreach (String assetType in assetTypes)
			{
				cbAssetTypes.Items.Add(assetType);
			}
			SecureForm();
		}

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}

		private void cbAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadDataGridView();
		}

		private void TabByAssetType_Enter(object sender, EventArgs e)
		{
			if (!m_assetTab.IsHidden)
			{
				m_assetTab.Hide();
			}
		}
	}
}
