using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RoadCare3.Properties;
using DatabaseManager;
using System.Collections;
using RoadCareDatabaseOperations;
using WeifenLuo.WinFormsUI.Docking;

namespace RoadCare3
{
	public partial class FormAssetView : BaseForm
	{
		public String m_networkID;
		private String m_networkName;
		private Hashtable m_hashAttributeYear;
		private Hashtable m_hashGroupAttributeList = new Hashtable();

		private DataSet m_assetSectionInfo;

		private AssetTab assetManager;
		private TabAssetBySelection tabAssetBySelection;
		private TabByAssetType tabByAssetType;

		private List<String> m_sectionIDs = new List<String>();

		public FormAssetView(String networkID, Hashtable hashAttributeYear)
		{
			InitializeComponent();
			m_networkID = networkID;
			m_hashAttributeYear = hashAttributeYear;
		}

		protected void SecureForm()
		{
			//throw new NotImplementedException();
		}

		private void FormAssetView_Load(object sender, EventArgs e)
		{
			SecureForm();
			FormLoad(Settings.Default.ASSET_VIEW_IMAGE_KEY, Settings.Default.ASSET_VIEW_IMAGE_KEY_SELECTED);
			m_networkName = DBOp.GetNetworkName(m_networkID);
			TabText = "Asset View-" + m_networkName;
			Tag = "Asset View-" + m_networkName;
			labelAttribute.Text += " " + m_networkName;

			comboBoxRouteFacilty.Text = "All";
			FillRouteTable();
			ApplyFiltersToSectionGrid();
			this.dgvSection.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSection_RowEnter);
			this.comboBoxRouteFacilty.SelectedIndexChanged += new System.EventHandler(this.comboBoxRouteFacilty_SelectedIndexChanged);

			// Add to the FormManager
			FormManager.AddBaseForm(this);

			// Now create a new Asset Manager in the right dock panel on this form.
			assetManager = new AssetTab(m_networkName, m_hashAttributeYear);
			assetManager.Show(dpAssetDisplayContainer, DockState.DockRight);

			tabByAssetType = new TabByAssetType(assetManager, this);
			tabByAssetType.Show(dpAssetDisplayContainer, DockState.Document);

			tabAssetBySelection = new TabAssetBySelection(assetManager);
			tabAssetBySelection.Show(dpAssetDisplayContainer, DockState.Document);
		}

		/// <summary>
		/// Fill the route table on the Attibute View.  Two versions of this.  One is filtered by
		/// Custom Filter, the other is not.
		/// </summary>
		private void FillRouteTable()
		{
			String strSelect = "SELECT DISTINCT FACILITY FROM SECTION_" + m_networkID;
			DataSet ds = DBMgr.ExecuteQuery(strSelect);
			comboBoxRouteFacilty.Items.Clear();
			comboBoxRouteFacilty.Items.Add("All");
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				comboBoxRouteFacilty.Items.Add(row[0].ToString());
			}
		}

		private void dgvSection_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			String strID = dgvSection["SECTIONID", e.RowIndex].Value.ToString();
		}

		private void comboBoxRouteFacilty_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFiltersToSectionGrid();
		}

		private void buttonAssetFilter_Click(object sender, EventArgs e)
		{
			FormAssetFilter formAssetFilter = new FormAssetFilter(m_hashAttributeYear, m_networkID);
			if (formAssetFilter.ShowDialog() == DialogResult.OK)
			{
				txtAssetFilter.Text = formAssetFilter.GetWhereClause();
				if (dgvSection.Rows.Count > 0)
				{
					DataSet dsAssetSectionIDs = formAssetFilter.GetSectionIDSet();
					List<DataGridViewRow> finalSectionIDs = new List<DataGridViewRow>();
					foreach (DataRow assetSectionRow in dsAssetSectionIDs.Tables[0].Rows)
					{
						for (int i = 0; i < dgvSection.Rows.Count; i++)
						{
							if (assetSectionRow[0].ToString() == dgvSection[2, i].Value.ToString())
							{
								finalSectionIDs.Add(dgvSection.Rows[i]);
							}
						}
					}
					dgvSection.Rows.Clear();
					foreach (DataGridViewRow dgvRow in finalSectionIDs)
					{
						dgvSection.Rows.Add(dgvRow);
					}
					dgvSection.Focus();
					LoadSelectedSections();
				}
			}
		}

		private void ApplyFiltersToSectionGrid()
		{
			String strSelect = "SELECT FACILITY,SECTION,SECTION_" + m_networkID.ToString() + ".SECTIONID";

			//Then at a minumum
			strSelect += " FROM SECTION_" + m_networkID.ToString();

			// Now for each table that is attached we add
			strSelect += " INNER JOIN SEGMENT_" + m_networkID + "_NS0 ON SECTION_" + m_networkID + ".SECTIONID=SEGMENT_" + m_networkID + "_NS0.SECTIONID";


			String strWhere = "";

			if (comboBoxRouteFacilty.Text != "All")
			{
				strWhere = " WHERE FACILITY='" + comboBoxRouteFacilty.Text + "'";
				strSelect += strWhere;
			}
			try
			{
				dgvSection.Rows.Clear();
				DataSet dsSections = DBMgr.ExecuteQuery(strSelect);
				foreach (DataRow dr in dsSections.Tables[0].Rows)
				{
					dgvSection.Rows.Add(dr.ItemArray);
				}
				for (int i = 0; i < dgvSection.ColumnCount; i++)
				{
					dgvSection.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				}
				dgvSection.Columns["SECTIONID"].Visible = false;
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Filling Section View. Check advanced search. " + exc.Message);
			}
		}

		private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
		{
			ApplyFiltersToSectionGrid();
		}

		private void LoadSelectedSections()
		{
			if( DBMgr.IsTableInDatabase( "ASSET_SECTION_" + m_networkID ) )
			{
				m_sectionIDs.Clear();
				// Get all selected rows in the dgv
				int iSelectedRow = dgvSection.Rows.GetFirstRow( DataGridViewElementStates.Selected );
				int iLastSelectedRow = dgvSection.Rows.GetLastRow( DataGridViewElementStates.Selected );
				for( int i = 0; i < dgvSection.Rows.Count; i++ )
				{
					if( iSelectedRow != -1 && !m_sectionIDs.Contains( dgvSection[2, iSelectedRow].Value.ToString() ) )
					{
						m_sectionIDs.Add( dgvSection[2, iSelectedRow].Value.ToString() );
					}
					if( iSelectedRow == iLastSelectedRow )
					{
						break;
					}
					iSelectedRow = dgvSection.Rows.GetNextRow( iSelectedRow, DataGridViewElementStates.Selected );
				}
				String query = "SELECT GEO_ID, SECTIONID, ASSET_TYPE, FACILITY, SECTION, BEGIN_STATION, END_STATION FROM ASSET_SECTION_" + m_networkID + " WHERE SECTIONID = ";
				String whereClause = "";
				foreach( String sectionID in m_sectionIDs )
				{
					whereClause += sectionID + " OR SECTIONID = ";
				}
				if( whereClause != "" )
				{
					whereClause = whereClause.Substring( 0, whereClause.Length - 16 );

					if (comboBoxRouteFacilty.Text != "All")
					{
						query += whereClause + " AND FACILITY = '" + comboBoxRouteFacilty.Text + "'";
					}
					else
					{
						query += whereClause;
					}
					try
					{
						m_assetSectionInfo = DBMgr.ExecuteQuery( query );
						tabAssetBySelection.LoadDataGridView( m_assetSectionInfo );
						tabByAssetType.LoadDataGridView();
					}
					catch( Exception exc )
					{
						Global.WriteOutput( "Error: Problem getting section asset information. " + exc.Message );
					}
				}
			}
		}

		public DataSet AssetSectionInfo
		{
			get { return m_assetSectionInfo; }
		}

		private void dgvSection_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyValue == 17 || e.KeyValue == 38 || e.KeyValue == 40)
			{
				LoadSelectedSections();
			}
		}

		private void dgvSection_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				LoadSelectedSections();
			}
		}

		private void FormAssetView_FormClosed(object sender, FormClosedEventArgs e)
		{
			FormUnload();
			FormManager.RemoveBaseForm(this);
		}

		

	}
}
