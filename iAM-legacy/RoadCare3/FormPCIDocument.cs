using AppliedResearchAssociates.PciDistress;
using DatabaseManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RoadCare3
{
	/// <summary>
	/// Allows the display and entering of PCI data for each
	/// individual section.
	/// </summary>
	public partial class FormPCIDocument : BaseForm
	{
        int m_nNextPCIID;
        private bool m_bUpdate = false;
		private List<String> m_listFacility = new List<String>();

		private DataAdapter pciDataAdapter;
		private DataAdapter pciDetailDataAdapter;
		private DataTable pciTable;
		private DataTable pciDetailsTable;

		private DataSet currentRouteSectionsDataSet;

		private BindingSource pciBindingSource;

		List<string> availableRoutesList;
        DataGridViewComboBoxCell facilityComboBoxCell;
		DataGridViewComboBoxCell routesComboBoxCell;
		DataGridViewComboBoxCell methodologiesComboBoxCell;

        Hashtable m_hashMethodDistress;//Key - method, value is hash of distresses and numbers
        Hashtable m_hashMethodCombo;


		private string m_strLinearYearFilter;
		private string m_strLinearRouteFilter;
		private string m_strSectionYearFilter;
		private string m_strSectionRouteFilter;
        private string m_strPreviousMethod;
		// Declare an internal class for the combo box
		public class DataGridViewSampleTypeComboBoxCell : DataGridViewComboBoxCell
		{
			public DataGridViewSampleTypeComboBoxCell()
			{
				this.Items.Add("Random");
				this.Items.Add("Additional");
				this.Items.Add("Ignore");
			}

			public override object Clone()
			{
				return base.Clone();
			}
		}

		// constructor
		public FormPCIDocument()
		{
			InitializeComponent();

			m_strLinearYearFilter = "";
			m_strLinearRouteFilter = "";
			m_strSectionYearFilter = "";
			m_strSectionRouteFilter = "";

			currentRouteSectionsDataSet = null;


			//TODO:  Find out how the section reference is determined
			// at this point in the program.

			rbSectionRef.Checked = false;
			rbLinearRef.Checked = true;

			FormatPCIFilters();
			CreateDictionaries();
			CreateComboBoxesForGrid();
            m_nNextPCIID = RoadCareGlobalOperations.GlobalDatabaseOperations.GetNextPCIID();
		}

		#region page loading functions
		/// <summary>
		/// Createst the dictionaries that hold the definitions
		/// of the various distresses.
		/// </summary>
		private void CreateDictionaries()
		{
            m_hashMethodDistress = RoadCareGlobalOperations.GlobalDatabaseOperations.GetPCIDistresses();

		}

		/// <summary>
		/// The main function that refreshes the screen and all database
		/// information displayed on the form.
		/// </summary>
		private void RefreshScreen()
		{
			dgvwPCI.DataSource = null;

			try
			{
				this.LoadPCITable();
				this.RefreshDetailsScreen();
			}
			catch (Exception exc)
			{
				string msg = String.Format("Database Error: {0}", exc.Message);
				Debug.WriteLine(msg);
			}

			return;

		}


		/// <summary>
		/// Refreshes the information for the PCI detailed distress information.
		/// </summary>
		private void RefreshDetailsScreen()
		{
			ConnectionParameters connectionParams = DBMgr.NativeConnectionParameters;
			string strSelect = "";
			string strMethod = "";
			int nIndex = 0;
			DataGridViewRow currentRow = null;


			try
			{
                if (dgvPCIDetail.Rows.Count > 0)
                {
                    dgvPCIDetail.Rows.Clear();
                }
            }

			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
            try
            {
				currentRow = dgvwPCI.CurrentRow;
				if (currentRow != null)
				{
					if (currentRow.Cells["METHOD_"].Value == null)
					{
						return;
					}
					else
					{
						strMethod = currentRow.Cells["METHOD_"].Value.ToString();
						DataGridViewComboBoxCell dgvCombo = (DataGridViewComboBoxCell)m_hashMethodCombo[strMethod];
						dgvPCIDetail.Columns["DISTRESS"].CellTemplate = dgvCombo;
					}
				}
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}

			try
			{
				if  ( (dgvwPCI.CurrentRow != null) && (dgvwPCI.CurrentRow.Tag != null) )
				{

					strSelect = BuildSQLStringForPCIDetailsTable();

					pciDetailsTable = new DataTable();
					pciDetailDataAdapter = new DataAdapter(strSelect, connectionParams);
					pciDetailDataAdapter.Fill(pciDetailsTable);

					foreach (DataRow dr in pciDetailsTable.Rows)
					{
						nIndex = dgvPCIDetail.Rows.Add();
						dgvPCIDetail.Rows[nIndex].Cells["DETAIL_ID"].Value = dr["ID_"].ToString();
						dgvPCIDetail.Rows[nIndex].Cells["DETAIL_PCI_ID"].Value = dr["PCI_ID"].ToString();
						dgvPCIDetail.Rows[nIndex].Cells["DISTRESS"].Value = dr["DISTRESS"].ToString().Trim();
						dgvPCIDetail.Rows[nIndex].Cells["SEVERITY"].Value = dr["SEVERITY"].ToString();
						dgvPCIDetail.Rows[nIndex].Cells["AMOUNT"].Value = dr["AMOUNT"].ToString();
						dgvPCIDetail.Rows[nIndex].Cells["EXTENT_"].Value = dr["EXTENT_"].ToString();
						dgvPCIDetail.Rows[nIndex].Cells["DEDUCTVALUE"].Value = dr["DEDUCTVALUE"].ToString();


						dgvPCIDetail.Rows[nIndex].Tag = dr["ID_"].ToString();

					}
				}
				////  Set the display for the columns in the detail
				//// screen

				dgvPCIDetail.Columns["DETAIL_ID"].Visible = false;
				dgvPCIDetail.Invalidate();
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
				string msg = String.Format("Failure getting the detail for this sample");
			}

			return;
		}


		/// <summary>
		/// Format the filters from the top of the page that filter
		/// by route and date.
		/// </summary>
		private void FormatPCIFilters()
		{
			// First clear the years combo box, allow an All option at the top of the combo box.
			cbYear.Items.Clear();

			if (rbLinearRef.Checked)
			{
				if (m_strLinearYearFilter != "")
				{
					cbYear.Text = m_strLinearYearFilter;
				}
				else
				{
					cbYear.Text = "All";
				}
			}
			else
			{
				if (m_strSectionYearFilter != "")
				{
					cbYear.Text = m_strSectionYearFilter;
				}
				else
				{
					cbYear.Text = "All";
				}
			}

			cbYear.Items.Add("All");
			String strQuery = "";
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					strQuery = "Select DISTINCT year(DATE_) AS Years From [dbo].[PCI] ORDER BY Years";
					break;
				case "ORACLE":
					strQuery = "SELECT DISTINCT TO_CHAR( DATE_, 'YYYY' ) as Years From PCI ORDER BY Years";
					break;
				default:
					throw new NotImplementedException( "ToDo: Implement ANSI SQL implementation for FormatPCIFilters()" );
					//break;
			}

			DataSet ds = null;

			ds = DBMgr.ExecuteQuery(strQuery);
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				cbYear.Items.Add(row.ItemArray[0].ToString());
			}

			// Now to fill in the routes cb, first we clear it though, and then we give and set its default value.
			cbRoutes.Items.Clear();

			cbRoutes.Items.Add("All");
			if (rbLinearRef.Checked)
			{
				strQuery = "Select DISTINCT ROUTES FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL ORDER BY ROUTES";
				lblRouteFacility.Text = "Route:";
			}
			else
			{
				strQuery = "Select DISTINCT FACILITY FROM NETWORK_DEFINITION WHERE FACILITY IS NOT NULL ORDER BY FACILITY";
				lblRouteFacility.Text = "Facility:";
			}

			ds = DBMgr.ExecuteQuery(strQuery);
			availableRoutesList = new List<string>(ds.Tables[0].Rows.Count);
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				cbRoutes.Items.Add(row.ItemArray[0].ToString());
				m_listFacility.Add(row.ItemArray[0].ToString());
				availableRoutesList.Add(row.ItemArray[0].ToString());
			}

			if (rbLinearRef.Checked)
			{
				if (m_strLinearRouteFilter != "")
				{
					cbRoutes.Text = m_strLinearRouteFilter;
				}
				else
				{
					//cbRoutes.Text = "All";
					if (cbRoutes.Items.Count >= 2)
					{
						cbRoutes.SelectedIndex = 1;
					}
					else
					{
						cbRoutes.Text = "All";
					}
				}
			}
			else
			{
				if (m_strSectionRouteFilter != "")
				{
					cbRoutes.Text = m_strSectionRouteFilter;
				}
				else
				{
					//cbRoutes.Text = "All";
					if (cbRoutes.Items.Count >= 2)
					{
						cbRoutes.SelectedIndex = 1;
					}
					else
					{
						cbRoutes.Text = "All";
					}
				}
			}
		}

		private void rbLinearRef_CheckedChanged(object sender, EventArgs e)
		{
            if (m_bUpdate && rbLinearRef.Checked)
            {
                FormatPCIFilters();
				CreateComboBoxesForGrid();
                this.RefreshScreen();
            }
		}

		private void rbSectionRef_CheckedChanged(object sender, EventArgs e)
		{
            if (m_bUpdate && rbSectionRef.Checked)
            {
				FormatPCIFilters();
				CreateComboBoxesForGrid();
                this.RefreshScreen();
            }
            
		}

		private void FormPCIDocument_Load(object sender, EventArgs e)
		{

			cbRoutes.SelectedIndex = 0;
			cbYear.SelectedIndex = 0;
            m_bUpdate = true;
			RefreshScreen();

			return;

		}

		/// <summary>
		/// This function is called one tmie only to create all  the
		/// various combo boxes that are needed inside both the PCI and
		/// the details grid view objects.
		/// </summary>
		private void CreateComboBoxesForGrid()
        {
            m_hashMethodCombo = new Hashtable();
            List<String> listMethods = RoadCareGlobalOperations.GlobalDatabaseOperations.GetPCIMethods();
            foreach (String strMethod in listMethods)
            {
                DataGridViewComboBoxCell dgvCombo = new DataGridViewComboBoxCell();
                dgvCombo.Sorted = true;
                Hashtable hashDistress = (Hashtable)m_hashMethodDistress[strMethod];
                foreach (DictionaryEntry de in hashDistress)
                {
                    dgvCombo.Items.Add(de.Key.ToString());
                }
                m_hashMethodCombo.Add(strMethod, dgvCombo);
            }

            methodologiesComboBoxCell = new DataGridViewComboBoxCell();
            for (int i = 0; i < listMethods.Count; i++)
            {
                methodologiesComboBoxCell.Items.Add(listMethods[i]);
            }

            routesComboBoxCell = new DataGridViewComboBoxCell();
            foreach (string s in availableRoutesList)
            {
                routesComboBoxCell.Items.Add(s);
            }

            facilityComboBoxCell = new DataGridViewComboBoxCell();
			foreach (string s in availableRoutesList)
			{
				facilityComboBoxCell.Items.Add(s);
			}
        


            dgvwPCI.Columns["ROUTES"].CellTemplate = routesComboBoxCell;
            dgvwPCI.Columns["FACILITY"].CellTemplate = facilityComboBoxCell;
            dgvwPCI.Columns["METHOD_"].CellTemplate = methodologiesComboBoxCell;


            return;
        }

		/// <summary>
		/// Loads the PCI grid with the section-level PCI information.
		/// </summary>
		private void LoadPCITable()
		{
			DataTable dataTable = new DataTable();
			ConnectionParameters connectionParams = DBMgr.NativeConnectionParameters;
			try
			{
				string strSelect;

				strSelect = BuildSQLStringForPCITable();
				pciTable = new DataTable();
				pciTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

				pciBindingSource = new BindingSource();
				pciDataAdapter = new DataAdapter(strSelect, connectionParams);
				pciDataAdapter.Fill(pciTable);

				dgvwPCI.Rows.Clear();

				int nIndex = 0;


				dgvwPCI.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

				// set the appropriate combo boxes for the grid
				dgvwPCI.Columns["ROUTES"].CellTemplate = routesComboBoxCell;
				dgvwPCI.Columns["METHOD_"].CellTemplate = methodologiesComboBoxCell;

				// Load the section-level information in the PCI grid view object.
				// Fill all columns, and hid the columns that are not available
				// because of section-based or linear-based selections.
				foreach (DataRow dr in pciTable.Rows)
				{
					nIndex = dgvwPCI.Rows.Add();
					dgvwPCI.Rows[nIndex].Cells["ROUTES"].Value = dr["ROUTES"].ToString();
					dgvwPCI.Rows[nIndex].Cells["DIRECTION"].Value = dr["DIRECTION"].ToString();
					dgvwPCI.Rows[nIndex].Cells["BEGIN_STATION"].Value = dr["BEGIN_STATION"].ToString();
					dgvwPCI.Rows[nIndex].Cells["END_STATION"].Value = dr["END_STATION"].ToString();
					dgvwPCI.Rows[nIndex].Cells["FACILITY"].Value = dr["FACILITY"].ToString();
					dgvwPCI.Rows[nIndex].Cells["SECTION"].Value = dr["SECTION"].ToString();
					dgvwPCI.Rows[nIndex].Cells["SAMPLE_"].Value = dr["SAMPLE_"].ToString();
					dgvwPCI.Rows[nIndex].Cells["AREA"].Value = dr["AREA"].ToString();
					dgvwPCI.Rows[nIndex].Cells["METHOD_"].Value = dr["METHOD_"].ToString();
					dgvwPCI.Rows[nIndex].Cells["SAMPLE_TYPE"].Value = dr["SAMPLE_TYPE"].ToString();
                    DateTime date = (DateTime)dr["DATE_"];
				    dgvwPCI.Rows[nIndex].Cells["DATE_"].Value = date.ToString("MM/dd/yyyy");
					//dgvwPCI.Rows[nIndex].Cells["DATE_"].Value = dr["DATE_"].ToString("MM/dd/yyyy");
					dgvwPCI.Rows[nIndex].Cells["PCI"].Value = dr["PCI"].ToString();

					dgvwPCI.Rows[nIndex].Tag = dr["ID_"].ToString();
				}

				if (rbLinearRef.Checked)
				{

					dgvwPCI.Columns["ID_"].Visible = false;
					dgvwPCI.Columns["FACILITY"].Visible = false;
					dgvwPCI.Columns["SECTION"].Visible = false;
					dgvwPCI.Columns["SAMPLE_"].Visible = false;

                    dgvwPCI.Columns["ROUTES"].Visible = true;
                    dgvwPCI.Columns["BEGIN_STATION"].Visible = true;
                    dgvwPCI.Columns["END_STATION"].Visible = true;
                    dgvwPCI.Columns["DIRECTION"].Visible = true;

				}
				else
				{
					dgvwPCI.Columns["ID_"].Visible = false;
					dgvwPCI.Columns["ROUTES"].Visible = false;
					dgvwPCI.Columns["BEGIN_STATION"].Visible = false;
					dgvwPCI.Columns["END_STATION"].Visible = false;
                    dgvwPCI.Columns["DIRECTION"].Visible = false;

                    dgvwPCI.Columns["FACILITY"].Visible = true;
                    dgvwPCI.Columns["SECTION"].Visible = true;
                    dgvwPCI.Columns["SAMPLE_"].Visible = true;

				}

               //bindingNavigatorPCI.BindingSource.DataSource =  pciTable;
               // dgvwPCI.Columns["DATE"].DefaultCellStyle.Format = "MM/dd/yyyy";
               // bindingNavigatorPCI.BindingSource = pciBindingSource;
			}
			catch (Exception exception)
			{
				Global.WriteOutput("Error: Error binding the view tables." + exception.Message);
				return;
			}
			return;
		}

		protected void RecalculateAllPCI()
		{
			DataTable pciDetailsTable = null;
			StringBuilder sbDeduct = null;
			DataTable dataTable = new DataTable();
			ConnectionParameters connectionParams = DBMgr.NativeConnectionParameters;
			double dPCI = 0.0;
			double dExtent = 0.0;
			double dArea = 0.0;
			double dDistressArea = 0.0;
			double dSumDeduct = 0.0;
			double dDeduct = 0.0;

			string strPCISelect = BuildSQLStringForPCITable();
			pciTable = new DataTable();
			pciTable.Locale = System.Globalization.CultureInfo.InvariantCulture;

			pciBindingSource = new BindingSource();
			pciDataAdapter = new DataAdapter(strPCISelect, connectionParams);
			pciDataAdapter.Fill(pciTable);

			foreach (DataRow dr in pciTable.Rows)
			{
				string section = dr[1].ToString();
				if (section.Contains("TWB"))
					System.Diagnostics.Debug.WriteLine("Section A17B1");

				string id = dr["ID_"].ToString();
				string area = dr["AREA"].ToString();
				string method = dr["METHOD_"].ToString();
				string sample = dr["SAMPLE_"].ToString();
				string sampleType = dr["SAMPLE_TYPE"].ToString();
				string inspectedSlabs = "";
				int iInspectedSlabs = 0;

				method = method.Trim();

				try
				{
					// We can't use the table query here because it looks for the
					// PCI_ID in the table.  Here we substitute 
					string pciDetailsQuery = BuildSQLStringForPCIDetailsQuery(id);


					pciDetailsTable = new DataTable();
					pciDetailDataAdapter = new DataAdapter(pciDetailsQuery, connectionParams);
					pciDetailDataAdapter.Fill(pciDetailsTable);
					dSumDeduct = 0.0;
					sbDeduct = new StringBuilder();

					foreach (DataRow detailsRow in pciDetailsTable.Rows)
					{
						string detailID = detailsRow["ID_"].ToString();
						string pciID = detailsRow["PCI_ID"].ToString();
						string distress = detailsRow["DISTRESS"].ToString();
						string severity = detailsRow["SEVERITY"].ToString();
						string sampleAmount = detailsRow["AMOUNT"].ToString();

						if (severity.Equals("N")) 
							continue;

						// Get the distress number from the distress name.

						// calculate the deducts
						if (method.Equals("pcc.faa") || method.Equals("pcc.mpr") ||
							method.Equals("pcc.astm"))
						{
							inspectedSlabs = dr["AREA"].ToString();
							dDeduct = CalculateCurrentDeducts(detailsRow, method, inspectedSlabs);
							string deduct = String.Format("{0:f2}", dDeduct);
							iInspectedSlabs = int.Parse(inspectedSlabs);
							double dSampleAmount = double.Parse(sampleAmount);
							dExtent = (double) (dSampleAmount / (double)iInspectedSlabs) * 100.0;
							dSumDeduct += dDeduct;

							sbDeduct.Append(deduct);
							sbDeduct.Append(",");
							detailsRow["EXTENT_"] = dExtent;
							detailsRow["DEDUCTVALUE"] = dDeduct;
						}
						else if (method.Equals("ac.faa") || method.Equals("ac.mpr") ||
							method.Equals("ac.astm"))
						{
							double.TryParse(area, out dArea);
							if (dArea > 0)
							{
								dDeduct = CalculateCurrentDeducts(detailsRow, method, area);
								double.TryParse(sampleAmount, out dDistressArea);
								dExtent = (dDistressArea / dArea) * 100.0;
								dSumDeduct += dDeduct;
								string deduct = String.Format("{0:f2}", dDeduct);
								sbDeduct.Append(deduct);
								sbDeduct.Append(",");

								detailsRow["EXTENT_"] = dExtent;
								detailsRow["DEDUCTVALUE"] = dDeduct;
							}
						}
					}

					if (sbDeduct.Length > 0)
					{
						sbDeduct.Remove(sbDeduct.Length - 1, 1);
						pciDetailDataAdapter.Update(pciDetailsTable);

						pciDetailsTable.Dispose();
						pciDetailDataAdapter.Dispose();

						dPCI = PciDistress.ComputePCIValue(sbDeduct.ToString(), method.Trim());
						dr["PCI"] = FormatPCIAsString(dPCI);
					}
				}
				catch (Exception exc1)
				{
					System.Diagnostics.Debug.WriteLine(exc1.Message);
				}



			}

			pciDataAdapter.Update(pciTable);
			//pciTable.Dispose();
			//pciDataAdapter.Dispose();

			return;

		}

		#endregion

		#region database operations
		/// <summary>
		/// Creates a select statement formatted for the filling of the
		/// PCI (section-level) table.
		/// </summary>
		/// <returns></returns>
		/// <remarks>It is a requirement that the select statement contain the keys
		/// for the PCI table or the updates will not work.
		/// </remarks>
		private string BuildSQLStringForPCITable()
		{
			StringBuilder sbSelect = new StringBuilder();
			Boolean bAddAnd = false;
			string strWhereClause = "";

			sbSelect.Append("SELECT ");
			string strLinearColumns;
			strLinearColumns = "ID_, ROUTES, DIRECTION, BEGIN_STATION, END_STATION, FACILITY, SECTION, SAMPLE_, AREA, METHOD_, SAMPLE_TYPE, DATE_, PCI";
			sbSelect.Append( strLinearColumns );
            if (rbLinearRef.Checked == true)
			{
				strWhereClause = " ROUTES = '" + cbRoutes.Text + "'";
			}
			else
			{
				strWhereClause = " FACILITY = '" + cbRoutes.Text + "'";
			}
			sbSelect.Append( " From PCI");
            
            if(rbLinearRef.Checked)
                sbSelect.Append(" WHERE ROUTES IS NOT NULL ");
            else
                sbSelect.Append(" WHERE FACILITY IS NOT NULL ");




			if (((cbRoutes.Text == "All") || (cbRoutes.Text == "")) &&
				((cbYear.Text == "All") || (cbYear.Text == "")))
			{
				// no where statement
				;
			}
			else
			{
				sbSelect.Append(" AND ");

				if ((cbRoutes.Text != "All") && (cbRoutes.Text != ""))
				{
					sbSelect.Append(strWhereClause);
					bAddAnd = true;
				}
				if ((cbYear.Text != "All") && (cbYear.Text != ""))
				{
					if (bAddAnd)
						sbSelect.Append(" AND ");
					//dsmelser 2009.01.30
					//this isn't the line as it was adjusted originally.
					//For now I'm not going to touch it.
					//However, this will fail with Oracle.
					//Date is a keyword that needs to be escaped (double quotes) to work.
					//Also, YEAR() is not a function in oracle.
					//instead, something like TO_CHAR(\"DATE\",'YYYY') = yeartobechecked needs to be used
						switch( DBMgr.NativeConnectionParameters.Provider )
						{
							case "MSSQL":
								sbSelect.Append( " YEAR(DATE_)='" );
								sbSelect.Append( cbYear.Text );
								break;
							case "ORACLE":
								sbSelect.Append( " TO_CHAR(DATE_,'YYYY') ='" );
								sbSelect.Append( cbYear.Text );
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for BuildSQLStringForPCITable()" );
								//break;
						}
					sbSelect.Append( "' ORDER BY DATE_" );

				}
			}

			return sbSelect.ToString();
		}


		/// <summary>
		/// Creates	A SQL query string to obtain the distress information for
		/// one section.
		/// </summary>
		/// <returns>The SQL string used to return the information
		/// in the details table.</returns>
		private String BuildSQLStringForPCIDetailsTable()
		{
			StringBuilder sbSelect = new StringBuilder();
			string strID = "";

			if (dgvwPCI.CurrentRow.Tag == null)
			{
				throw new Exception("There is no tag set in the current row in the PCI table.");
			}

			strID = dgvwPCI.CurrentRow.Tag.ToString();
			return BuildSQLStringForPCIDetailsQuery(strID);
		}
		/// <summary>
		/// Creates a SQL query string to obtain the distress information for
		/// one section.
		/// </summary>
		/// <param name="strID">The id of the sample record in the PCI table</param>
		/// <returns>The SQL string used for the PCI details query</returns>
		private String BuildSQLStringForPCIDetailsQuery(string strID)
		{
			StringBuilder sbSelect = new StringBuilder();

			sbSelect.Append("SELECT ");
			sbSelect.Append(" ID_, PCI_ID,");
			sbSelect.Append("DISTRESS, SEVERITY, AMOUNT, EXTENT_, DEDUCTVALUE ");
			sbSelect.Append(" From PCI_DETAIL");

			sbSelect.Append(" WHERE PCI_ID = ");
			sbSelect.Append(strID);

			return sbSelect.ToString();
		}
		/// <summary>
		/// Inserts a new record in the database based on the entries
		/// made in the PCI data grid view.
		/// </summary>
		/// <remarks>The tag value of the row of the data grid view
		/// contains the key that is auto-generated in the database.</remarks>
		/// <param name="currentRowIndex">The currently selected row in
		/// the data grid view</param>
		/// <param name="currentColumnIndex">The column that has been updated
		/// and contains values to insert into the database.</param>
		private void InsertNewPCIRecord(int currentRowIndex, int currentColumnIndex)
		{
			int nRowsAffected = 0;
			string strValue = "";
			string strName = "";
			StringBuilder sbInsert = new StringBuilder();

			strName = dgvwPCI.Rows[currentRowIndex].Cells[currentColumnIndex].OwningColumn.Name.ToString();
			strValue = dgvwPCI.Rows[currentRowIndex].Cells[currentColumnIndex].Value.ToString();

			sbInsert.Append("INSERT INTO PCI (ID_,");
			sbInsert.Append(strName);
			sbInsert.Append(" ) VALUES (" + m_nNextPCIID + ",");

			sbInsert.Append("'");
			sbInsert.Append(strValue);
			sbInsert.Append("'");

			sbInsert.Append(" ) ");

			try
			{
				nRowsAffected = DBMgr.ExecuteNonQuery(sbInsert.ToString());

			}
			catch (Exception exc)
			{
				//Debug.WriteLine(exc.Message);
				MessageBox.Show("Failed to insert the new record: " + exc.Message);
			}

            dgvwPCI.Rows[currentRowIndex].Tag = m_nNextPCIID;
            m_nNextPCIID++;

			return;
		}

		/// <summary>
		///  Updates the database with new data entered in one column of the PCI
		///  data grid view.  The tag value of the indicated row of the PCI data
		///  grid view must be set prior to this function or an exception will 
		///  be generated.
		/// </summary>
		/// <param name="currentRowIndex">Currently selected row in the PCI data grid view</param>
		/// <param name="currentColumnIndex">The updated column from the PCI
		/// data grid view.</param>
		private void UpdatePCIRecord(int currentRowIndex, int currentColumnIndex)
		{
			int nRowsAffected = 0;
			string strValue = "";
			string strName = "";
			string strPCI = "";
			StringBuilder sbUpdate = new StringBuilder();

			strName = dgvwPCI.Rows[currentRowIndex].Cells[currentColumnIndex].OwningColumn.Name.ToString();
			strValue = dgvwPCI.Rows[currentRowIndex].Cells[currentColumnIndex].Value.ToString();
            if (dgvwPCI.Rows[currentRowIndex].Cells["PCI"].Value != null)
            {
                strPCI = dgvwPCI.Rows[currentRowIndex].Cells["PCI"].Value.ToString();
            }
			if ((strValue.Equals(""))) return;

			sbUpdate.Append("UPDATE PCI SET ");
			sbUpdate.Append(strName);
			sbUpdate.Append(" = ");

			sbUpdate.Append("'");
			sbUpdate.Append(strValue);
			sbUpdate.Append("'");


			if ((strPCI != "") && (strName != "PCI"))
			{
				sbUpdate.Append(", PCI = ");
				sbUpdate.Append(strPCI);
			}

				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						sbUpdate.Append( " WHERE ID_ = " );
						break;
					case "ORACLE":
						sbUpdate.Append( " WHERE \"ID_\" = " );
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for UpdatePCIRecord()" );
						//break;
				}
			sbUpdate.Append(dgvwPCI.Rows[currentRowIndex].Tag.ToString());

			try
			{
				nRowsAffected = DBMgr.ExecuteNonQuery(sbUpdate.ToString());
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
				MessageBox.Show(exc.Message);
			}

			return;

		}

		/// <summary>
		/// Inserts a new record in the PCI_Details table based on the entry of a
		/// value in a column of the PCIDetails data grid view.
		/// </summary>
		/// <param name="currentRowIndex">Currently selected row of the PCI Details
		/// data grid view.</param>
		/// <param name="currentColumnIndex">The column number of the data grid view
		/// that is to be updated.</param>
		private void InsertNewPCIDetailRecord(int currentRowIndex, int currentColumnIndex)
		{

			int nRowsAffected = 0;
			string strValue = "";
			string strName = "";
			string strPCI_ID = "";

			// get the current value of the matching PCI record
			DataGridViewRow currentPCIRow = dgvwPCI.CurrentRow;

			if (currentPCIRow.Tag == null)
			{
				throw new Exception("No current PCI Row.");
			}
			if (dgvPCIDetail.Rows[currentRowIndex].Cells[currentColumnIndex].Value == null)
			{
				return;
			}

			strPCI_ID = currentPCIRow.Tag.ToString();
			strName = dgvPCIDetail.Rows[currentRowIndex].Cells[currentColumnIndex].OwningColumn.Name.ToString();
			strValue = dgvPCIDetail.Rows[currentRowIndex].Cells[currentColumnIndex].Value.ToString();

			if (strValue == "")
				return;

			StringBuilder sbInsert = new StringBuilder();
			sbInsert.Append("INSERT INTO PCI_DETAIL ( ");
			int index = strName.IndexOf("DETAIL_");
			if (index > -1)
			{
				strName = strName.Substring(index + 7);
			}
			sbInsert.Append(strName);
			sbInsert.Append(", PCI_ID ");
			sbInsert.Append(" ) VALUES ( ");

			if ((strName == "DISTRESS") || (strName == "SEVERITY"))
			{
				sbInsert.Append("'");
				sbInsert.Append(strValue);
				sbInsert.Append("'");
			}
			else
			{
				sbInsert.Append(strValue);
			}

			sbInsert.Append(", ");
			sbInsert.Append(strPCI_ID);
			sbInsert.Append(" ) ");


			try
			{
				nRowsAffected = DBMgr.ExecuteNonQuery(sbInsert.ToString());
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
				MessageBox.Show(exc.Message);
			}


			String strIdentity = "";
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						strIdentity = "SELECT IDENT_CURRENT ('PCI_DETAIL') FROM PCI_DETAIL";
						break;
					case "ORACLE":
						//strIdentity = "SELECT PCI_DETAIL_SEQ.CURRVAL FROM DUAL";
						//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'PCI_DETAIL_SEQ'";
						strIdentity = "SELECT MAX(ID_) FROM PCI_DETAIL";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
						//break;
				}

			try
			{
				DataSet ds = DBMgr.ExecuteQuery(strIdentity);
				strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
				dgvPCIDetail.Rows[currentRowIndex].Tag = strIdentity;
			}
			catch (Exception exc1)
			{
				// Note:  This error will occur if there is no key value or
				// if the database has not identified the ID field as an
				// identity.
				Debug.WriteLine("Failed to find the current detail record:" + exc1.Message);
				MessageBox.Show("Could not locate the current detail record.");
			}
			
			
			return;
		}

		/// <summary>
		/// Updates an existing record int he PCI_Details database table based on
		/// an entry in a single column of the PCIDetails data grid view.
		/// </summary>
		/// <param name="currentRowIndex">The row that has been edited in the
		/// PCI Details data grid view</param>
		/// <param name="currentColumnIndex">The column containning the value to be
		/// updated in the PCIDetail data grid view.</param>
		private void UpdateNewPCIDetailRecord(int currentRowIndex, int currentColumnIndex)
		{
			int nRowsAffected = 0;
			string strValue = "";
			string strName = "";
			StringBuilder sbUpdate = new StringBuilder();
			DataGridViewRow currentPCIRow = dgvwPCI.CurrentRow;
			ConnectionParameters cp = DBMgr.NativeConnectionParameters;

			if (currentPCIRow == null)
				throw new Exception("No valid PCI Row");

			try
			{
				strName = dgvPCIDetail.Rows[currentRowIndex].Cells[currentColumnIndex].OwningColumn.Name.ToString();
				strValue = dgvPCIDetail.Rows[currentRowIndex].Cells[currentColumnIndex].Value.ToString();

				if (strValue.Equals(""))
					return;

				int index = strName.IndexOf("DETAIL_");
				if (index > -1)
				{
					strName = strName.Substring(index + 7);
				}

				sbUpdate.Append("UPDATE PCI_DETAIL SET ");
				sbUpdate.Append(strName);
				sbUpdate.Append(" = ");

				if ((strName == "DISTRESS") || (strName == "SEVERITY"))
				{
					sbUpdate.Append("'");
					sbUpdate.Append(strValue);
					sbUpdate.Append("'");
				}
				else
				{
					sbUpdate.Append(strValue);

				}

				if (dgvPCIDetail.Rows[currentRowIndex].Cells["EXTENT_"].Value != null)
				{
					if (!String.IsNullOrEmpty(dgvPCIDetail.Rows[currentRowIndex].Cells["EXTENT_"].Value.ToString()))
					{
						sbUpdate.Append(", EXTENT_ = ");
						sbUpdate.Append(dgvPCIDetail.Rows[currentRowIndex].Cells["EXTENT_"].Value.ToString());
					}
				}

				if (dgvPCIDetail.Rows[currentRowIndex].Cells["DEDUCTVALUE"].Value != null)
				{
					if (!String.IsNullOrEmpty(dgvPCIDetail.Rows[currentRowIndex].Cells["DEDUCTVALUE"].Value.ToString()))
					{
						string strDeductValue = dgvPCIDetail.Rows[currentRowIndex].Cells["DEDUCTVALUE"].Value.ToString();
						if (!(strDeductValue.Equals("NaN")))
						{
							sbUpdate.Append(", DEDUCTVALUE = ");
							sbUpdate.Append(dgvPCIDetail.Rows[currentRowIndex].Cells["DEDUCTVALUE"].Value.ToString());
						}
					}
				}

				switch( cp.Provider )
				{
					case "MSSQL":
						sbUpdate.Append( " WHERE ID_ = " );
						break;
					case "ORACLE":
						sbUpdate.Append( " WHERE \"ID_)\" = " );
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for UpdateNewPCIDetailRecord()" );
					//break;
				}
				sbUpdate.Append(dgvPCIDetail.Rows[currentRowIndex].Tag.ToString());

				string sqlUpdateCommand = sbUpdate.ToString();
				Debug.WriteLine(sqlUpdateCommand);
				nRowsAffected = DBMgr.ExecuteNonQuery(sqlUpdateCommand, cp);
				string msg = String.Format("Rows updated: {0:D}", nRowsAffected);
				Debug.WriteLine(msg);
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}
			return;
		}

		private DataSet GetSectionsForRoute(string routeName)
		{
			DataSet routeDataSet = null;
			string strQuery = "SELECT ID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM NETWORK_DEFINITION WHERE ROUTES = '" + routeName + "'";
			try
			{
				routeDataSet = new DataSet();
				routeDataSet = DBMgr.ExecuteQuery(strQuery);

			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
				MessageBox.Show("Section information could not be obtained for this route");
			}


			return routeDataSet;
		}
		private bool GetCurrentSectionsDataSet()
		{
			string strRouteName = "";
			bool isValidRoute = false;

			try
			{
				if (dgvwPCI.CurrentRow.Cells["ROUTES"].Value != null)
				{
					strRouteName = dgvwPCI.CurrentRow.Cells["ROUTES"].Value.ToString();
					if (!String.IsNullOrEmpty(strRouteName))
					{
						currentRouteSectionsDataSet = this.GetSectionsForRoute(strRouteName);
					}
					else
					{
						if (currentRouteSectionsDataSet != null)
							currentRouteSectionsDataSet.Dispose();
						currentRouteSectionsDataSet = null;
					}

					if (currentRouteSectionsDataSet != null)
						isValidRoute = true;
				}
				else
				{
					if (currentRouteSectionsDataSet != null)
						currentRouteSectionsDataSet.Dispose();
					currentRouteSectionsDataSet = null;
				}
			}
			catch (Exception exc)
			{
				Debug.WriteLine("Error in GetCurrentSectionsDataSet: " + exc.Message);
				isValidRoute = false;
			}
			return isValidRoute;

		}

		// This is commented out because we currently aren't checking the values of
		// the input data for validity.  This code is untested.
		//private bool IsValidBeginStation(string strRouteName, string strBeginStation)
		//{
		//    bool isValid = false;
		//    double dBeginStation = 0.0;
		//    double dTestBeginStation = 0.0;
		//    double dTestEndStation = 0.0;

		//    string strTestBeginStation = "";
		//    string strTestEndStation = "";

		//    try
		//    {
		//        dBeginStation = Double.Parse(strBeginStation);

		//        foreach (DataRow row in currentRouteSectionsDataSet.Tables[0].Rows)
		//        {
		//            strTestBeginStation = row["BEGIN_STATION"].ToString();
		//            strTestEndStation = row["END_STATION"].ToString();

		//            if ((String.IsNullOrEmpty(strTestBeginStation)) && (String.IsNullOrEmpty(strTestEndStation)))
		//            {
		//                dTestBeginStation = Double.Parse(strTestBeginStation);
		//                dTestEndStation = Double.Parse(strTestEndStation);

		//                if (dBeginStation >= dTestBeginStation)
		//                {
		//                    isValid = true;
		//                }
		//            }
		//        }

		//    }
		//    catch (Exception exc)
		//    {
		//        Debug.WriteLine("Error in IsValidBeginStation: " + exc.Message);
		//    }


		//    return isValid;
		//}
		
		#endregion

		#region computation routines

		/// <summary>
		/// Uses the distress value in the current selection of the Details
		/// data grid view to compute the deducts for that distress.
		/// </summary>
		/// <param name="currentRow">The row in the PCI Details data grid view
		/// that is currently selected.</param>
		private void CalculateCurrentRowDeducts(DataGridViewRow currentRow)
		{

			int nDistress = 0;
			string strDistress = "";
			string strAmount = "";
			string strArea = "";
			string strExtent = "";
			string sSeverity = "";
			string strMethod = "";
			double dAmount = 0.0;
			double dSamsiz = 0.0;
			double dPCIDeduct = 0.0;
			double dExtent = 0.0;
			string pciDeductString = "";
			try
			{
				if ((currentRow.Cells["DISTRESS"].Value == null) || (currentRow.Cells["SEVERITY"].Value == null)
					|| (currentRow.Cells["AMOUNT"].Value == null) || (dgvwPCI.CurrentRow.Cells["METHOD_"].Value == null))
					return;

                
				strDistress = currentRow.Cells["DISTRESS"].Value.ToString();
                sSeverity = currentRow.Cells["SEVERITY"].Value.ToString();
                strAmount = currentRow.Cells["AMOUNT"].Value.ToString();


				strArea = dgvwPCI.CurrentRow.Cells["AREA"].Value.ToString();
				strMethod = dgvwPCI.CurrentRow.Cells["METHOD_"].Value.ToString();
				if ((String.IsNullOrEmpty(strMethod)) || (String.IsNullOrEmpty(strDistress)) ||
						(String.IsNullOrEmpty(strAmount)) || (String.IsNullOrEmpty(strArea)) ||
						(String.IsNullOrEmpty(sSeverity)))
				{
					pciDeductString = "";
					strExtent = "";
				}
				else
				{
                    Hashtable hashDistress = (Hashtable)m_hashMethodDistress[strMethod];
 					nDistress = int.Parse(hashDistress[strDistress].ToString());
                    

					if (String.IsNullOrEmpty(strAmount))
						dAmount = 0.0;
					else
						dAmount = Convert.ToDouble(strAmount);

					if (String.IsNullOrEmpty(strArea))
						dSamsiz = 0.0;
					else
						dSamsiz = Convert.ToDouble(strArea);

					if (dSamsiz > 0.0)
						dExtent = (dAmount / dSamsiz) * 100.0;
					else
						dExtent = 0.0;

					// Get the distress information from the PCI object and
					// display it in the detail grid.
					if (!PciDistress.IsWASHCLKMethod(strMethod))
                    {

                        dPCIDeduct = PciDistress.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);
                        pciDeductString = dPCIDeduct.ToString("f2");
                        strExtent = dExtent.ToString("f5");
                    }
                    else
                    {
						// Clark County uses a different definition of the extent
						// whe computing deducts.  Don't multiply times 100.
						dExtent = dAmount / dSamsiz;
                        dPCIDeduct = PciDistress.pvt_ComputeNonPCIDeduct(strMethod,nDistress, sSeverity, dExtent);
                        pciDeductString = dPCIDeduct.ToString("f2");
                        strExtent = dExtent.ToString("f5");
                    }

				}

				currentRow.Cells["DEDUCTVALUE"].Value = pciDeductString;
				currentRow.Cells["EXTENT_"].Value = strExtent;

			}
			catch (Exception exc)
			{
				Debug.WriteLine("Exception in CalculateCurrentRowDeducts: " + exc.Message);
				dgvPCIDetail.CurrentRow.Cells["DEDUCTVALUE"].Value = DBNull.Value;
			}
			return;
		}

		/// <summary>
		/// Calculates the current deducts for one row of the
		/// PCI detail table.
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		private double CalculateCurrentDeducts(DataRow row, string strMethod, string strArea)
		{
			int nDistress = 0;
			double dAmount = 0.0;
			double dPCIDeduct = 0.0;
			double dExtent = 0.0;
			double dSamsiz = 0.0;

			string strDistress = "";
			string strAmount = "";
			string sSeverity = "";

			try
			{

				strDistress = row["DISTRESS"].ToString();
				sSeverity = row["SEVERITY"].ToString();
				strAmount = row["AMOUNT"].ToString();

				if ((String.IsNullOrEmpty(strMethod)) || (String.IsNullOrEmpty(strDistress)) ||
						(String.IsNullOrEmpty(strAmount)) || (String.IsNullOrEmpty(strArea)) ||
						(String.IsNullOrEmpty(sSeverity)))
				{
					dPCIDeduct = 0.0;
				}
				else
				{
					Hashtable hashDistress = (Hashtable)m_hashMethodDistress[strMethod];
					nDistress = int.Parse(hashDistress[strDistress].ToString());


					if (String.IsNullOrEmpty(strAmount))
						dAmount = 0.0;
					else
						dAmount = Convert.ToDouble(strAmount);

					if (String.IsNullOrEmpty(strArea))
						dSamsiz = 0.0;
					else
						dSamsiz = Convert.ToDouble(strArea);

					if (dSamsiz > 0.0)
						dExtent = (dAmount / dSamsiz) * 100.0;
					else
						dExtent = 0.0;

					// Get the distress information from the PCI object and
					// display it in the detail grid.
					if (strMethod != "ac.clk" && strMethod != "bit.clk")
					{

						dPCIDeduct = PciDistress.pvt_ComputePCIDeduct(nDistress, sSeverity, dAmount, dSamsiz);
					}
					else
					{
						dPCIDeduct = PciDistress.pvt_ComputeNonPCIDeduct(strMethod, nDistress, sSeverity, dExtent);
					}

				}

			}
			catch (Exception exc)
			{
				Debug.WriteLine("Exception in CalculateCurrentDeducts: " + exc.Message);
				dPCIDeduct = 0.0;
			}
			return dPCIDeduct;
		}


		/// <summary>
		/// Calculates the current PCI for a section by iterating through
		/// the distresses for that section and submitting the totals
		/// to the PCI object.
		/// </summary>
		/// <returns>PCI for the current section</returns>
		private double CalculatePCI()
		{
			double dPCI = 100.0;
			string strMethod = "";
			string strID = "";
			string strDeductValue = "";

			StringBuilder sbDeduct = new StringBuilder();
			DataGridViewRow currentRow = dgvwPCI.CurrentRow;

			if (currentRow.Tag == null)
			{
				return dPCI;
			}

			strID = currentRow.Tag.ToString();
            double dSumDeduct = 0;
			foreach (DataGridViewRow row in dgvPCIDetail.Rows)
			{

				CalculateCurrentRowDeducts(row);
				if (row.Cells["DEDUCTVALUE"].Value != null)
				{
					strDeductValue = row.Cells["DEDUCTVALUE"].Value.ToString();
					if (!String.IsNullOrEmpty(strDeductValue) )
					{
						dSumDeduct += double.Parse(strDeductValue);
						sbDeduct.Append(row.Cells["DEDUCTVALUE"].Value.ToString());
						sbDeduct.Append(",");
					}
				}
			}
			if (sbDeduct.Length > 1)
			{
				sbDeduct.Remove(sbDeduct.Length - 1, 1);
				strMethod = currentRow.Cells["METHOD_"].Value.ToString();
                if (!String.IsNullOrEmpty(strMethod))
                {
                    dPCI = PciDistress.ComputePCIValue(sbDeduct.ToString(), strMethod.Trim());
                }

			}

			return dPCI;

		}

		private String FormatPCIAsString(double dPCI)
		{
			String strPCI = "";

			if (dPCI < 0.01)
			{
				strPCI = "";
			}
			else
			{
				strPCI = dPCI.ToString("f0");
			}
			return strPCI;

		}
		#endregion

		#region page events
		/// <summary>
		/// Use the advanced search to find specific sections.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSearch_Click(object sender, EventArgs e)
		{
			bool bLinear = rbLinearRef.Checked;
			String strQuery = tbSearch.Text;
			// TODO:  We need another search box here or do something
			// to prevent the exception failures in the current dialog box.
			//FormQueryRaw form = new FormQueryRaw(strPCI, bLinear, strQuery);
			//if (form.ShowDialog() == DialogResult.OK)
			//{
			//    tbSearch.Text = form.m_strQuery;

			//}
			//this.RefreshScreen();
			//DeleteThisFunction();
			return;
		}

		private void cbRoutes_SelectedIndexChanged(object sender, EventArgs e)
		{
            if(m_bUpdate)
			    RefreshScreen();
			return;

		}

		private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
		{
            if(m_bUpdate)
			    RefreshScreen();
			return;

		}

		#endregion

		#region PCI DataGridView events

		private void dgvwPCI_SelectionChanged(object sender, EventArgs e)
		{
			PCISelectionChanged(dgvwPCI.CurrentRow);
		}

		private void PCISelectionChanged(DataGridViewRow currentRow)
		{
			//double dPCI = 0.0;
			//string strPCI = "";
			string strMethod = "";

			if (currentRow != null && currentRow.Tag != null)
			{
				try
				{
					// now set up the detail grid to have the right dropdown lists
					if (currentRow.Cells["METHOD_"].Value != null)
					{
						strMethod = currentRow.Cells["METHOD_"].Value.ToString();
						DataGridViewComboBoxCell dgvCombo = (DataGridViewComboBoxCell)m_hashMethodCombo[strMethod];
						dgvPCIDetail.Columns["DISTRESS"].CellTemplate = dgvCombo;
					}
					this.RefreshDetailsScreen();
					//dPCI = CalculatePCI();
					//strPCI = FormatPCIAsString(dPCI);
					//dgvwPCI.Rows[dgvwPCI.CurrentRow.Index].Cells["PCI"].Value = strPCI;
					//UpdatePCIRecord(dgvwPCI.CurrentRow.Index, dgvwPCI.CurrentCell.ColumnIndex);



					// note:  We currently aren't getting this data since we are not validating
					// the input data at this time.
					// Get the list of sections so we can verify proper entry
					//GetCurrentSectionsDataSet();

				}
				catch (Exception exc)
				{
					string msg = String.Format("Update failed to PCI table: {0}", exc.Message);
					Debug.WriteLine(msg);
				}
			}

			return;

		}


		private void dgvwPCI_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			string msg = String.Format("Data error: {0}", e.Exception.Message);
			//MessageBox.Show(msg, "Data Error");
			return;
		}

		private void dgvwPCI_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
            Global.WriteOutput("Begin update of row.");
			double dPCI = 0.0;
			string strPCI = "";
			int currentColumnIndex = e.ColumnIndex;
			int currentRowIndex = e.RowIndex;
			StringBuilder sbInsert = new StringBuilder();
			DataGridViewRow currentRow = dgvwPCI.CurrentRow;

			try
			{

				//  Enter the information into the database.
				if (dgvwPCI.Rows[currentRowIndex].Tag == null)
				{
					// Get the key value from the current row.  If 
					// it does exist, use it to insert.

					InsertNewPCIRecord(currentRowIndex, currentColumnIndex);
				}
				else
				{
                    // If editing Method... wipe out dteals
                    if (e.ColumnIndex == 9)
                    {
                        if (dgvwPCI[e.ColumnIndex, e.RowIndex] != null)
                        {
                            if (m_strPreviousMethod != dgvwPCI[e.ColumnIndex, e.RowIndex].Value.ToString())
                            {
                                String strDelete = "DELETE FROM PCI_DETAIL WHERE PCI_ID=" + dgvwPCI.Rows[currentRowIndex].Tag;
                                DBMgr.ExecuteNonQuery(strDelete);
                            }
                        }
                    }
					// recalculate the PCI

					//dPCI = CalculatePCI();
					//strPCI = FormatPCIAsString(dPCI);
					//dgvwPCI.Rows[currentRowIndex].Cells["PCI"].Value = strPCI;
                    if(rbLinearRef.Checked)
                    {
                        CheckLinearRecord();
                    }
                    else
                    {
                        CheckSectionRecord();
                    }
					// do the database update
                    if(e.ColumnIndex == 11)           
                    {
                        Global.WriteOutput("Begin calculate PCI.");
						Global.WriteOutput("Check2");
					    dPCI = CalculatePCI();
					    strPCI = FormatPCIAsString(dPCI);
					    dgvwPCI.Rows[currentRowIndex].Cells["PCI"].Value = strPCI;
                        foreach (DataGridViewRow row in dgvPCIDetail.Rows)
                        {
                            UpdateNewPCIDetailRecord(row.Index, 3);
                            UpdateNewPCIDetailRecord(row.Index, 4);

                        }
                    }
					UpdatePCIRecord(currentRowIndex, currentColumnIndex);
				}


			}
			catch (Exception exc)
			{
				Debug.WriteLine("dgvwPCI Cell validated error: " + exc.Message);
			}
			return;
		}
		private void dgvwPCI_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			DataGridViewRow deletingRow = e.Row;
			string strID = "";
			string sqlCommand = "";
			int rowsAffected = 0;

			if (deletingRow == null)
				return;
			try
			{
				strID = deletingRow.Tag.ToString();

				// first, delete the matching rows from the PCI Detail
				// table
				sqlCommand = "DELETE FROM PCI_DETAIL WHERE PCI_ID = " + strID;
				rowsAffected = DBMgr.ExecuteNonQuery(sqlCommand);

				// Now delete the main row in the pci table.
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							sqlCommand = "DELETE FROM PCI WHERE ID_ = " + strID;
							break;
						case "ORACLE":
							sqlCommand = "DELETE FROM PCI WHERE \"ID_\" = " + strID;
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for dgvwPCI_UserDeletingRow()" );
							//break;
					}
				sqlCommand = "DELETE FROM PCI WHERE ID_ = " + strID;
				rowsAffected = DBMgr.ExecuteNonQuery(sqlCommand);
			}
			catch (Exception exc)
			{

				Debug.WriteLine("Error deleting from PCI: " + exc.Message);
				MessageBox.Show("Row not removed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			return;
		}

		private void dgvwPCI_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			// We currently aren't testing for validation of the input data.  This is a start
			// to some rudimentary validation.

			//int currentColumnIndex = e.ColumnIndex;
			//int currentRowIndex = e.RowIndex;
			//DataGridViewRow currentRow = dgvwPCI.CurrentRow;
			//bool isValidRoute = false;
			//string strBeginStation = "";
			//double dBeginStation = 0.0;

			//// Check if the route is entered and it is valid.
			//// Validate the from and to stations before entering
			//// the data into the database.
			//if (dgvwPCI.CurrentRow.Cells["ROUTES"].Value != null)
			//{
			//    if (currentColumnIndex == 1)
			//    {
			//        isValidRoute = GetCurrentSectionsDataSet();
			//        if (!isValidRoute)
			//            return;
			//    }

			//    if (dgvwPCI.CurrentRow.Cells["BEGIN_STATION"].Value != null)
			//    {
			//        strBeginStation = dgvwPCI.CurrentRow.Cells["BEGIN_STATION"].Value.ToString();
			//        if (!String.IsNullOrEmpty(strBeginStation))
			//        {
			//            dBeginStation = Double.Parse(strBeginStation);
			//        }
			//    }

			//    if (dgvwPCI.CurrentRow.Cells["END_STATION"].Value != null)
			//    {
			//    }

			//    if (dgvwPCI.CurrentRow.Cells["DIRECTION"].Value != null)
			//    {
			//    }
			//}
			return;
		}


		#endregion

		//private void DeleteThisFunction()
		//{
		//    String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		//    String strOutFile = strMyDocumentsFolder + "\\blockCrackFix.txt";


		//    List<BlockCrack> blockCracks = new List<BlockCrack>();
		//    StreamReader reader = new StreamReader(strOutFile);
		//    while(!reader.EndOfStream)
		//    {
		//        string line = reader.ReadLine();
		//        blockCracks.Add(new BlockCrack(line));
		//    }
		//    reader.Close();

		//    foreach(DataGridViewRow blockCrackRow in dgvwPCI.Rows)
		//    {
		//        //string id = blockCrackRow.Cells["ID_"].Value.ToString();

		//        if (blockCrackRow.Cells["FACILITY"].Value != null)
		//        {
		//            string facility = blockCrackRow.Cells["FACILITY"].Value.ToString();
		//            string section = blockCrackRow.Cells["SECTION"].Value.ToString();
		//            string sample = blockCrackRow.Cells["SAMPLE_"].Value.ToString();
		//            string date = DateTime.Parse(blockCrackRow.Cells["DATE_"].Value.ToString()).Year.ToString();
		//            string area = blockCrackRow.Cells["AREA"].Value.ToString();

		//            if (date == "2009")
		//            {
		//                dgvwPCI.FirstDisplayedScrollingRowIndex = blockCrackRow.Index;
		//                dgvwPCI.Refresh();
		//                dgvwPCI.CurrentCell = blockCrackRow.Cells[6];
		//                blockCrackRow.Selected = true;

		//                dgvPCIDetail.Invalidate();
		//                dgvPCIDetail.Refresh();
		//                dgvwPCI.Invalidate();
		//                dgvwPCI.Refresh();

		//                bool bFoundTwiceL = false;
		//                bool bFoundTwiceM = false;
		//                bool bFoundTwiceH = false;
		//                foreach (DataGridViewRow blockCrackDetail in dgvPCIDetail.Rows)
		//                {
		//                    if (blockCrackDetail.Cells[2].Value != null)
		//                    {
		//                        if (blockCrackDetail.Cells[2].Value.ToString() == "Alligator Cracking")
		//                        {
		//                            string amount = blockCrackDetail.Cells["AMOUNT"].Value.ToString();
		//                            string severity = blockCrackDetail.Cells["SEVERITY"].Value.ToString();

		//                            //System.Diagnostics.Debug.WriteLine("FACILITY: " + facility);
		//                            //System.Diagnostics.Debug.WriteLine("SECTION: " + section);
		//                            //System.Diagnostics.Debug.WriteLine("SAMPLE: " + sample);
		//                            //System.Diagnostics.Debug.WriteLine("AREA: " + area);
		//                            //System.Diagnostics.Debug.WriteLine("AMOUNT: " + amount);
		//                            //System.Diagnostics.Debug.WriteLine("SEVERITY: " + severity);
		//                            //System.Diagnostics.Debug.WriteLine("DISTRESS: Alligator Cracking");
		//                            //System.Diagnostics.Debug.WriteLine("-----------------------------------------------------------------");

		//                            List<BlockCrack> foundBlockCrack = blockCracks.FindAll(
		//                                delegate(BlockCrack bc)
		//                                {
		//                                    return (bc.Facility == facility && bc.Section == section && bc.Severity == severity && bc.Quantity == amount && bc.SampleNumber == sample && bc.SampleSize == area);
		//                                });
		//                            if (foundBlockCrack.Count > 1)
		//                            {
		//                                throw new Exception();
		//                            }
		//                            else if (foundBlockCrack.Count == 1)
		//                            {
		//                                //if (MessageBox.Show("Found Gator Cracking", "Test", MessageBoxButtons.YesNo) == DialogResult.Yes)
		//                                //{
		//                                if (severity == "M" && !bFoundTwiceM) Fixit(blockCrackDetail);
		//                                if (severity == "H" && !bFoundTwiceH) Fixit(blockCrackDetail);
		//                                if (severity == "L" && !bFoundTwiceL) Fixit(blockCrackDetail);

		//                                //}
		//                                if (amount == "H") bFoundTwiceH = true;
		//                                if (amount == "M") bFoundTwiceM = true;
		//                                if (amount == "L") bFoundTwiceL = true;
		//                            }

		//                        }
		//                    }

		//                }
		//            }
		//        }
		//        else
		//        {
		//            MessageBox.Show("Complete");
		//        }
		//    }
		//}

		//private void Fixit(DataGridViewRow row)
		//{
		//    row.Cells["DISTRESS"].Value = "Block Cracking";

		//    dgvPCIDetail.FirstDisplayedScrollingRowIndex = row.Index;
		//    dgvPCIDetail.Refresh();
		//    dgvPCIDetail.CurrentCell = row.Cells[2];
		//    row.Selected = true;


		//    DetailEndEdit(2, row.Index);
		//}
		

		//public class BlockCrack
		//{
		//    string facility;
		//    string section;
		//    string samplenumber, samplesize, severity, quantity;


		//    public BlockCrack(string line)
		//    {
		//        string [] blockCrackLine = line.Split('\t');
		//        facility = blockCrackLine[0];
		//        section = blockCrackLine[1];
		//        samplenumber = blockCrackLine[2];
		//        samplesize = blockCrackLine[3];
		//        severity = blockCrackLine[4];
		//        quantity = blockCrackLine[5];
		//    }


		//    public string Facility
		//    {
		//        get { return facility; }
		//    }

		//    public string Section
		//    {
		//        get { return section; }
		//    }

		//    public string SampleNumber
		//    {
		//        get { return samplenumber; }
		//    }

		//    public string SampleSize
		//    {
		//        get { return samplesize; }
		//    }

		//    public string Severity
		//    {
		//        get { return severity; }
		//    }

		//    public string Quantity
		//    {
		//        get { return quantity; }
		//    }
		//}
		

		#region PCIDetail Data Grid events

		private void dgvPCIDetail_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			DetailEndEdit(e.ColumnIndex, e.RowIndex);

		}

		private void DetailEndEdit(int nCol, int nRow)
		{

			int currentColumnIndex = nCol;
			int currentRowIndex = nRow;
			try
			{

				// Calculate the deducts and then update the values
				// in the database.
				CalculateCurrentRowDeducts(dgvPCIDetail.CurrentRow);
				if (dgvPCIDetail.Rows[currentRowIndex].Tag == null)
				{
					InsertNewPCIDetailRecord(currentRowIndex, currentColumnIndex);
				}
				else
				{
					UpdateNewPCIDetailRecord(currentRowIndex, currentColumnIndex);
				}

				// now calculate the pci using the deducts from each row
				// of the details GridView and place it in the pci grid
				double dPCI = CalculatePCI();
				string strPCI = FormatPCIAsString(dPCI);
				dgvwPCI.CurrentRow.Cells["PCI"].Value = strPCI;
				UpdatePCIRecord(dgvwPCI.CurrentRow.Index, dgvwPCI.CurrentRow.Cells["PCI"].ColumnIndex);
				dgvwPCI.Invalidate();
                dgvwPCI.Refresh();


			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}

			return;

		}

        private void CheckLinearRecord()
        {
            for(int i = 0; i < 5; i++)
            {
                dgvwPCI.CurrentRow.Cells[i].Style.ForeColor = Color.Black;
            }


            if (dgvwPCI.CurrentRow.Cells["ROUTES"].Value == null ||
                dgvwPCI.CurrentRow.Cells["DIRECTION"].Value == null ||
                dgvwPCI.CurrentRow.Cells["BEGIN_STATION"].Value == null ||
                dgvwPCI.CurrentRow.Cells["END_STATION"].Value == null) return;

            String sRoutes = dgvwPCI.CurrentRow.Cells["ROUTES"].Value.ToString() ;
            String sBegin = dgvwPCI.CurrentRow.Cells["BEGIN_STATION"].Value.ToString();
            String sEnd = dgvwPCI.CurrentRow.Cells["END_STATION"].Value.ToString();
            String sDirection = dgvwPCI.CurrentRow.Cells["DIRECTION"].Value.ToString();



            List<String> listDirections = new List<String>();
            try
            {
                String strSelect = "SELECT DIRECTION,BEGIN_STATION,END_STATION FROM NETWORK_DEFINITION WHERE ROUTES='" + sRoutes + "' AND DIRECTION='" + sDirection + "'";
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Global.WriteOutput("Error: Routes and Direction must be defined in Network Defintion before PCI record may be entered.  Route:" + sRoutes + " Direction:" + sDirection + " is not defined.");
                    dgvwPCI.CurrentRow.Cells["DIRECTION"].Style.ForeColor = Color.Red;
                }
                else
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    double dBegin = double.Parse(dr["BEGIN_STATION"].ToString());
                    double dEnd = double.Parse(dr["END_STATION"].ToString());

                    if(double.Parse(sBegin) < dBegin || double.Parse(sBegin) > dEnd)
                    {
                        Global.WriteOutput("Error: Begin Station must be greater than or equal to the Begin Station in Network Defintion and less than or equal to the End Station before PCI record may be entered.");
                        dgvwPCI.CurrentRow.Cells["BEGIN_STATION"].Style.ForeColor = Color.Red;
                    }

                    if(double.Parse(sEnd) < dBegin || double.Parse(sEnd) > dEnd)
                    {
                        Global.WriteOutput("Error: End Station must be greater than or equal to the Begin Station in Network Defintion and less than or equal to the End Station before PCI record may be entered.");
                        dgvwPCI.CurrentRow.Cells["END_STATION"].Style.ForeColor = Color.Red;
                    }
                    if(double.Parse(sEnd) < double.Parse(sBegin))
                    {
                        Global.WriteOutput("Error: End Station must be greater than or equal to the Begin Station.");
                        dgvwPCI.CurrentRow.Cells["END_STATION"].Style.ForeColor = Color.Red;
                        dgvwPCI.CurrentRow.Cells["BEGIN_STATION"].Style.ForeColor = Color.Red;
                    }
                }
            }
            catch(Exception exc)
            {
                Global.WriteOutput("Error: During check PCI record." + exc.Message);
            }
        }

        private void CheckSectionRecord()
        {
            for (int i = 0; i < 7; i++)
            {
                dgvwPCI.CurrentRow.Cells[i].Style.ForeColor = Color.Black;
            }


            if (dgvwPCI.CurrentRow.Cells["FACILITY"].Value == null ||
                dgvwPCI.CurrentRow.Cells["SECTION"].Value == null) return;

            String sFacility = dgvwPCI.CurrentRow.Cells["FACILITY"].Value.ToString();
            String sSection = dgvwPCI.CurrentRow.Cells["SECTION"].Value.ToString();

            List<String> listDirections = new List<String>();
            try
            {
                String strSelect = "SELECT FACILITY,SECTION FROM NETWORK_DEFINITION WHERE FACILITY='" + sFacility + "' AND SECTION='" + sSection + "'";
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    Global.WriteOutput("Error: Facility and Section must be defined in Network Defintion before PCI record may be entered.  Facilty:" + sFacility + " Section:" + sSection + " is not defined.");
                    dgvwPCI.CurrentRow.Cells["FACILTY"].Style.ForeColor = Color.Red;
                    dgvwPCI.CurrentRow.Cells["SECTION"].Style.ForeColor = Color.Red;
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: During check PCI record." + exc.Message);
            }
        }

        
        
        
        private void dgvPCIDetail_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			Debug.WriteLine("PCIDetail Data Error: " + e.Exception.Message);
		}


		private void dgvPCIDetail_SelectionChanged(object sender, EventArgs e)
		{
			DataGridViewRow currentRow = null;
			string strMethod = "";
			try
			{
				currentRow = dgvwPCI.CurrentRow;

				if (currentRow.Cells["METHOD_"].Value == null)
				{
					return;
				}
				else
				{
					strMethod = currentRow.Cells["METHOD_"].Value.ToString();
                    DataGridViewComboBoxCell dgvCombo = (DataGridViewComboBoxCell)m_hashMethodCombo[strMethod];
                    dgvPCIDetail.Columns["DISTRESS"].CellTemplate = dgvCombo;
				}
			}
			catch (Exception exc)
			{
				Debug.WriteLine(exc.Message);
			}

		}

		private void dgvPCIDetail_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
		{
			DataGridViewRow deletingRow = e.Row;
			string strID = "";
			int rowsAffected = 0;

			if (deletingRow == null)
				return;
			try
			{
				strID = deletingRow.Tag.ToString();

				string sqlCommand = "DELETE FROM PCI_DETAIL WHERE ID_ = " + strID;
				rowsAffected = DBMgr.ExecuteNonQuery(sqlCommand);
			}
			catch (Exception exc)
			{
				Debug.WriteLine("Error deleting from PCI_DETAIL: " + exc.Message);
				MessageBox.Show("Row not removed", "ERROR:" + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
			}
			return;

		}

		#endregion

        private void toolStripButtonBulkLoad_Click(object sender, EventArgs e)
        {
            bool bIsLinear = true;
            if (rbSectionRef.Checked) bIsLinear = false;
            FormPCIBulkLoad form = new FormPCIBulkLoad(bIsLinear);
            form.Show(FormManager.GetDockPanel());
        }

        private void dgvPCIDetail_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            string strPCI = FormatPCIAsString(CalculatePCI());
            dgvwPCI.CurrentRow.Cells["PCI"].Value = strPCI;
            UpdatePCIRecord(dgvwPCI.CurrentRow.Index, dgvwPCI.CurrentRow.Cells["PCI"].ColumnIndex);
            dgvwPCI.Invalidate();
            dgvwPCI.Refresh();
        }

        private void dgvwPCI_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // If editing Method... wipe out details if change
            if (e.ColumnIndex == 9)
            {
                if (dgvwPCI[e.ColumnIndex, e.RowIndex] != null)
                {
                    if (dgvwPCI[e.ColumnIndex, e.RowIndex].Value != null)
                    {
                        m_strPreviousMethod = dgvwPCI[e.ColumnIndex, e.RowIndex].Value.ToString();
                    }
                    else
                    {
                        m_strPreviousMethod = "";
                    }
                }
            }
        }

		private void toolStripButtonCalculatePCI_Click(object sender, EventArgs e)
		{
			this.RecalculateAllPCI();
		}







	}
}
