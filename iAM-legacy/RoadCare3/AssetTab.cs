using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Collections;
using RoadCare3.Properties;
using DatabaseManager;
using RoadCareDatabaseOperations;
using System.Text.RegularExpressions;

namespace RoadCare3
{
	public partial class AssetTab : DockContent
	{
		private DataTable m_MapQueryDataTable;
		private String m_strImagePath;
		private String m_strNetworkName;
		private Hashtable m_htAttributeYears;
		private String m_oldValue;
		//private int m_iNetworkID;

		public AssetTab(String strNetworkName, Hashtable htAttributeYears)
		{
			InitializeComponent();

			m_strNetworkName = strNetworkName;
			PositionAssetControls();

			// Load up previous settings
			tbShapeFilePath.Text = Settings.Default.LOCAL_PATH;

			m_htAttributeYears = htAttributeYears;
		}

		/// Created - 8/19/2008 Author: CBB
		/// <summary>
		/// Redraw the asset tab page controls to fit any size changes made to the control.
		/// </summary>
		private void PositionAssetControls()
		{
			bool bCheckPicture = chkBoxShowAssetPicture.Checked;
			if (!bCheckPicture)
			{
				lblDate.Top = chkBoxShowAssetPicture.Bottom + 10;
				cbActivityDate.Top = chkBoxShowAssetPicture.Bottom + 10;

				dgvAssetSelection.Top = lblDate.Bottom + 10;
				dgvAssetSelection.Left = 5;
				dgvAssetSelection.Width = Width - 10;
				dgvAssetSelection.Height = (Height - lblDate.Bottom + 0);

				pictureBoxAsset.Visible = false;
			}
			else
			{
				pictureBoxAsset.Visible = true;
				pictureBoxAsset.Left = 5;
				pictureBoxAsset.Width = Width - 10;
				pictureBoxAsset.Top = tbShapeFilePath.Bottom + 5;

				lblDate.Top = pictureBoxAsset.Bottom + 10;
				cbActivityDate.Top = pictureBoxAsset.Bottom + 10;

				dgvAssetSelection.Top = pictureBoxAsset.Bottom + 10;
				dgvAssetSelection.Left = 5;
				dgvAssetSelection.Width = Width - 10;
				dgvAssetSelection.Height = (Height - pictureBoxAsset.Height - cbSelectedAsset.Height - tbShapeFilePath.Height - 30) / 2;
			}
			Refresh();
		}

		/// <summary>
		/// Adds possible asset selections (according to a radius around which the user clicked) to the selected asset combo box
		/// and auto selects the first item in the list.
		/// </summary>
		/// <param name="queryDataTable">A data table from the DataSet containing all the asset information.  Used to populate the asset
		/// datagridview.</param>
		/// Created: 8/19/2008 By CBB
		public void UpdateAssetData(DataTable queryDataTable)
		{
			// Set the member data table to the table containing the information for the selected asset.
			m_MapQueryDataTable = queryDataTable;
			cbSelectedAsset.Items.Clear();

			// Add point assets in an area around the spot where the user clicked.
			foreach (DataRow dataRow in queryDataTable.Rows)
			{
				cbSelectedAsset.Items.Add(dataRow["GEOMETRY"].ToString());
			}
			if (cbSelectedAsset.Items.Count > 0)
			{
				// Select the first asset in the combo box.
				cbSelectedAsset.SelectedIndex = 0;
			}
			else
			{
				cbSelectedAsset.Text = "";
				dgvAssetSelection.Rows.Clear();
			}
		}

		/// <summary>
		/// Fills the asset datagridview with asset information selected from the asset combo box.
		/// </summary>
		private void UpdateAssetDGV()
		{
			List<String> listGeomIDs = new List<String>();

			// Get the row from the table that corresponds to the item selected from the combo box.
			int iSelectedIndex = cbSelectedAsset.SelectedIndex;
			DataRow dataRow = m_MapQueryDataTable.Rows[iSelectedIndex];

			// Clear the old dgv rows to prepare for new data, then loop through the data table containing the asset information
			// and place it into the dgv.
			dgvAssetSelection.Rows.Clear();
			for (int i = 0; i < dataRow.ItemArray.Length; i++)
			{
				dgvAssetSelection.Rows.Add(m_MapQueryDataTable.Columns[i].ColumnName, dataRow.ItemArray[i].ToString());
			}

			// Set the image path for this asset if an image folder location was given.
			String strGeoID = dataRow["GEO_ID"].ToString();
			try
			{
				m_strImagePath = dataRow[Settings.Default.IMAGE_PATH].ToString();
				m_strImagePath = m_strImagePath.Substring(1, m_strImagePath.Length - 1);
			}
			catch //(Exception exc)
			{
				//Global.WriteOutput("Warning: Image column does not yet exist for this asset. " + exc.Message);
			}

			// Color the selected asset yellow on the GISViewer.
			FormGISLayerManager formGISLayerManager;
			if (FormManager.IsFormGISLayerManagerOpen(out formGISLayerManager))
			{
				listGeomIDs.Add(strGeoID);
				formGISLayerManager.SetPointColor(Color.Yellow, listGeomIDs);
				this.Show();
			}

			// Query the <asset name>_CHANGELOG table and populate the combo box with <activity type> - DateModified.
			// On selection of an activity date from the combo box, temporarily undo changes made to the sign back to that date
			// to get the sign as it was on the date selected.
			RePopulateAssetHistoryComboBox(strGeoID, true);
		}

		/// Created - 8/19/2008 By: CBB
		/// <summary> 
		/// As users add changes to asset history data, the data is stored in changelog tables in the database.
		/// The new event must be populated to the asset history combo box, so the user can select it immediately after making the change.
		/// </summary>
		/// <param name="strGeoID">The asset ID of the asset to which the change is being applied.</param>
		/// <param name="b">Not being used currently...</param>
		private void RePopulateAssetHistoryComboBox(String strGeoID, bool b)
		{
			DataSet ds = null;
			try
			{
				// Get the new asset history information, including any changes just made in the data grid view.
				ds = DBOp.QueryAssetHistory(m_MapQueryDataTable.TableName, strGeoID);
			}
			catch (Exception exc)
			{
				//Global.WriteOutput("Error: Problem querying asset history table. " + exc.Message);
				System.Diagnostics.Debug.WriteLine("Error: Problem querying asset history table. " + exc.Message);
				return;
			}

			// Re-create the combo box.
			cbActivityDate.Items.Clear();
			cbActivityDate.Items.Add("Most Recent");
			String activity;
			String date;
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				activity = dr["WORKACTIVITY"].ToString();
				date = dr["DATE_MODIFIED"].ToString();
				cbActivityDate.Items.Add(activity + " - " + date);
			}
		}

		/// <summary>
		/// The user can use the combo box to change the asset they are viewing, without having to select it from the GIS viewer.
		/// This event handles changing the selected asset on the GIS viewer, and fills the asset data grid view with the new asset
		/// information.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// Created: 8/19/2008 By CBB
		private void cbSelectedAsset_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Give the asset tab focus if it doesnt have it and update the asset dgv.
            UpdateAssetDGV();
            if (chkBoxShowAssetPicture.Checked == true)
            {
                // Get the picture of the selected asset.
                String strPicturePath = tbShapeFilePath.Text + m_strImagePath;
                try
                {
                    pictureBoxAsset.PictureFile = strPicturePath;
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: " + exc.Message);
                    pictureBoxAsset.PictureFile = "";
                }
            }
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlgOpen = new FolderBrowserDialog();
			if (dlgOpen.ShowDialog() == DialogResult.OK)
			{
				String strFilePath = dlgOpen.SelectedPath;
				tbShapeFilePath.Text = strFilePath;
				Settings.Default.LOCAL_PATH = tbShapeFilePath.Text;

				if (chkBoxShowAssetPicture.Checked == true)
				{
					// Get the picture of the selected asset.
					String strPicturePath = tbShapeFilePath.Text + m_strImagePath;
					try
					{
						pictureBoxAsset.PictureFile = strPicturePath;
					}
					catch (Exception exc)
					{
						Global.WriteOutput("Error: " + exc.Message);
						pictureBoxAsset.PictureFile = "";
					}

					// Reset the UI
					PositionAssetControls();
				}
			}
		}

		private void chkBoxShowAssetPicture_CheckedChanged(object sender, EventArgs e)
		{
			if (tbShapeFilePath.Text != "")
			{
				if (chkBoxShowAssetPicture.Checked == false)
				{
					PositionAssetControls();
				}
				if (chkBoxShowAssetPicture.Checked == true)
				{
					PositionAssetControls();

					// Get the picture of the selected asset.
					String strPicturePath = tbShapeFilePath.Text + m_strImagePath;
					try
					{
						pictureBoxAsset.PictureFile = strPicturePath;
					}
					catch (Exception exc)
					{
						Global.WriteOutput("Error: " + exc.Message);
						pictureBoxAsset.PictureFile = "";
					}
				}
			}
		}

		private void AssetTab_SizeChanged(object sender, EventArgs e)
		{
			PositionAssetControls();
		}

		private void dgvAssetSelection_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			// Get the GEO_ID
			//int iSelectedIndex = cbSelectedAsset.SelectedIndex;
			//DataRow dataRow = m_MapQueryDataTable.Rows[iSelectedIndex];
			//String strGeoID = dataRow["GEO_ID"].ToString();
			//String strField = dgvAssetSelection[0, e.RowIndex].Value.ToString();
			//String strValue;
			//if (dgvAssetSelection[1, e.RowIndex].Value != null)
			//{
			//    strValue = dgvAssetSelection[1, e.RowIndex].Value.ToString();
			//}
			//else
			//{
			//    strValue = DBNull.Value.ToString();
			//}

			//// Get a reason for the modification
			//FormReasonAssetChanged form = new FormReasonAssetChanged();
			//if (form.ShowDialog() == DialogResult.OK)
			//{
			//    String workActivity = form.GetWorkActivity();
			//    try
			//    {
			//        DBOp.UpdateAssetTable(m_MapQueryDataTable.TableName, strGeoID, strField, strValue, m_oldValue, workActivity);
			//    }
			//    catch (Exception exc)
			//    {
			//        Global.WriteOutput("Error: Could not modify " + m_MapQueryDataTable.TableName + " update or insert failed. " + exc.Message);
			//    }
			//    RePopulateAssetHistoryComboBox(strGeoID, false);
			//}
			//return;
		}

		private void dgvAssetSelection_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			m_oldValue = dgvAssetSelection[1, e.RowIndex].Value.ToString();
		}

		private void OnActivityChange()
		{
			int iSelectedIndex = cbSelectedAsset.SelectedIndex;
			DataRow dataRow = m_MapQueryDataTable.Rows[iSelectedIndex];
			String strGeoID = dataRow["GEO_ID"].ToString();

			dgvAssetSelection.Rows.Clear();

			// Show the asset in its most current state. (This is its state in the asset viewer.)
			List<String> listGeomIDs = new List<String>();
			for (int i = 0; i < dataRow.ItemArray.Length; i++)
			{
				dgvAssetSelection.Rows.Add(m_MapQueryDataTable.Columns[i].ColumnName, dataRow.ItemArray[i].ToString());
			}
			
			//m_strImagePath = dataRow[Settings.Default.IMAGE_PATH].ToString();
			//if (m_strImagePath != "")
			//{
			//    m_strImagePath = m_strImagePath.Substring(1, m_strImagePath.Length - 1);
			//}


			// Use the GEO_ID to undo any changes made to the asset up to and including the selected ActivityDate.
			if (cbActivityDate.Text != "Most Recent")
			{
				Regex dateSelector = new Regex("(1[0-2]|[0-9])/[0-3]?[0-9]/[0-9]+");
				Regex timeSelector = new Regex("((1[0-2])|[1-9]):[0-5][0-9]:[0-5][0-9] [A|P]M");
				String strDate = dateSelector.Match(cbActivityDate.Text).Value + " " + timeSelector.Match(cbActivityDate.Text).Value;
				DataSet ds = DBOp.QueryAssetHistory(m_MapQueryDataTable.TableName, strGeoID, strDate);
				Hashtable attributeRows = new Hashtable();
				foreach (DataGridViewRow dgvRow in dgvAssetSelection.Rows)
				{
					attributeRows.Add(dgvRow.Cells[0].Value.ToString(), dgvRow);
				}
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					//set the value of the associated row to value from the changelog
					//System.Diagnostics.Debug.WriteLine(dr["VALUE"].ToString());
					((DataGridViewRow)attributeRows[dr["FIELD"].ToString()]).Cells[1].Value = dr["VALUE"].ToString();
				}
			}
			else
			{
				//((DataGridViewRow)attributeRows[dr["FIELD"].ToString()]).Cells[1].Value = dr["VALUE"].ToString();
			}
		}

		public void LoadDataGridFromAssetView(String geoID, String assetType)
		{
			dgvAssetSelection.Rows.Clear();
			String query = "SELECT * FROM " + assetType + " WHERE GEO_ID = " + geoID;
			try
			{
				ConnectionParameters cp = DBMgr.GetAssetConnectionObject(assetType);
				DataSet assetInfo = DBMgr.ExecuteQuery(query, cp);
				DataRow assetDataRow = assetInfo.Tables[0].Rows[0];
				assetInfo.Tables[0].TableName = assetType;
				UpdateAssetData(assetInfo.Tables[0]);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not get asset information from " + assetType + " table. " + exc.Message);
			}
		}

		private void cbActivityDate_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Disable dgv editing for any historical views. (ie - only allow editing of most recent data)
			if (cbActivityDate.Text == "Most Recent")
			{
				dgvAssetSelection.ReadOnly = false;
			}
			else
			{
				dgvAssetSelection.ReadOnly = true;
			}
			OnActivityChange();
		}

		private void AssetTab_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormManager.RemoveAssetTab(this);
		}
	}
}
