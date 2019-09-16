using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using DatabaseManager;
using WeifenLuo.WinFormsUI.Docking;
using RoadCare3.Properties;

namespace RoadCare3
{
    public partial class FormConstructionHistory : BaseForm
    {
        int m_nCount = 0; //Number of records in Construction History Table.
        BindingSource binding;
        DataAdapter dataAdapter;
        DataTable table;
        bool m_bInitialUpdate = false; //To keep year and route list box update happening prematurely.
        bool m_bReferenceUpdate = false;

        public TreeNode associatedNode;//so far only used for setting the lightbulb on the selected node

        public FormConstructionHistory()
        {
            InitializeComponent();
        }

		protected void SecureForm()
		{
			//I'm going to leave this unimplemented for the time being until the form is closer to complete and we have a better
			//idea of exactly what controls it will contain
			//throw new NotImplementedException();
		}

        private void FormConstructionHistory_Load(object sender, EventArgs e)
        {
			SecureForm();
			FormLoad(Settings.Default.CONSTRUCTION_HISTORY_IMAGE_KEY, Settings.Default.CONSTRUCTION_HISTORY_IMAGE_KEY_SELECTED);

            this.TabText = "Construction";
            
            String strSelect;
            m_nCount = 0;
            //Get construction history count in case large return.
            strSelect = "SELECT COUNT(*) FROM CONSTRUCTION_HISTORY";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(), out m_nCount);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Getting count of Construction History failed.  SQL error is " + exception.Message);

            }
            if (Global.IsLinear == true)
            {
                rbLinearRef.Checked = true;
            }
            else
            {
                rbSectionRef.Checked = true;
            }

            FillYearsList();
            FillRouteFacilityList();
            m_bInitialUpdate = true;
            m_bReferenceUpdate = true;
            FillDataGridView();

        }

        private void FillYearsList()
        {
            cbYear.Items.Clear();
            String strSelect;
			if( rbLinearRef.Checked )
			{
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						strSelect = "SELECT DISTINCT year(DATE_) AS YEARS FROM CONSTRUCTION_HISTORY WHERE ROUTES <>'' ORDER BY YEARS DESC";
						break;
					case "ORACLE":
						strSelect = "SELECT DISTINCT TO_CHAR(DATE_,'YYYY') AS YEARS FROM CONSTRUCTION_HISTORY WHERE ROUTES LIKE '_%' ORDER BY YEARS DESC";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for FillYearsList()" );
						//break;
				}
			}
			else
			{
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						strSelect = "SELECT DISTINCT year(DATE_) AS YEARS FROM CONSTRUCTION_HISTORY WHERE FACILITY <>'' ORDER BY YEARS DESC";
						break;
					case "ORACLE":
						strSelect = "SELECT DISTINCT TO_CHAR( DATE_, 'YYYY' ) AS YEARS FROM CONSTRUCTION_HISTORY WHERE FACILITY LIKE '_%' ORDER BY YEARS DESC";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for FillYearsList()" );
						//break;
				}
			}
 
                
            cbYear.Items.Add("All");
            try
            {

                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    cbYear.Items.Add(row["YEARS"].ToString());
                }

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Filling year selection filter.  Contact database administrator.  SQL message is - " + exception.Message);
            }
			//catch( Exception ex )
			//{
			//    Global.WriteOutput("Error: Filling year selection filter.  " + ex.Message);
			//}

            if (m_nCount < Global.MaximumReturn)
            {
                cbYear.Text = "All";
            }
            else
            {
                if (cbYear.Items.Count > 1)
                {
                    cbYear.Text = cbYear.Items[1].ToString();
                }
                else
                {
                    cbYear.Text = "All";
                }
            }
        }

        private void FillRouteFacilityList()
        {
            cbRoutes.Items.Clear();
            String strSelect;
			if( rbLinearRef.Checked )
			{
				lblRouteFacility.Text = "Route";
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						strSelect = "SELECT DISTINCT ROUTES FROM CONSTRUCTION_HISTORY WHERE ROUTES <>'' ORDER BY ROUTES ASC";
						break;
					case "ORACLE":
						strSelect = "SELECT DISTINCT ROUTES FROM CONSTRUCTION_HISTORY WHERE ROUTES LIKE '_%' ORDER BY ROUTES ASC";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//break;
				}
			}
			else
			{
				lblRouteFacility.Text = "Facility";
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							strSelect = "SELECT DISTINCT FACILITY FROM CONSTRUCTION_HISTORY WHERE FACILITY <>'' ORDER BY FACILITY ASC";
							break;
						case "ORACLE":
							strSelect = "SELECT DISTINCT FACILITY FROM CONSTRUCTION_HISTORY WHERE FACILITY LIKE '_%' ORDER BY FACILITY ASC";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
						//break;
					}
			}


            cbRoutes.Items.Add("All");
            try
            {

                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    cbRoutes.Items.Add(row[0].ToString());
                }

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Filling route/facility selection filter.  Contact database administrator.  SQL message is - " + exception.Message);
            }
			//catch
			//{
			//    Global.WriteOutput("Error: Filling route/facility selection filter.");
			//}

            if (m_nCount < Global.MaximumReturn)
            {
                cbRoutes.Text = "All";
            }
            else
            {
                if (cbRoutes.Items.Count > 1)
                {
                    cbRoutes.Text = cbRoutes.Items[1].ToString();
                }
                else
                {
                    cbRoutes.Text = "All";
                }
            }
        }

        private void rbLinearRef_CheckedChanged(object sender, EventArgs e)
        {
            if (m_bInitialUpdate == true)
            {
                m_bReferenceUpdate = false;

                FillYearsList();
                FillRouteFacilityList();

                m_bReferenceUpdate = true;
                FillDataGridView();
            }
        }

        private void FillDataGridView()
        {
            String strSelect;
            String strWhere;
            binding = new BindingSource();
            #region lrs
            if (rbLinearRef.Checked)//Linear
            {
                if (cbRoutes.Text == "All")
                {
						switch( DBMgr.NativeConnectionParameters.Provider )
						{
							case "MSSQL":
								strWhere = " WHERE ROUTES <>'' ";
								break;
							case "ORACLE":
								strWhere = " WHERE ROUTES LIKE '_%' ";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
								//break;
						}
				}
                else
                {
                    strWhere = " WHERE ROUTES ='" + cbRoutes.Text.ToString() + "'";
                }

                if (cbYear.Text != "All")
                {
						switch( DBMgr.NativeConnectionParameters.Provider )
						{
							case "MSSQL":
								strWhere += " AND year(DATE_)='" + cbYear.Text.ToString() + "'";
								break;
							case "ORACLE":
								strWhere += " AND TO_CHAR(DATE_,'YYYY')='" + cbYear.Text.ToString() + "'";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for FillDataGridView()" );
								//break;
						}
				}

                try
                {
					SqlConnection conn = DBMgr.NativeConnectionParameters.SqlConnection;
					strSelect = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,LANES,DATE_,ACTIVITY,COMMENT_,THICKNESS,ACTIVITY_TYPE FROM CONSTRUCTION_HISTORY " + strWhere;
					dataAdapter = new DataAdapter( strSelect );

                    // Populate a new data table and bind it to the BindingSource.
                    table = new DataTable();
                    table.Locale = System.Globalization.CultureInfo.InvariantCulture;

                    dataAdapter.Fill(table);
                    binding.DataSource = table;
                    dgvConstructionHistory.DataSource = binding;
                    bindingNavigatorConstructionHistory.BindingSource = binding;
                    dgvConstructionHistory.Columns["ID_"].Visible = false;
                    dgvConstructionHistory.Columns["ID_"].Width = 0;
                    dgvConstructionHistory.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Connecting contruction history table. SQL message is " + exception.Message);
                }
            }
            #endregion
            #region srs
            else
            {
                if (cbRoutes.Text == "All")
                {
						switch( DBMgr.NativeConnectionParameters.Provider )
						{
							case "MSSQL":
								strWhere = " WHERE FACILITY <>'' ";
								break;
							case "ORACLE":
								strWhere = " WHERE FACILITY LIKE '_%' ";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
								//break;
						}
				}
                else
                {
                    strWhere = " WHERE FACILITY ='" + cbRoutes.Text.ToString() + "'";
                }

                if (cbYear.Text != "All")
                {
						switch( DBMgr.NativeConnectionParameters.Provider )
						{
							case "MSSQL":
								strWhere += " AND year(DATE_)='" + cbYear.Text.ToString() + "'";
								break;
							case "ORACLE":
								strWhere += " AND TO_CHAR(DATE_,'YYYY')='" + cbYear.Text.ToString() + "'";
								break;
							default:
								throw new NotImplementedException( "TODO: Create ANSI implementation for FillDataGridView" );
								//break;
						}
				}

                try
                {
					strSelect = "SELECT ID_,FACILITY,SECTION,LANES,DATE_,ACTIVITY,COMMENT_,THICKNESS,ACTIVITY_TYPE FROM CONSTRUCTION_HISTORY " + strWhere;
					dataAdapter = new DataAdapter( strSelect );

                    // Populate a new data table and bind it to the BindingSource.
                    table = new DataTable();

                    table.Locale = System.Globalization.CultureInfo.InvariantCulture;


                    dataAdapter.Fill(table);
                    binding.DataSource = table;
                    dgvConstructionHistory.DataSource = binding;
                    dgvConstructionHistory.Columns["ID"].Visible = false;
                    dgvConstructionHistory.Columns["ID"].Width = 0;
                    dgvConstructionHistory.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Connecting contruction history table. SQL message is " + exception.Message);
                }


            }
            #endregion
        }

        private void cbRoutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_bInitialUpdate && m_bReferenceUpdate)
            {
                if (cbRoutes.SelectedItem != null)
                {
                    FillDataGridView();
                }
            }
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_bInitialUpdate && m_bReferenceUpdate)
            {
                if (cbYear.SelectedItem != null)
                {
                    FillDataGridView();
                }
            }
        }

        private void FormConstructionHistory_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();

            FormManager.RemoveFormConstructionHistory(this);
        }

     }
}