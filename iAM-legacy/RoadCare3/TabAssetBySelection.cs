using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;

namespace RoadCare3
{
	public partial class TabAssetBySelection : BaseForm
	{
		AssetTab m_assetTab;

		public TabAssetBySelection(AssetTab assetTab)
		{
			InitializeComponent();
			m_assetTab = assetTab;
		}

		public void LoadDataGridView(DataSet assetSectionInfo)
		{
			dgvAssetsBySelection.Rows.Clear();
			foreach (DataRow dr in assetSectionInfo.Tables[0].Rows)
			{
				dgvAssetsBySelection.Rows.Add(dr.ItemArray);
			}
			if (dgvAssetsBySelection.Rows.Count > 0)
			{
				dgvAssetsBySelection.Rows[0].Selected = true;
			}
		}

		private void dgvAssetsBySelection_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			//LoadAssetManager(e.RowIndex);
		}

		private void LoadAssetManager(int rowSelected)
		{
			// Populate the asset manager on the right with information regarding the specific asset.
			// GEO_ID is stored in the first hidden column.
			if (dgvAssetsBySelection[0, rowSelected].Value != null)
			{
				String geoID = dgvAssetsBySelection["GEO_ID", rowSelected].Value.ToString();
				String assetType = dgvAssetsBySelection["ASSET_TYPE", rowSelected].Value.ToString();
				m_assetTab.LoadDataGridFromAssetView(geoID, assetType);
			}
		}

		private void dgvAssetsBySelection_KeyUp(object sender, KeyEventArgs e)
		{
			//if (e.KeyValue == 38 || e.KeyValue == 40)
			//{
			//    int iSelected = dgvAssetsBySelection.Rows.GetFirstRow(DataGridViewElementStates.Selected);
			//    LoadAssetManager(iSelected);
			//}
		}

		private void TabAssetBySelection_Enter(object sender, EventArgs e)
		{
			if (m_assetTab.IsHidden)
			{
				m_assetTab.Show();
			}
		}

		private void TabAssetBySelection_Load( object sender, EventArgs e )
		{
			SecureForm();
		}

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}

		private void dgvAssetsBySelection_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			LoadAssetManager(e.RowIndex);
		}
	}
}
