using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

using DatabaseManager;
using WeifenLuo.WinFormsUI.Docking;
using Reports;
using RoadCare3.Properties;
using RoadCareGlobalOperations;
using DataObjects;
using RoadCareDatabaseOperations;
using System.Diagnostics;

namespace RoadCare3
{
    public partial class FormSectionView :  BaseForm
    {
        private String m_strNetwork;
        private String m_strNetworkID;
        private BindingSource binding;
        private DataAdapter dataAdapter;
        private DataTable table;
        Hashtable m_hashAttributeYear;
        Hashtable m_hashGroupAttributeList = new Hashtable();
        SectionObject m_lastSectionObject;
		private string m_selectedRowSectionID;

        public FormSectionView(String strNetwork,Hashtable hashAttributeYear)
        {
            InitializeComponent();
            m_hashAttributeYear = hashAttributeYear;
            m_strNetwork = strNetwork;
            String strSelect = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + m_strNetwork + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception e)
            {
                String strError = "Error in getting NETWORKID for NETWORK_NAME = " + m_strNetwork + ". SQL error = " + e.Message;
                Global.WriteOutput(strError);
                MessageBox.Show(strError);
            }
            ToolTip tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            
			// Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(buttonUpdate, "Update data displayed in Section View dependent upon selected filters.");
            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
        }

        private void FormSectionView_Load(object sender, EventArgs e)
        {
			SecureForm();
			FormLoad(Settings.Default.SECTION_VIEW_IMAGE_KEY, Settings.Default.SECTION_VIEW_IMAGE_KEY_SELECTED);
            this.TabText = "Section View-" + m_strNetwork;
            this.Tag = "Section View-" + m_strNetwork;
            labelAttribute.Text = "Section View: " + m_strNetwork;

            //Load Facility/Route Filter
            String strSelect = "SELECT DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE TYPE_='STRING'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxFilterAttribute.Items.Add(row[0].ToString());
            }
            comboBoxRouteFacilty.Text = "All";
            FillRouteTable();
            UpdateSectionGrid();
            CreateSectionCategoryTabs();
            this.dgvSection.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSection_RowEnter);
            this.comboBoxRouteFacilty.SelectedIndexChanged += new System.EventHandler(this.comboBoxRouteFacilty_SelectedIndexChanged);

            // Add to the FormManager
            FormManager.AddFormSectionView(this);
        }

		protected void SecureForm()
		{
			LockDataGridView( dgvSection );
		}


        public String NetworkID
        {
            get { return m_strNetworkID; }
        }

        /// <summary>
        /// Fill the route table on the Attibute View.  Two versions of this.  One is filtered by
        /// Custom Filter, the other is not.
        /// </summary>
        private void FillRouteTable()
        {
            String strSelect = "SELECT DISTINCT FACILITY FROM SECTION_" + m_strNetworkID.ToString();
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            comboBoxRouteFacilty.Items.Clear();
            comboBoxRouteFacilty.Items.Add("All");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxRouteFacilty.Items.Add(row[0].ToString());
            }
        }

        private void UpdateSectionGrid()
        {
			bool IsLinear = false;
			if (checkMilepost.Checked)
			{
				IsLinear = true;
			}
			string strSelect = DBOp.GetAdvancedFilterSelectStatement(IsLinear, m_strNetworkID, comboBoxRouteFacilty.Text, textBoxAdvanceSearch.Text, comboBoxFilterAttribute.Text, comboBoxAttributeValue.Text);

            if (dataAdapter != null) dataAdapter.Dispose();// Free up the resources
            if(binding != null) binding.Dispose();
            if(table != null) table.Dispose();
            try
            {
                binding = new BindingSource();
                dataAdapter = new DataAdapter(strSelect);

                // Populate a new data table and bind it to the BindingSource.
                table = new DataTable();

                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                binding.DataSource = table;
                dgvSection.DataSource = binding;
                bindingNavigatorSectionView.BindingSource = binding;
                dgvSection.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;


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

        private void buttonAdvancedSearch_Click(object sender, EventArgs e)
        {
            FormAdvancedSearch formSearch = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear, textBoxAdvanceSearch.Text, false);
            if (formSearch.ShowDialog() == DialogResult.OK)
            {
                textBoxAdvanceSearch.Text = formSearch.GetWhereClause();
                UpdateSectionGrid();
            }
        }

        private void comboBoxRouteFacilty_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSectionGrid();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateSectionGrid();
        }

        private void CreateSectionCategoryTabs()
        {
			String strSelect;
			strSelect = "SELECT ATTRIBUTE_, GROUPING FROM ATTRIBUTES_";
            try
            {
                List<String> listAttributes;
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    String strAttribute = dr["ATTRIBUTE_"].ToString();
                    String strGrouping = dr["GROUPING"].ToString().ToUpper();

                    if (m_hashAttributeYear.Contains(strAttribute))
                    {
                        if (!m_hashGroupAttributeList.Contains(strGrouping))
                        {
                            listAttributes = new List<String>();
                            m_hashGroupAttributeList.Add(strGrouping,listAttributes);
                        }
                        else
                        {
                            listAttributes = (List<String>)m_hashGroupAttributeList[strGrouping];
                        }
                        listAttributes.Add(strAttribute);
                    }
                }

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                foreach (String strKey in m_hashGroupAttributeList.Keys)
                {
                    String strChecked = strKey;
                    if (strKey == "") strChecked = "Attributes";
                    TabPage page = new TabPage(strChecked);
                    page.Tag = strChecked;
                    page.Name = strChecked;


                    DataGridView dgv = new DataGridView();
                    dgv.Name = strChecked;
                    page.Controls.Add(dgv);
                    dgv.Dock = DockStyle.Fill;

                    listAttributes = (List<String>)m_hashGroupAttributeList[strKey];

                    int nMinYear = -1;
                    int nMaxYear = -1;

                    foreach (String strAttribute in listAttributes)
                    {
                        List<String> listYear = (List<String>)m_hashAttributeYear[strAttribute];
                        foreach (String strYear in listYear)
                        {
                            if (nMinYear < 0) nMinYear = int.Parse(strYear);
                            if (nMaxYear < 0) nMaxYear = int.Parse(strYear);


                            if (int.Parse(strYear) < nMinYear) nMinYear = int.Parse(strYear);
                            if (int.Parse(strYear) > nMaxYear) nMaxYear = int.Parse(strYear);

                        }
                    }
                    DataGridViewColumn columnMR = new DataGridViewColumn();
                    columnMR.Name = "Most Recent";
                    columnMR.HeaderText = "Most Recent";
                    columnMR.Tag = "Most Recent";
                    columnMR.CellTemplate = cell;
                    dgv.Columns.Add(columnMR);


                    //Add a colunm for nMinYear to nMaxYear + a most Recent
                    if (nMinYear > 0)
                    {
                        for (int i = nMaxYear; i >= nMinYear; i--)
                        {
                            DataGridViewColumn column = new DataGridViewColumn();
                            column.Name = i.ToString();
                            column.HeaderText = i.ToString();
                            column.Tag = i.ToString();
                            column.CellTemplate = cell;
                            dgv.Columns.Add(column);

                        }
                    }


                    foreach (String strAttribute in listAttributes)
                    {
                        int nRow = dgv.Rows.Add();
                        dgv.Rows[nRow].HeaderCell.Value = strAttribute;
                    }

                    dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
					LockDataGridView( dgv );
					//dgv.AllowUserToAddRows = false;
                    //dgv.AllowUserToDeleteRows = false;
                    tabControlSection.TabPages.Add(page);
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error creating categories. " + exc.Message);

            }
        }

        private void dgvSection_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
           // Global.WriteOutput("Facility: " + dgvSection[0, e.RowIndex].Value.ToString() + " Section: " + dgvSection[1, e.RowIndex].Value.ToString());
			String strID = dgvSection["SECTIONID", e.RowIndex].Value.ToString();
			m_selectedRowSectionID = strID;
            UpdateCategories(strID);

        }

        private void UpdateCategories(String strID)
        {
            List<String> listAttributes;
            List<String> listYear;


            String strSelect = "SELECT * FROM SEGMENT_" + m_strNetworkID + "_NS0 WHERE SECTIONID=" + strID;
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);

                if (ds.Tables[0].Rows[0] != null)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    foreach (String strKey in m_hashGroupAttributeList.Keys)
                    {
                        String strChecked = strKey;
                        if (strKey == "") strChecked = "Attributes";
                        listAttributes = (List<String>)m_hashGroupAttributeList[strKey];

                        TabPage page = tabControlSection.TabPages[strChecked];
                        DataGridView dgv = (DataGridView)page.Controls[strChecked];

                        int nRowIndex = 0;
                        foreach (String strAttribute in listAttributes)
                        {
                        
                            listYear = (List<String>)m_hashAttributeYear[strAttribute];
                            foreach (String strYear in listYear)
                            {
                                String strAttributeYear = strAttribute + "_" + strYear;
                                dgv[strYear, nRowIndex].Value = dr[strAttributeYear].ToString();
                            }

                            dgv["Most Recent", nRowIndex].Value = dr[strAttribute].ToString();
                            
                            nRowIndex++;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error retrieving section data. " + exc.Message);

            }

        }

		//private void buttonExport_Click(object sender, EventArgs e)
		//{
		//    ExportSectionReport();
		//}

		public void ExportSectionReport()
		{
			String strSearch = textBoxAdvanceSearch.Text.Trim();
			String strSelect;
			int nMinYear = 0, nMaxYear = 0;

			List<String> listAttributes;
			// Determine min and max year.  Used to limit report data to most current 8 years.
			foreach (String strKey in m_hashGroupAttributeList.Keys)
			{
				listAttributes = (List<String>)m_hashGroupAttributeList[strKey];

				foreach (String strAttribute in listAttributes)
				{
					List<String> listYear = (List<String>)m_hashAttributeYear[strAttribute];
					foreach (String strYear in listYear)
					{
						if (nMinYear < 0) nMinYear = int.Parse(strYear);
						if (nMaxYear < 0) nMaxYear = int.Parse(strYear);


						if (int.Parse(strYear) < nMinYear) nMinYear = int.Parse(strYear);
						if (int.Parse(strYear) > nMaxYear) nMaxYear = int.Parse(strYear);

					} // end strYear
				} // end strAttribute
			} // end strKey
			//nMinYear = nMaxYear - nMinYear > 6 ? nMaxYear - 6 : nMinYear; //test case
			nMinYear = nMaxYear - nMinYear > 7 ? nMaxYear - 7 : nMinYear;
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					strSelect = "SELECT FACILITY,SECTION,BEGIN_STATION AS [BEGIN], END_STATION AS [END],DIRECTION AS DIR";
					break;
				case "ORACLE":
					strSelect = "SELECT FACILITY,SECTION,BEGIN_STATION AS \"[BEGIN]\", END_STATION AS \"[END]\",DIRECTION AS DIR";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for buttonExport_Click()");
					//break;
			}
			foreach (String strKey in m_hashGroupAttributeList.Keys)
			{
				listAttributes = (List<String>)m_hashGroupAttributeList[strKey];

				foreach (String strAttribute in listAttributes)
				{
					List<String> listYear = (List<String>)m_hashAttributeYear[strAttribute];
					foreach (String strYear in listYear)
					{
						if (int.Parse(strYear) >= nMinYear && int.Parse(strYear) <= nMaxYear)
						{
							strSelect += "," + strAttribute + "_" + strYear;
						}
					}
				}
			}

			string fromClause = " FROM SECTION_" + m_strNetworkID + " INNER JOIN SEGMENT_" + m_strNetworkID + "_NS0 ON "
								+ "SECTION_" + m_strNetworkID + ".SECTIONID=SEGMENT_" + m_strNetworkID + "_NS0.SECTIONID ";

			//strSelect += " FROM SECTION_" + m_strNetworkID + " INNER JOIN SEGMENT_" + m_strNetworkID + "_NS0 ON "
			//                    + "SECTION_" + m_strNetworkID + ".SECTIONID=SEGMENT_" + m_strNetworkID + "_NS0.SECTIONID ";

			String strWhere = "";
			strSearch = textBoxAdvanceSearch.Text;
			strSearch = strSearch.Trim();

			if (comboBoxRouteFacilty.Text != "All")
			{
				strWhere = " WHERE FACILITY='" + comboBoxRouteFacilty.Text + "'";
				//strSelect += strWhere;

				if (strSearch != "")
				{
					strWhere += " AND ";
					strWhere += "(" + strSearch + ")";
				}
			}
			else
			{
				//This just can't be right.  You've never inserted a where clause here...
				//what could you possibly be ANDing?
				//if (strSearch != "")
				//{
				//    strSelect += " AND ";
				//    strSelect += "(" + strSearch + ")";
				//}

				if( strSearch != "" )
				{
					strWhere = " WHERE " + strSearch;
				}
			}
			string orderByClause = "";
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					orderByClause = " ORDER BY FACILITY, DIRECTION, [BEGIN]";
					break;
				case "ORACLE":
					orderByClause = " ORDER BY FACILITY, DIRECTION, \"[BEGIN]\"";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for buttonExport_Click()");
					//break;
			}
			string testSelectClause = "SELECT COUNT(*) " + fromClause + strWhere;

			try
			{
				int rowCount = DBMgr.ExecuteScalar( testSelectClause );
				if( rowCount <= 50 )
				{
					strSelect += fromClause + strWhere + orderByClause;
					DataSet ds = DBMgr.ExecuteQuery( strSelect );
					SectionViewReport svr = new SectionViewReport( ds, m_strNetwork, nMinYear, nMaxYear );
					svr.CreateSectionReport();
				}
				else
				{
					Global.WriteOutput( "ERROR: Choose a selection with 50 or fewer sections for this report." );
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error retrieving section data. " + exc.Message);

			}
		}

        private void FormSectionView_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveFormSectionView(this);
        }

		private void checkBoxCustomFilter_CheckedChanged( object sender, EventArgs e )
		{
			if (checkBoxCustomFilter.Checked)
			{
				checkBoxCustomFilter.Text = "";
				comboBoxFilterAttribute.Visible = true;
				comboBoxAttributeValue.Visible = true;
			}
			else
			{
				checkBoxCustomFilter.Text = "Enable Custom Filters";
				comboBoxFilterAttribute.Visible = false;
				comboBoxAttributeValue.Visible = false;
			}
		}

		private void comboBoxFilterAttribute_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool IsLinear = false;
			if (checkMilepost.Checked)
			{
				IsLinear = true;
			}
			string selectedAttribute = comboBoxFilterAttribute.Text;
			comboBoxAttributeValue.Items.Clear();
			comboBoxAttributeValue.Items.Add("All");
			String strSelect = "SELECT DISTINCT DATA_ FROM " + selectedAttribute;
			DataSet ds = DBMgr.ExecuteQuery(strSelect);
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				comboBoxAttributeValue.Items.Add(row[0].ToString());
			}
			comboBoxAttributeValue.Text = "All";
			
			string filterQuery = DBOp.GetAdvancedFilterSelectStatement(IsLinear, m_strNetworkID, comboBoxRouteFacilty.Text, textBoxAdvanceSearch.Text, comboBoxFilterAttribute.Text, comboBoxAttributeValue.Text);
		}

		private void comboBoxAttributeValue_SelectedIndexChanged(object sender, EventArgs e)
		{
			bool IsLinear = false;
			if (checkMilepost.Checked)
			{
				IsLinear = true;
			}
			string facility = comboBoxRouteFacilty.Text;
			string advancedSearchText = textBoxAdvanceSearch.Text;
			string property = comboBoxFilterAttribute.Text;
			string value = comboBoxAttributeValue.Text;
			DBOp.GetAdvancedFilterSelectStatement(IsLinear, m_strNetworkID, facility, advancedSearchText, property, value);
		}

        public override void NavigationTick(NavigationObject navigationObject)
        {
            NetworkNavigationObject nno = navigationObject.Networks.Find(delegate(NetworkNavigationObject network) { return network.NetworkID == this.NetworkID; });

            if (nno != null)
            {
                SectionObject sectionObject = nno.CurrentSection;

                if (sectionObject != m_lastSectionObject)
                {
                    if (sectionObject != null)
                    {
                        foreach (DataGridViewRow row in dgvSection.Rows)
                        {
                            if (row.Cells["FACILITY"].Value.ToString() == sectionObject.Facility && row.Cells["SECTION"].Value.ToString() == sectionObject.Section)
                            {

                                row.Selected = true;
                                UpdateCategories(sectionObject.SectionID);
                                ScrollGrid();
                            }
                            else
                            {
                                row.Selected = false;
                            }
                        }
                    }
                    m_lastSectionObject = sectionObject;
                }
            }
        }
        private void ScrollGrid()
        {
            int halfWay = (dgvSection.DisplayedRowCount(false) / 2);
            if (dgvSection.FirstDisplayedScrollingRowIndex + halfWay > dgvSection.SelectedRows[0].Index ||
                (dgvSection.FirstDisplayedScrollingRowIndex + dgvSection.DisplayedRowCount(false) - halfWay) <= dgvSection.SelectedRows[0].Index)
            {
                int targetRow = dgvSection.SelectedRows[0].Index;

                targetRow = Math.Max(targetRow - halfWay, 0);
                dgvSection.FirstDisplayedScrollingRowIndex = targetRow;

            }
        }

        private void dgvSection_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.ImageView)
            {
                //  this.Stop = true;
                int nIndex = e.RowIndex;
                String strFacility = dgvSection.Rows[nIndex].Cells["FACILITY"].Value.ToString();
                String strSection = dgvSection.Rows[nIndex].Cells["SECTION"].Value.ToString();
                String strDirection = "";
                String strBeginStation = "";
                NavigationEvent navigationEvent = null;

                String strSelect = "SELECT SECTIONID,DIRECTION,BEGIN_STATION FROM SECTION_" + m_strNetworkID + " WHERE FACILITY='" + strFacility + "' AND SECTION='" + strSection + "'";
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    String strID = row["SECTIONID"].ToString();
                    int nSectionID = int.Parse(strID);
                    if (nSectionID < 1000000)
                    {
                        strDirection = row["DIRECTION"].ToString();
                        strBeginStation = row["BEGIN_STATION"].ToString();
                        navigationEvent = new NavigationEvent(strFacility, strDirection, double.Parse(strBeginStation));
                    }
                    else
                    {
                        navigationEvent = new NavigationEvent(strFacility, strSection);
                    }
                }

                m_event.issueEvent(navigationEvent);
            }
            
        }

		private void buttonGraph_Click(object sender, EventArgs e)
		{

		}

		private void toolStripButtonARAN_Click(object sender, EventArgs e)
		{
			if (dgvSection.SelectedRows.Count == 1)
			{
				string videoLog = Environment.GetEnvironmentVariable("videolog").ToString();

				string routeID = "";
				string beginMP = "";
				string endMP = "";

				string sectionID = m_selectedRowSectionID;
				string query = "SELECT sec.BEGIN_STATION, sec.END_STATION, seg.AA_ROUTE_ID_COND FROM SECTION_" + m_strNetworkID + " sec INNER JOIN SEGMENT_" + m_strNetworkID + "_NS0 seg ON sec.SECTIONID = seg.SECTIONID WHERE sec.SECTIONID = '" + sectionID + "'";

				DataSet routeData = DBMgr.ExecuteQuery(query);
				foreach (DataRow dr in routeData.Tables[0].Rows)
				{
					routeID = dr["AA_ROUTE_ID_COND"].ToString();
					beginMP = dr["BEGIN_STATION"].ToString();
					endMP = dr["END_STATION"].ToString();
				}
				Global.WriteOutput(videoLog + "?routeId=" + routeID + "&mp=" + beginMP + "&dir=5");
				Process.Start(videoLog + "?routeId=" + routeID + "&mp=" + beginMP + "&dir=5");
			}
			else
			{
				Global.WriteOutput("Multiple Rows found!");
			}
		}

		//private void btnARANView_Click(object sender, EventArgs e)
		//{
		//    if (dgvSection.SelectedRows.Count == 1)
		//    {
		//        string videoLog = Environment.GetEnvironmentVariable("videolog").ToString();

		//        string routeID = "";
		//        string beginMP = "";
		//        string endMP = "";

		//        string sectionID = m_selectedRowSectionID;
		//        string query = "SELECT sec.BEGIN_STATION, sec.END_STATION, seg.AA_ROUTE_ID_COND FROM SECTION_" + m_strNetworkID + " sec INNER JOIN SEGMENT_" + m_strNetworkID + "_NS0 seg ON sec.SECTIONID = seg.SECTIONID WHERE sec.SECTIONID = '" + sectionID + "'";
				
		//        DataSet routeData = DBMgr.ExecuteQuery(query);
		//        foreach(DataRow dr in routeData.Tables[0].Rows)
		//        {
		//            routeID = dr["AA_ROUTE_ID_COND"].ToString();
		//            beginMP = dr["BEGIN_STATION"].ToString();
		//            endMP = dr["END_STATION"].ToString();
		//        }
		//        Global.WriteOutput(videoLog + "?routeId=" + routeID + "&mp=" + beginMP);
		//        Process.Start(videoLog + "?routeId=" + routeID +"&mp=" + beginMP);
		//    }
		//    else
		//    {
		//        Global.WriteOutput("Multiple Rows found!");
		//    }
		//}
    }
}
