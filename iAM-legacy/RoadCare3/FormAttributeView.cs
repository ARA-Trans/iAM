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
using RoadCareDatabaseOperations;
//using ReportExporters.WinForms.Adapters;
using System.IO;
using RoadCareGlobalOperations;
using DataObjects;
//using ReportExporters.Common.Adapters;
//using ReportExporters.WinForms;
//using ReportExporters.WinForms.Adapters;
//using ReportExporters.Common;
using System.Text.RegularExpressions;
using System.Diagnostics;
//using ReportExporters.Common;
//using ReportExporters.Common.Adapters;
//using ReportExporters.WinForms;

namespace RoadCare3
{
    public partial class FormAttributeView : BaseForm
    {
        String m_strNetwork;
        String m_strNetworkID;
        List<String> m_listColumns = new List<String>();

        BindingSource binding;
        DataAdapter dataAdapter;
        DataTable table;
        Hashtable m_hashAttributeYear;
        Hashtable m_hashSimulationID = new Hashtable();
        SectionObject m_lastSectionObject;

		List<string> treeNodeTags;
        
        public FormAttributeView(String strNetwork, Hashtable hashAttributeYear)
        {
            InitializeComponent();

			treeNodeTags = new List<string>();

            m_strNetwork = strNetwork;
            m_hashAttributeYear = hashAttributeYear;
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
            tip.SetToolTip(buttonUpdate, "Update data displayed in Attribute View dependent upon selected filters.");

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(buttonEditColumns, "Change the selected attributes and their display order.");

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;


            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(checkBoxCustomFilter, "Custom filter will limit output to sections identified by the filter");

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(buttonAdvancedSearch, "Create custom search which is used in conjunction with other filters.");

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(textBoxAdvanceSearch, "Current advanced search criteria.");

            tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(comboBoxRouteFacilty, "Limit Attribute View to Facility selected.");
        }

		public void GenerateReport()
		{
			try
			{
				AttributeViewReport avr = new AttributeViewReport( dgvAttributeView, m_strNetwork );
				avr.CreateAttributeReport();
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: Creating attribute view report. " + exc.Message );
			}
		}

		protected void SecureForm()
		{
			//shouldn't ever need to unlock, this is just a reporting tool basically
			LockDataGridView( dgvAttributeView );
		}

        private void FormAttributeView_Load(object sender, EventArgs e)
        {
			SecureForm();

            this.TabText = "Attribute-" + m_strNetwork;
            this.Tag = "Attribute-" + m_strNetwork;
            labelAttribute.Text = "Attribute View: " + m_strNetwork;

            //Load Facility/Route Filter
            String strSelect = "SELECT DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE TYPE_='STRING'";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxFilterAttribute.Items.Add(row[0].ToString());
            }

            FillRouteTable();
            comboBoxRouteFacilty.Text = "All";


            toolStripComboBoxSimulation.Items.Add("");
            strSelect = "SELECT SIMULATION,SIMULATIONID FROM SIMULATIONS WHERE NETWORKID='" + m_strNetworkID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    String strSimulation = dr["SIMULATION"].ToString();
                    String strSimulationID = dr["SIMULATIONID"].ToString();
                    m_hashSimulationID.Add(strSimulation, strSimulationID);
                    toolStripComboBoxSimulation.Items.Add(strSimulation);
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Loading simualtions." + exception.Message);
            }
        }

		private void SetAllAttributeIconsActive()
		{
			SetAllAttributeIcons(Settings.Default.ACTIVE_IMAGE_KEY, Settings.Default.ACTIVE_IMAGE_KEY_SELECTED);
		}

        public String NetworkID
        {
            get { return m_strNetworkID; }
        }

        public bool UpdateAttributeView(List<String> listNew, bool bReadCurrentColumns)
        {
            if (bReadCurrentColumns)
            {
                m_listColumns.Clear();
                foreach (DataGridViewColumn col in dgvAttributeView.Columns)
                {
                    if (col.HeaderText != "FACILITY" && col.HeaderText != "SECTION" && col.HeaderText != "BEGIN_STATION" && col.HeaderText != "END_STATION" && col.HeaderText != "DIRECTION")
                    {
                        m_listColumns.Add(col.HeaderText);
                    }
                }
            }

			//dsmelser 2008.08.01
			//Clear icons
			SetAllAttributeIconsInactive();
			treeNodeTags.Clear();

            foreach (String str in listNew)
            {
                if (!m_listColumns.Contains(str))
                {
                    m_listColumns.Add(str);
				}
			}


			foreach (String str in m_listColumns)
			{

				//dsmelser 2008.08.01
				//Store the appropriate Tags to change the correct icons
				int divideIndex = str.LastIndexOf('_');
				int throwaway;
				if (divideIndex >= 0)
				{
					string yearName = str.Substring(divideIndex + 1);
					if (Int32.TryParse(yearName, out throwaway))
					{
						string attributeName = str.Substring(0, str.Length - yearName.Length - 1);
						treeNodeTags.Add(m_strNetworkID + " " + attributeName + " " + yearName);
					}
					else
					{
						treeNodeTags.Add(m_strNetworkID + " " + str + " " + str);
					}
				}
				else
				{
					treeNodeTags.Add(m_strNetworkID + " " + str + " " + str);
				}
			}

			SetAllAttributeIconsActive();			

            String strSelect;
            if (checkMilepost.Checked)
            {
                strSelect = "SELECT FACILITY,SECTION,BEGIN_STATION,END_STATION,DIRECTION";
            }
            else
            {
                strSelect = "SELECT FACILITY,SECTION";
            }


            String strSimulation = toolStripComboBoxSimulation.Text;
            String strSimulationID = "";
            if (strSimulation != "")
            {
                strSimulationID = m_hashSimulationID[strSimulation].ToString();

            }

            List<string> listColumns = new List<string>();
            string simulationTable = "SIMULATION_" + m_strNetworkID.ToString() + "_" + strSimulationID.ToString();
            if (!string.IsNullOrWhiteSpace(strSimulationID))
            {
               listColumns = DBMgr.GetTableColumns(simulationTable);
            }


            ////Then each of the input columns
            foreach (String str in m_listColumns)
            {

                strSelect += ",";
                if (listColumns.Contains(str)) strSelect += simulationTable + ".";
                strSelect += str;
            }

            String strFrom = DBOp.BuildFromStatement(m_strNetworkID, strSimulationID, true);
            strSelect += strFrom;


            String strWhere = "";
            String strSearch = textBoxAdvanceSearch.Text;
            strSearch = strSearch.Trim();

            if (comboBoxRouteFacilty.Text != "All")
            {
                strWhere = " WHERE FACILITY='" + comboBoxRouteFacilty.Text + "'";
                strSelect += strWhere;
            }
			if( strSearch != "" )
			{
				strSelect += " AND ";
				strSelect += "(" + strSearch + ")";
			}
			if( checkBoxCustomFilter.Checked )
			{
				if( comboBoxFilterAttribute.Text.Trim() != "" )
				{
					if( comboBoxAttributeValue.Text.Trim() != "All" )
					{
						strSelect += " AND ";
						strSelect += "(" + comboBoxFilterAttribute.Text.Trim() + " = '" + comboBoxAttributeValue.Text.Trim() + "')";
					}
				}
			}

            // SELECT field1, field2, field3
            // FROM first_table
            // INNER JOIN second_table
            // ON first_table.keyfield = second_table.foreign_keyfield
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
                dgvAttributeView.DataSource = binding;
                bindingNavigatorAttributeView.BindingSource = binding;
                dgvAttributeView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

                for (int i = 0; i < m_listColumns.Count; i++)
                {
                    String strCol = m_listColumns[i].ToString();
                    if(checkMilepost.Checked)
					{
                        dgvAttributeView.Columns[strCol].DisplayIndex = i + 5;
					}
                    else
					{
                        dgvAttributeView.Columns[strCol].DisplayIndex = i + 2;
					}

					dgvAttributeView.Columns[strCol].DefaultCellStyle.Format = GetAttributeFormat( strCol );
                }

				if( dgvAttributeView.Columns["BEGIN_STATION"] != null )
				{
					dgvAttributeView.Columns["BEGIN_STATION"].DefaultCellStyle.Format = "f2";
				}
				if( dgvAttributeView.Columns["END_STATION"] != null )
				{
					dgvAttributeView.Columns["END_STATION"].DefaultCellStyle.Format = "f2";
				}

                if (checkMilepost.Checked)
                {
                    dgvAttributeView.Columns["FACILITY"].DisplayIndex = 0;
                    dgvAttributeView.Columns["SECTION"].DisplayIndex = 1;
                    dgvAttributeView.Columns["BEGIN_STATION"].DisplayIndex = 2;
                    dgvAttributeView.Columns["END_STATION"].DisplayIndex = 3;
                    dgvAttributeView.Columns["DIRECTION"].DisplayIndex = 4;
                }
                else
                {
                    dgvAttributeView.Columns["FACILITY"].DisplayIndex = 0;
                    dgvAttributeView.Columns["SECTION"].DisplayIndex = 1;
                }

				
				//update with validation stuff
				UpdateValidation();

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Building current attribute view, try re-rolling up the network. " + exception.Message);
                return false;
            }
            return true;
        }

		private string GetAttributeFormat( string rolledUpAttributeColumnName )
		{
			string attributeName = GetAttributeNameFromRolledUpColumn( rolledUpAttributeColumnName );
			string format = DBMgr.ExecuteQuery( "SELECT FORMAT FROM ATTRIBUTES_ WHERE ATTRIBUTE_ = '" + attributeName + "'" ).Tables[0].Rows[0]["FORMAT"].ToString();

			return format;
		}

		private string GetAttributeNameFromRolledUpColumn( string rolledUpAttributeColumnName )
		{
			string attributeName = rolledUpAttributeColumnName;
			Regex yearFilter = new Regex( "_[1-9][0-9][0-9][0-9]$" );
			Regex initialValueFilter = new Regex("_[0]$");
			if (yearFilter.IsMatch(rolledUpAttributeColumnName))
			{
				attributeName = rolledUpAttributeColumnName.Substring(0, rolledUpAttributeColumnName.Length - 5);
			}
			else
			{
				if(initialValueFilter.IsMatch(rolledUpAttributeColumnName))
				{
					attributeName = rolledUpAttributeColumnName.Substring(0, rolledUpAttributeColumnName.Length - 2);
				}
			}

			return attributeName;
		}

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateAttributeView(new List<String>(),true);

			ToolTip tip = new ToolTip();
			tip.InitialDelay = 1000;
			tip.ReshowDelay = 500;
			// Force the ToolTip text to be displayed whether or not the form is active.
			tip.ShowAlways = true;
			tip.SetToolTip( buttonUpdate, "Update data displayed in Attribute View dependent upon selected filters." );

		}

        private void buttonEditColumns_Click(object sender, EventArgs e)
        {
            EditColumns();
            ToolTip tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(buttonEditColumns, "Change the selected attributes and their display order.");
        }

        private void EditColumns()
        {
            m_listColumns.Clear();
            foreach (DataGridViewColumn col in dgvAttributeView.Columns)
            {
                if (col.HeaderText != "FACILITY" && col.HeaderText != "SECTION" && col.HeaderText != "BEGIN_STATION" && col.HeaderText != "END_STATION" && col.HeaderText != "DIRECTION")
                {
                    m_listColumns.Add(col.HeaderText);
                }
            }

            String strSimulation = toolStripComboBoxSimulation.Text;
            String strSimulationID = "";
            if (strSimulation != "")
            {
                strSimulationID = m_hashSimulationID[strSimulation].ToString();

            }

            Hashtable hashAttributeYear = Global.GetAttributeYear(m_strNetworkID,strSimulationID);

            FormAttributeViewSelectColumns formColumns = new FormAttributeViewSelectColumns(m_listColumns, hashAttributeYear);
            if (formColumns.ShowDialog() == DialogResult.OK)
            {
                m_listColumns = formColumns.m_listColumns;
                UpdateAttributeView(new List<String>(), false);

            }
        }

		//private void buttonSummary_Click(object sender, EventArgs e)
		//{
		//    //Get unique years from LANES

		//    if(!m_hashAttributeYear.Contains("LANES"))
		//    {    
		//        Global.WriteOutput("Error: Attribute LANES required to calculate network averages");
		//        return;
		//    }
		//    if(!checkMilepost.Checked)
		//    {
		//        Global.WriteOutput("Error: Show Begin/End/Directions must be checked to generate summary");
		//        return;
		//    }

		//    if (m_listColumns.Count == 0)
		//    {
		//        Global.WriteOutput("Error: At least one attribute must be selected for calculation of summary.");
		//        return;

		//    }            
            
		//    List<string> listYear = (List<string>)m_hashAttributeYear["LANES"];
		//    //
		//    // Find years for analysis
		//    List<string> listLaneYear = new List<string>();
		//    Hashtable hashColumnAttribute = new Hashtable();
		//    List<string> listAttribute = new List<string>();

		//    foreach (string str in m_listColumns)
		//    {
		//        if(str.Length > 4)
		//        {
		//            string strYear = str.Substring(str.Length - 4);
		//            try
		//            {
		//                int nYear = int.Parse(strYear);
		//                if (nYear < 1900 && nYear > 2100)
		//                {
		//                    continue;
		//                }
		//                if(!listLaneYear.Contains(nYear.ToString()))
		//                {
		//                    listLaneYear.Add(nYear.ToString());
		//                    hashColumnAttribute.Add(str, str.Substring(0, str.Length - 5));
		//                    if(!listAttribute.Contains(str.Substring(0, str.Length - 5)))
		//                    {
		//                        listAttribute.Add(str.Substring(0, str.Length - 5));
		//                    }
		//                }
		//            }
		//            catch
		//            {
		//                hashColumnAttribute.Add(str, str);
		//                if (!listAttribute.Contains(str))
		//                {
		//                    listAttribute.Add(str);
		//                }
		//                if(listLaneYear.Contains(""))continue;
		//                listLaneYear.Add("");
		//            }
		//        }
		//        else
		//        {
		//            hashColumnAttribute.Add(str, str);
		//            if (!listAttribute.Contains(str))
		//            {
		//                listAttribute.Add(str);
		//            }

		//            if(listLaneYear.Contains(""))continue;
		//            listLaneYear.Add("");
		//        }
		//    }
            

		//    String strSelect;
		//    strSelect = "SELECT FACILITY,SECTION";

		//    ////Then each of the input columns
		//    foreach (String str in listLaneYear)
		//    {
		//        strSelect += ",";
		//        if(str != "" && listYear.Contains(str))
		//        {
		//            strSelect += "LANES_";
		//            strSelect +=str;
		//        }
		//        else
		//        {
		//            strSelect += "LANES";
		//        }
		//    }

		//    //Then at a minumum
		//    strSelect += " FROM SECTION_" + m_strNetworkID.ToString();

		//    // Now for each table that is attached we add
		//    strSelect += " INNER JOIN SEGMENT_" + m_strNetworkID.ToString() + "_NS0 ON SECTION_" + m_strNetworkID.ToString() + ".SECTIONID=SEGMENT_" + m_strNetworkID + "_NS0.SECTIONID";


		//    String strWhere = "";
		//    String strSearch = textBoxAdvanceSearch.Text;
		//    strSearch = strSearch.Trim();

		//    if (comboBoxRouteFacilty.Text != "All")
		//    {
		//        strWhere = " WHERE FACILITY='" + comboBoxRouteFacilty.Text + "'";
		//        strSelect += strWhere;

		//        if (strSearch != "")
		//        {
		//            strSelect += " AND ";
		//            strSelect += "(" + strSearch + ")";
		//        }
		//    }
		//    else
		//    {
		//        if (strSearch != "")
		//        {
		//            strSelect += " AND ";
		//            strSelect += "(" + strSearch + ")";
		//        }

		//    }

		//    //SqlDataReader dr = DBMgr.CreateDataReader(strSelect);
		//    DataReader dr = new DataReader(strSelect);
		//    //IDataReader idr = (IDataReader)new DataReader("BLAH");

		//    //For each column, determine which year to use for lanes.
		//    Hashtable hashColumnYear = new Hashtable();
  

		//    foreach(string str in m_listColumns)
		//    {
		//        if (str.Length > 4)
		//        {
		//            string strYear = str.Substring(str.Length - 4);
		//            try
		//            {
		//                int nYear = int.Parse(strYear);
		//                if (nYear < 1900 && nYear > 2100)
		//                {
		//                    hashColumnYear.Add(str,"LANES");
		//                }
		//                else if (listYear.Contains(nYear.ToString()))
		//                {
		//                    hashColumnYear.Add(str, "LANES_" + nYear.ToString());

		//                    if (!hashColumnAttribute.Contains(str))
		//                    {
		//                        hashColumnAttribute.Add(str, str.Substring(0, str.Length - 5));
		//                    }
		//                }
		//                else
		//                {
		//                    hashColumnYear.Add(str,"LANES");
		//                }
		//            }
		//            catch
		//            {
		//                hashColumnYear.Add(str, "LANES");
		//            }
		//        }
		//        else
		//        {
		//            hashColumnYear.Add(str, "LANES");
		//        }
		//    }
		//    //For each attribute lookup ASCENDING,LEVEL1,LEVEL2,LEVEL3,LEVEL4,LEVEL5
		//    //Calculate non-blank area for each attribute_year
		//    //(Column heading)  -attribute_year store either float[6] or hashStringArea
		//    float[] fTotalArea = new float[m_listColumns.Count];
		//    foreach (DataGridViewRow row in dgvAttributeView.Rows)
		//    {
		//        dr.Read();

		//        string strEnd = row.Cells[3].Value.ToString();
		//        string strBegin = row.Cells[2].Value.ToString();

		//        float fEnd = 0;
		//        float fBegin = 0;
		//        float.TryParse(strEnd, out fEnd);
		//        float.TryParse(strBegin, out fBegin);

		//        float dDelta = Math.Abs(fEnd - fBegin);
		//        if (dDelta <= 0) dDelta = 1;

		//        int nIndex = 0;//First data column.
		//        foreach (string column in m_listColumns)
		//        {
		//            string lane = hashColumnYear[column].ToString();
		//            lane = dr[lane].ToString();
		//            int nLane = 1;
		//            int.TryParse(lane, out nLane);

		//            float fArea = dDelta * (float)nLane;

		//            fTotalArea[nIndex] += fArea;
		//            nIndex++;
		//        }
		//    }
		//    dr.Close();
		//    return;
		//}

        private void checkBoxCustomFilter_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCustomFilter.Checked)
            {
                checkBoxCustomFilter.Text = "";
                comboBoxFilterAttribute.Visible = true;
                comboBoxAttributeValue.Visible = true;
            }
            else
            {
                checkBoxCustomFilter.Text = "Enable custom filter:";
                comboBoxFilterAttribute.Visible = false;
                comboBoxAttributeValue.Visible = false;
            }
        }

        private void comboBoxFilterAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strAttribute = comboBoxFilterAttribute.Text;
            comboBoxAttributeValue.Items.Clear();
            comboBoxAttributeValue.Items.Add("All");
            String strSelect = "SELECT DISTINCT DATA_ FROM " + strAttribute;
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                comboBoxAttributeValue.Items.Add(row[0].ToString());

            }
            comboBoxAttributeValue.Text = "All";
        }

        private void comboBoxAttributeValue_SelectedIndexChanged(object sender, EventArgs e)
        {
			UpdateAttributeView( new List<String>(), true );
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

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateAttributeView(new List<String>(),true);
        }

        private void editColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditColumns();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dgvAttributeView.GetClipboardContent());
        }

        private void buttonAdvancedSearch_Click(object sender, EventArgs e)
        {
            String strSimulationID = "";
            String strSimulation = toolStripComboBoxSimulation.Text;
            if (strSimulation == "") strSimulationID = "";
            else
            {
                strSimulationID = m_hashSimulationID[strSimulation].ToString();
            }

            FormAdvancedSearch formSearch = new FormAdvancedSearch(m_strNetworkID, strSimulationID,textBoxAdvanceSearch.Text);
            if (formSearch.ShowDialog() == DialogResult.OK)
            {
                textBoxAdvanceSearch.Text = formSearch.GetWhereClause();
                UpdateAttributeView(new List<String>(),true);
            }

        }

        private void FormAttributeView_FormClosed(object sender, FormClosedEventArgs e)
        {
			SetAllAttributeIconsInactive();
            FormManager.RemoveAttributeViewForm(this);
        }

		private void SetAllAttributeIconsInactive()
		{
			SetAllAttributeIcons( Settings.Default.ATTRIBUTE_VIEW_SPECIFIC_YEAR_IMAGE_KEY,
				Settings.Default.ATTRIBUTE_VIEW_SPECIFIC_YEAR_IMAGE_KEY_SELECTED);
		}

		private void SetAllAttributeIcons(string normalKey, string selectedKey)
		{
			foreach (string seekTag in treeNodeTags)
			{
				ChangeAssociatedIcon(seekTag, normalKey, selectedKey);
			}
		}

        private void checkMilepost_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAttributeView(new List<String>(),false);
        }

        private void comboBoxRouteFacilty_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAttributeView(new List<String>(), false);
        }

		//private void buttonExport_Click(object sender, EventArgs e)
		//{
		//    //DataGridView dgvTest = new DataGridView();
		//    //Create DataGridViewReportDataAdapter instance
		//    IReportDataAdapter reportDataAdapter = new DataGridViewReportDataAdapter(dgvAttributeView);

		//    //Create WinFormsReportExporter instance for reportDataAdapter
		//    IReportExporter winFormsReportExporter = new WinFormsReportExporter(reportDataAdapter);

		//    //Execute method ExportToXls to get Excel file content
		//    MemoryStream xlsFileData = winFormsReportExporter.ExportToXls();
		//}

        /// <summary>
        /// Update for ImageView
        /// </summary>
        public override void UpdateNode(DockPanel dockPanel, object ob)
        {
            if (!this.ImageView)
            {
                this.ImageView = true;
                this.HideOnClose = true;
            }
            List<String> listAttribute = (List<String>)ob;
            UpdateAttributeView(listAttribute, false);

            this.Show(dockPanel);
        }


        public override void NavigationTick(NavigationObject navigationObject)
        {
            NetworkNavigationObject nno = navigationObject.Networks.Find(delegate(NetworkNavigationObject network) { return network.NetworkID == this.NetworkID; });

            if(nno != null)
            {
                SectionObject sectionObject = nno.CurrentSection;

                if(sectionObject != m_lastSectionObject)
                {
                    if (sectionObject != null)
                    {
                        foreach (DataGridViewRow row in dgvAttributeView.Rows)
                        {
                            if (row.Cells["FACILITY"].Value.ToString() == sectionObject.Facility && row.Cells["SECTION"].Value.ToString() == sectionObject.Section)
                            {

                                row.Selected = true;
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
            int halfWay = (dgvAttributeView.DisplayedRowCount(false) / 2);
            if (dgvAttributeView.FirstDisplayedScrollingRowIndex + halfWay > dgvAttributeView.SelectedRows[0].Index ||
                (dgvAttributeView.FirstDisplayedScrollingRowIndex + dgvAttributeView.DisplayedRowCount(false) - halfWay) <= dgvAttributeView.SelectedRows[0].Index)
            {
                int targetRow = dgvAttributeView.SelectedRows[0].Index;

                targetRow = Math.Max(targetRow - halfWay, 0);
                dgvAttributeView.FirstDisplayedScrollingRowIndex = targetRow;

            }
        }

        private void dgvAttributeView_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.ImageView)
            {
              //  this.Stop = true;
                int nIndex = e.RowIndex;
                String strFacility = dgvAttributeView.Rows[nIndex].Cells["FACILITY"].Value.ToString();
                String strSection = dgvAttributeView.Rows[nIndex].Cells["SECTION"].Value.ToString();
                String strDirection = "";
                String strBeginStation = "";
                NavigationEvent navigationEvent=null;
            
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
                        navigationEvent = new NavigationEvent(strFacility,strDirection,double.Parse(strBeginStation));
                    }
                    else
                    {
                        navigationEvent = new NavigationEvent(strFacility,strSection);
                    }
                }

                m_event.issueEvent(navigationEvent);
            }
            
        }

		private void btnARAN_Click(object sender, EventArgs e)
		{
			if (dgvAttributeView.SelectedRows.Count == 1)
			{
				string videoLog = Environment.GetEnvironmentVariable("videolog").ToString();

				string routeID = "";
				string beginMP = "";
				string endMP = "";

				string strFacility = dgvAttributeView.SelectedRows[0].Cells["FACILITY"].Value.ToString();
				string strSection = dgvAttributeView.SelectedRows[0].Cells["SECTION"].Value.ToString();
				string sectionID = "";

                String strSelect = "SELECT SECTIONID,DIRECTION,BEGIN_STATION FROM SECTION_" + m_strNetworkID + " WHERE FACILITY='" + strFacility + "' AND SECTION='" + strSection + "'";
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					sectionID = row["SECTIONID"].ToString();
				}
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

		private void setLimitsToolStripMenuItem_Click( object sender, EventArgs e )
		{
			buttonValidation_Click( null, null );
		}

		private void setFontToolStripMenuItem_Click( object sender, EventArgs e )
		{
			buttonFont_Click( null, null );
		}

		private void buttonFont_Click( object sender, EventArgs e )
		{
			
		}

		private void buttonValidation_Click( object sender, EventArgs e )
		{
			
		}

		private void UpdateValidation()
		{
			Regex yearStripper = new Regex( @".*(?=_[0-9][0-9][0-9][0-9]$)" );
			List<AttributeValidationData> attributesWithValidation = GetValidationData();
			foreach( DataGridViewColumn columnToCheck in dgvAttributeView.Columns )
			{
				string columnName = columnToCheck.HeaderText;
				string attributeName = yearStripper.Match( columnName ).ToString();
				if( attributeName == "" )
				{
					attributeName = columnName;
				}
				AttributeValidationData range = attributesWithValidation.Find( delegate( AttributeValidationData a )
				{
					return a.Name == attributeName;
				} );
				if( range != null )
				{
					ValidateColumn( columnToCheck, range );
				}
			}
		}

		private List<AttributeValidationData> GetValidationData()
		{
			List<AttributeValidationData> toReturn = new List<AttributeValidationData>();

			TextReader validationFileReader = LoadValidationFile();
			string attributeRow;
			while( ( attributeRow = validationFileReader.ReadLine() ) != null )
			{
				string[] attributeCells = attributeRow.Split( '\t' );

				string name = attributeCells[0];
				double min;
				double max;

				if( !double.TryParse( attributeCells[1], out min ) )
				{
					min = double.NaN;
				}
				if( !double.TryParse( attributeCells[2], out max ) )
				{
					max = double.NaN;
				}

				AttributeValidationData toAdd = new AttributeValidationData( name, min, max );
				toReturn.Add( toAdd );
			}
			validationFileReader.Close();

			return toReturn;
		}

		private void ValidateColumn( DataGridViewColumn columnToCheck, AttributeValidationData range )
		{
			foreach( DataGridViewRow rowToCheck in dgvAttributeView.Rows )
			{
				DataGridViewCell cellToCheck = rowToCheck.Cells[columnToCheck.Name];
				double cellValue;
				if( double.TryParse( cellToCheck.Value.ToString(), out cellValue ) )
				{
					if( cellValue > range.Max || cellValue < range.Min )
					{
						cellToCheck.Style.BackColor = Color.Red;
					}
					else
					{
						cellToCheck.Style.BackColor = SystemColors.Window;
					}
				}
			}
		}


		private TextReader LoadValidationFile()
		{
			string validationFilePath = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\RoadCare Projects\attributevalidation.txt";
			FileInfo validationFileInfo = new FileInfo( validationFilePath );

			if( !validationFileInfo.Exists )
			{
				CreateValidationFile();
			}

			TextReader toReturn = new StreamReader( validationFilePath );
			return toReturn;
		}

		private void CreateValidationFile()
		{
			TextWriter attributeFileWriter = new StreamWriter( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ) + @"\RoadCare Projects\attributevalidation.txt" );
			DataSet numericAttributeData = DBMgr.ExecuteQuery( "SELECT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE TYPE_ = 'NUMBER'" );
			foreach( DataRow numericAttributeRow in numericAttributeData.Tables[0].Rows )
			{
				string toWrite = numericAttributeRow[0].ToString() + "\t\t";
				attributeFileWriter.WriteLine( toWrite );
			}
			attributeFileWriter.Close();
		}

		private void FormAttributeView_LocationChanged( object sender, EventArgs e )
		{
			UpdateValidation();
		}

		private void topToggleToolStripMenuItem_Click( object sender, EventArgs e )
		{
			if( splitContainer1.Panel1Collapsed )
			{
				//going to uncollapse
				topToggleToolStripMenuItem.Text = "Hide Top";
			}
			else
			{
				//going to collapse
				topToggleToolStripMenuItem.Text = "Show Top";
			}
			splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
		}

		private void dgvAttributeView_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{

		}

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save the current attribute format to an xml file.
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML Files | *.xml";
            sfd.DefaultExt = "xml";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataTable toBindTo =(DataTable)((BindingSource)dgvAttributeView.DataSource).DataSource;
                toBindTo.TableName = "AttributeView";
                toBindTo.WriteXml(sfd.FileName);
            }
        }

        private void validationLimitsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormAttributeValidation limitScreen = new FormAttributeValidation();
            if (limitScreen.ShowDialog() == DialogResult.OK)
            {
                UpdateValidation();
            }
        }

        private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog d = new FontDialog();
            if (d.ShowDialog() == DialogResult.OK)
            {
                dgvAttributeView.DefaultCellStyle.Font = d.Font;
                dgvAttributeView.AutoResizeRows();
                dgvAttributeView.AutoResizeColumns();
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditColumns();
            ToolTip tip = new ToolTip();
            tip.InitialDelay = 1000;
            tip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            tip.ShowAlways = true;
            tip.SetToolTip(buttonEditColumns, "Change the selected attributes and their display order.");
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Load the selected attribute view into the datagridview.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML Files | *.xml";
            ofd.DefaultExt = "xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                DataSet toBindTo = new DataSet();
                toBindTo.ReadXml(ofd.FileName);
                dgvAttributeView.DataSource = toBindTo.Tables[0];
            }
        }



	
    }

	public class AttributeValidationData
	{
		string _name;
		double _min = double.NaN;
		double _max = double.NaN;

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public double Min
		{
			get
			{
				return _min;
			}
		}

		public double Max
		{
			get
			{
				return _max;
			}
		}

		public AttributeValidationData( string name, double min, double max )
		{
			_name = name;
			_min = min;
			_max = max;
		}
	}

}
