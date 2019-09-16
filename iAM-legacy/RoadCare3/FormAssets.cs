using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Data.SqlClient;
using WeifenLuo.WinFormsUI.Docking;
using DatabaseManager;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;
using KMLManager;
using System.Collections.Specialized;
namespace RoadCare3
{
    public partial class FormAssets : BaseForm
    {
        private String m_strAsset = "";
        private BindingSource binding;
        private DataAdapter dataAdapter;
        private DataTable table;
		private ConnectionParameters m_assetCP;
		private string m_advancedSearchText;

        public FormAssets(String strAsset)
        {
            InitializeComponent();
            m_strAsset = strAsset;
			m_assetCP = DBMgr.GetAssetConnectionObject(m_strAsset);
        }

		protected void SecureForm()
		{
			//throw new NotImplementedException();

		}

		public string AdvancedSearchText
		{
			get { return m_advancedSearchText; }
		}

		public string AssetName
		{
			get { return m_strAsset; }
		}

        private void FormAssets_Load(object sender, EventArgs e)
        {
			SecureForm();
			FormLoad(Settings.Default.ASSET_IMAGE_KEY, Settings.Default.ASSET_IMAGE_KEY_SELECTED);
            lblMaterial.Text = "Asset: " + m_strAsset;
            UpdateAssetGrid();
        }

        private void UpdateAssetGrid()
        {
            if (dataAdapter != null) dataAdapter.Dispose();// Free up the resources
            if (binding != null) binding.Dispose();
            if (table != null) table.Dispose();

            try
            {
                String strQuery = "Select * From " + m_strAsset;
                if (tbAdvancedSearch.Text != "")
                {
                    strQuery += " WHERE " + tbAdvancedSearch.Text;
                }
                binding = new BindingSource();
                dataAdapter = new DataAdapter(strQuery, m_assetCP);

                // Populate a new data table and bind it to the BindingSource.
                table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                binding.DataSource = table;
                dgvAssets.DataSource = binding;
                bindingNavigatorAsset.BindingSource = binding;
                dgvAssets.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
				dgvAssets.Columns["GEOMETRY"].Visible = false;
				dgvAssets.Columns["EnvelopeMinX"].Visible = false;
				dgvAssets.Columns["EnvelopeMaxX"].Visible = false;
				dgvAssets.Columns["EnvelopeMinY"].Visible = false;
				dgvAssets.Columns["EnvelopeMaxY"].Visible = false;
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Building current attribute view. SQL message is " + exception.Message);
            }
        }

        private void btnAdvancedSearch_Click(object sender, EventArgs e)
        {
            dataAdapter.Update((DataTable)binding.DataSource);
            dataAdapter.Dispose();

            String strQuery = tbAdvancedSearch.Text;
            FormQueryRaw formAdvancedSearch = new FormQueryRaw(m_strAsset, strQuery);
            if (formAdvancedSearch.ShowDialog() == DialogResult.OK)
            {
                tbAdvancedSearch.Text = formAdvancedSearch.m_strQuery;
				m_advancedSearchText = formAdvancedSearch.m_strQuery;
            }
            UpdateAssetGrid();
        }

        private void dgvAssets_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Global.WriteOutput("Error: " + e.Exception.Message);
        }

        private void FormAssets_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();

            FormManager.RemoveAssetsForm(this);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Global.BulkPaste(m_strAsset, dataAdapter, binding, dgvAssets);
        }

        private void FormAssets_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!DBMgr.NativeConnectionParameters.IsOleDBConnection)
            {
				try
				{
					dataAdapter.Update((DataTable)binding.DataSource);
				}
				catch (Exception exc)
				{
					Global.WriteOutput("Error closing tab. " + exc.Message);
				}
            }
        }

		//private void dgvAssets_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		//{
		//    //if (dgvAssets[e.ColumnIndex, e.RowIndex].Value != null)
		//    //{
		//    //    m_oldValue = dgvAssets[e.ColumnIndex, e.RowIndex].Value.ToString();
		//    //}
		//}

		//private void dgvAssets_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		//{
		//    //try
		//    //{
		//    //    DBOp.UpdateAssetTable(m_strAsset, dgvAssets[0, e.RowIndex].Value.ToString(),
		//    //                          dgvAssets.Columns[e.ColumnIndex].HeaderText, dgvAssets[e.ColumnIndex, e.RowIndex].Value.ToString(), m_oldValue, null);
		//    //}
		//    //catch (Exception exc)
		//    //{
		//    //    Global.WriteOutput("Error: Could not modify " + m_strAsset + " table or its CHANGELOG. " + exc.Message);
		//    //}

		//}

		private void FormAssets_Leave(object sender, EventArgs e)
		{
			try
			{
				dataAdapter.Update((DataTable)binding.DataSource);
			}
			catch (DBConcurrencyException dbConEx)
			{
				Global.WriteOutput( "Error: Database commit failed, please validate changes." + dbConEx.Message );
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Database commit failed, please validate changes." + exc.Message);
			}
		}

		private void dgvAssets_BindingContextChanged(object sender, EventArgs e)
		{
			if (dgvAssets.Columns.Contains("GEO_ID"))
			{
				dgvAssets.Columns["GEO_ID"].Visible = false;
			}
			if (dgvAssets.Columns.Contains("ID"))
			{
				dgvAssets.Columns["ID"].Visible = false;
			}
			if (dgvAssets.Columns.Contains("ENTRY_DATE"))
			{
				dgvAssets.Columns["ENTRY_DATE"].Visible = false;
			}
		}

        private void toolStripButtonKML_Click(object sender, EventArgs e)
        {
            String strAsset = this.m_strAsset;
            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\" + strAsset + ".kml";


            FormAssetKML formAssetKML = new FormAssetKML(strAsset);
            //if (formAssetKML.ShowDialog() == DialogResult.OK)
            {


            }



            StringCollection listProperties = new StringCollection();
			List<string> atrColumns = DBMgr.GetTableColumns(strAsset);
			foreach (string columnName in atrColumns)
			{
				listProperties.Add(columnName);
			}
            KML.CreateAssetKML(strAsset, strMyDocumentsFolder,strAsset, listProperties);

            System.Diagnostics.Process.Start(strMyDocumentsFolder);
        }
    }
}
