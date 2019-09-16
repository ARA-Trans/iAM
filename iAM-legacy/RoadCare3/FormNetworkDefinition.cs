using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using System.Data.SqlClient;
using WeifenLuo.WinFormsUI.Docking;
using System.Web.UI;
using System.IO;
using System.Collections;
using RoadCare3.Properties;
using SharpMap.Geometries;
using EGIS.ShapeFileLib;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
    //TODO. On verfify trap for names that are to long.  Update adapter doesn't like that.  Also check for commas, single quotes, double quotes.
    public partial class FormNetworkDefinition : BaseForm
    {
        BindingSource binding;
        DataAdapter dataAdapter;
        DataTable table;

        List<NetworkDefinitionData> m_listSFD;
        Hashtable m_htSFDs;
        private bool m_bLinear;
        SolutionExplorerTreeNode _netDefNode;

        /// <summary>
        /// If linear is true the linear reference data is displayed, if it is false, then the section data is displayed.
        /// </summary>
        /// <param name="bLinear"></param>
        public FormNetworkDefinition(bool bLinear, SolutionExplorerTreeNode currentNetDefNode)
        {
            InitializeComponent();
            m_bLinear = bLinear;
            _netDefNode = currentNetDefNode;
            //comboBoxUpdateNetworkDefinition.Width = 600;
        }

		protected void SecureForm()
		{
			checkBoxAllowEdit.Checked = false;
			LockCheckBox( checkBoxAllowEdit );
			LockDataGridView( dgvNetworkDefinition );
			LockButton( btnImportGeometries );
			LockButton( btnImportShapefile );

			//the binding navigator will add and delete items from the associated datagridview at the end of the item click events
			//no matter what processing occurs within the event.  This is a problem because the datagrid view gets updated when it
			//loses focus (and several other times, actually).
			//Theoretically, we might simply check the state of the "can edit" checkbox, but there may be security
			//gradiations finer than just a binary can edit/cannot edit.  For example, the user may be allowed to edit rows but not
			//add any new ones or to add new ones but not to delete existing ones, etc.  That checkbox, however, is now used
			//repeatedly throughout the form so for the sake of time, I'm going to leave it as is for now and attempt to santize
			//everything else.

			//To prevent this ;issue we have to disable the buttons entirely

			LockBindingNavigator( bindingNavigatorNetworkDefinition );

			//Additionally, we'll set up the checked event for the box to turn these buttons on and off
			//(depending on our security permissions)

			//Also have to get rid of possible context menu exploits.
			LockToolStripMenuItem( tsmiDelete );
			LockToolStripMenuItem( tsmiPaste );

			//cbUnits.Enabled = false;
			LockComboBox( cbUnits );

			if( Global.SecurityOperations.CanModifyNetworkDefinitionData() )
			{
				UnlockCheckBox( checkBoxAllowEdit );
			}

            // Add items to Remote Network Definition drop down
            List<string> remoteNetDefConnParams = DBOp.GetRemoteNetworkDefinitionConnectionParameterNames();
            foreach (string toAdd in remoteNetDefConnParams)
            {
                comboBoxUpdateNetworkDefinition.Items.Add(toAdd);
            }
		}

        private void FormNetworkDefinition_Load(object sender, EventArgs e)
        {
            SecureForm();
            
            if (this.ImageView)
            {

            }
            else
            {
                if (m_bLinear)
                {
                    FormLoad(Settings.Default.LINEAR_NETWORK_DEFINITION_IMAGE_KEY, Settings.Default.LINEAR_NETWORK_DEFINITION_IMAGE_KEY_SELECTED);
					tsbJoinMultilinestring.Visible = true;
                }
                else
                {
                    FormLoad(Settings.Default.SECTION_NETWORK_DEFINITION_IMAGE_KEY, Settings.Default.SECTION_NETWORK_DEFINITION_IMAGE_KEY_SELECTED);
					tsbJoinMultilinestring.Visible = false;
                }
            }
                        
            //From GLOBAL
            //Get SRS area UNITS
            //Get LRS area UNITS
            if (m_bLinear)
            {
                this.TabText = "Route";
            }
            else
            {
                this.TabText = "Section";
            }
            
            DataSet ds;
            ToolTip tipVerify = new ToolTip();
            tipVerify.InitialDelay = 1000;
            tipVerify.ReshowDelay = 500;

            // Force the ToolTip text to be displayed whether or not the form is active.
            tipVerify.ShowAlways = true;

			// For remote network definition selections, we need to populate the combobox with items.
            // First we need a selection to add new network definition connection params
            comboBoxUpdateNetworkDefinition.Items.Add("Add New...");
            string query = "SELECT CONNECTION_NAME FROM CONNECTION_PARAMETERS WHERE IDENTIFIER = 'NETWORK'";
            DataSet dsNumConnections = DBMgr.ExecuteQuery(query);
            foreach (DataRow dr in dsNumConnections.Tables[0].Rows)
            {
                comboBoxUpdateNetworkDefinition.Items.Add(dr["CONNECTION_NAME"].ToString());
            }

            String strQuery;
            if (m_bLinear)
            {
                this.Text = "Linear Route Definition";
                labelNetworkDefinition.Text = "Linear Route Definition";
            }
            else
            {
                comboFacility.Items.Add("All");

					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            strQuery = "SELECT DISTINCT FACILITY FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY <> '' ORDER BY FACILITY";
							break;
						case "ORACLE":
                            strQuery = "SELECT DISTINCT FACILITY FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY LIKE '_%' ORDER BY FACILITY";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
							//break;
					}
                ds = DBMgr.ExecuteQuery(strQuery);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboFacility.Items.Add(row.ItemArray[0].ToString());
                }
                comboFacility.Text = "All";
                this.Text = "Facility Section Definition";
                labelNetworkDefinition.Text = "Facility Section Definition";
                labelUnits.Text = "Section area units: feet squared";
                labelFacility.Visible = true;
                comboFacility.Visible = true;
            }
            CreateGridView();

            // Now select the appropriate LR units for this network, based on the OPTIONS table field NETWORK_DEFINITION_UNITS
            strQuery = "SELECT OPTION_VALUE FROM OPTIONS WHERE OPTION_NAME = 'NETWORK_DEFINITION_UNITS'";
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
                String strNDUnits = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                if (cbUnits.Items.Contains(strNDUnits))
                {
                    cbUnits.Text = strNDUnits;
                }
                else
                {
                    cbUnits.Text = "Miles";
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Failed to load network definition units. " + exc.Message);
                cbUnits.Text = "Miles";
            }
        }

        private void CreateGridView()
        {
            String strQuery;
            if (m_bLinear)
            {
				strQuery = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE ROUTES IS NOT NULL ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
			}
            else
            {

					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY <>'' ORDER BY FACILITY, SECTION";
							break;
						case "ORACLE":
                            strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY LIKE '_%' ORDER BY FACILITY, SECTION";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for CreateGridView()" );
					}
            }
            binding = new BindingSource();
            dataAdapter = new DataAdapter(strQuery);

            // Populate a new data table and bind it to the BindingSource.
            table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);
            table.Columns[0].ColumnMapping = MappingType.Hidden;

			//this doesn't seem to actually work when the column name is ID

			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					break;
				case "ORACLE":
					DataColumn[] primaryKeyColumns = new DataColumn[1];
					table.Columns["ID_"].AutoIncrement = true;
					primaryKeyColumns[0] = table.Columns["ID_"];
					table.PrimaryKey = primaryKeyColumns;
                    table.TableName = _netDefNode.NetworkDefinition.TableName;
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
			}
			
			binding.DataSource = table;
            dgvNetworkDefinition.DataSource = binding;
            bindingNavigatorNetworkDefinition.BindingSource = binding;
			if( checkBoxAllowEdit.Checked && Global.SecurityOperations.CanModifyNetworkDefinitionData() )
			{
				dgvNetworkDefinition.ReadOnly = false;
			}
			else
			{
				dgvNetworkDefinition.ReadOnly = true;
			}

            if (m_bLinear)
            {
                dgvNetworkDefinition.Columns["ROUTES"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvNetworkDefinition.Columns["BEGIN_STATION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
				dgvNetworkDefinition.Columns["BEGIN_STATION"].DefaultCellStyle.Format = "G";


				dgvNetworkDefinition.Columns["END_STATION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
				dgvNetworkDefinition.Columns["END_STATION"].DefaultCellStyle.Format = "G";
	
				dgvNetworkDefinition.Columns["DIRECTION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);

            }
            else
            {
                dgvNetworkDefinition.Columns["FACILITY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvNetworkDefinition.Columns["SECTION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
                dgvNetworkDefinition.Columns["AREA"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
            }

            dgvNetworkDefinition.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        private void comboFacility_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strText = comboFacility.Text.ToString();
            if (dataAdapter == null) return;
            if (checkBoxAllowEdit.Checked) dataAdapter.Update((DataTable)binding.DataSource);
            String strQuery;
            if (comboFacility.Text == "All")
            {
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY <>''";
							break;
						case "ORACLE":
                            strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY LIKE '_%'";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for comboFacility_SelectedIndexChanged()" );
							//break;
					}
            }
            else
            {
                strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY ='" + strText + "'";
            }

            SqlConnection conn = DBMgr.NativeConnectionParameters.SqlConnection;
            binding = new BindingSource();
            dataAdapter = new DataAdapter(strQuery);

            // Populate a new data table and bind it to the BindingSource.
            table = new DataTable();

            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            dataAdapter.Fill(table);

            binding.DataSource = table;
            dgvNetworkDefinition.DataSource = binding;
            if (checkBoxAllowEdit.Checked) dgvNetworkDefinition.ReadOnly = false;
            else dgvNetworkDefinition.ReadOnly = true;
            
            dgvNetworkDefinition.Columns["FACILITY"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvNetworkDefinition.Columns["SECTION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
            dgvNetworkDefinition.Columns["AREA"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
            dgvNetworkDefinition.Columns["ID_"].Visible = false;
            dgvNetworkDefinition.Columns["ID_"].Width = 0;
            dgvNetworkDefinition.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;


        }

        private void FormNetworkDefinition_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ImageView) 
                CloseImageView();
            else
                CloseRoadCare();
        }

        private void CloseImageView()
        {
            try
            {
                Global.WriteOutput("Updating data adapter.");
                if (checkBoxAllowEdit.Checked) dataAdapter.Update((DataTable)binding.DataSource);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not update the database table. " + exc.Message);
                return;
            }
        }


        private void CloseRoadCare()
        {
            FormUnload();

            try
            {
                if (checkBoxAllowEdit.Checked) dataAdapter.Update((DataTable)binding.DataSource);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not update the database table. " + exc.Message);
                return;
            }
            if (m_bLinear)
            {
                FormManager.RemoveFormNetworkDefinitionLinear(this);
            }
            else
            {
                FormManager.RemoveFormNetworkDefinitionSection(this);
            }
        }


        private void dgvNetworkDefinition_KeyUp(object sender, KeyEventArgs e)
        {   
            if (e.Control)
            {
				if( e.KeyCode == Keys.V )
				{
					if( Global.SecurityOperations.CanCreateNetworkDefinitionData() )
					{
						ImportRows();
					}
					else
					{
						Global.WriteOutput( "Error: Insufficient permission to add new network definition rows." );
					}
				}
            }
			
        }

        private void ImportRows()
        {
            // Create the list of possible errors to print to the output window.
            this.Cursor = Cursors.WaitCursor;
            List<String> listErrors = new List<String>();

            if (!checkBoxAllowEdit.Checked)
            {
                Global.WriteOutput("Error: To paste data Allow Edit check box must be checked.");
                return;
            }
            float f;
            dataAdapter.Update((DataTable)binding.DataSource);
            dataAdapter.Dispose();

			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\NetworkDefinition.txt";
            TextWriter tw = new StreamWriter(strOutFile);
            

            string s = Clipboard.GetText();
            s = s.Replace("\r\n", "\n");
            string[] lines = s.Split('\n');

            String strCell0,strCell1, strCell2, strCell3;
            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] cells = line.Split('\t');
                    //INSERT INTO table_name (column1, column2,...)VALUES (value1, value2,....)
                    if (m_bLinear)
                    {
						if (cells.Length == 4)
						{
							strCell0 = cells[0].ToString().Trim();
							strCell1 = cells[1].ToString().Trim();
							strCell2 = cells[2].ToString().Trim();
							strCell3 = cells[3].ToString().Trim();

							if (strCell0 == "")
							{
								listErrors.Add(line + "\t ROUTE can not be blank.");
								continue;
							}
							try
							{
								f = float.Parse(cells[1].ToString());
							}
							catch
							{
								listErrors.Add(line + "\t BEGIN_STATION is not a number.");
								continue; //if not number
							}

							try
							{
								f = float.Parse(cells[2].ToString());
							}
							catch
							{
								listErrors.Add(line + "\t END_STATION is not a number.");
								continue; //if not number
							}

							if (strCell3 == "")
							{
								listErrors.Add(line + "\t DIRECTION must not be blank.");
								continue;
							}

								switch (DBMgr.NativeConnectionParameters.Provider)
								{
									case "MSSQL":
										tw.WriteLine("\t" + strCell0 + "\t" + strCell1 + "\t" + strCell2 + "\t" + strCell3 + "\t\t\t\t\t\t\t\t");
										break;
									case "ORACLE":
										tw.WriteLine(strCell0 + "\t" + strCell1 + "\t" + strCell2 + "\t" + strCell3 + "\t\t\t\t\t\t\t\t");
										break;
									default:
										throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
										//break;
								}
							if (!comboFacility.Items.Contains(cells[0].ToString()))
							{
								comboFacility.Items.Add(cells[0].ToString());
							}
						}
                    }
                    else
                    {
						try
						{
							if (cells.Length == 3)
							{
								strCell0 = cells[0].ToString().Trim();
								strCell1 = cells[1].ToString().Trim();
								strCell2 = cells[2].ToString().Trim();

								if (strCell0 == "")
								{
									listErrors.Add(line + "\t FACILITY must not be blank.");
									continue;//no blank facility
								}
								if (strCell1 == "")
								{
									listErrors.Add(line + "\t SECTION must not be blank.");
									continue;//no blank section
								}
								if (strCell2 == "")
								{
									listErrors.Add(line + "\t AREA must not be blank.");
									continue;// no blank area.
								}

								try
								{
									f = float.Parse(cells[2].ToString());
								}
								catch
								{
									listErrors.Add(line + "\t Area is not a number.");
									continue; //if not number

								}
								tw.WriteLine("\t\t\t\t\t" + strCell0 + "\t" + strCell1 + "\t" + strCell2 + "\t\t\t\t\t");
								if (!comboFacility.Items.Contains(cells[0].ToString()))
								{
									comboFacility.Items.Add(cells[0].ToString());
								}
							}
						}
						catch (Exception exc)
						{
							Global.WriteOutput("Error populating data cell. " + exc.Message);
						}
                    }
                }
                else
                {
                    break;
                }
            }
            tw.Close();
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
                        DBMgr.SQLBulkLoad(_netDefNode.NetworkDefinition.TableName, strOutFile, '\t');
						break;
					case "ORACLE":
						List<string> columnNames = new List<string>();
						//we don't insert IDs.  They are generated automatically. 
						columnNames.Add( "ROUTES" );
						columnNames.Add( "BEGIN_STATION" );
						columnNames.Add( "END_STATION" );
						columnNames.Add( "DIRECTION" );
						columnNames.Add( "FACILITY" );
						columnNames.Add( "SECTION" );
						columnNames.Add( "AREA" );
						columnNames.Add( "GEOMETRY" );
						columnNames.Add( "ENVELOPE_MINX" );
						columnNames.Add( "ENVELOPE_MAXX" );
						columnNames.Add( "ENVELOPE_MINY" );
						columnNames.Add( "ENVELOPE_MAXY" );
						try
						{
                            DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, _netDefNode.NetworkDefinition.TableName, strOutFile, columnNames, "\\t");
						}
						catch( Exception ex )
						{
							Global.WriteOutput( "Oracle bulk load failed: " + ex.Message );
						}
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				}
            CreateGridView();

            // Write out the errors to the output window.
            // Create error output in Output window.
            String strError = "";
            Global.ClearOutputWindow();
            foreach (String str in listErrors)
            {
                strError += str + "\r\n";
            }
            Global.WriteOutput(strError);
            this.Cursor = Cursors.Default;
        }

        public void OnVerify()
        {
            String strQuery;
            DataSet ds;
            if (checkBoxAllowEdit.Checked) dataAdapter.Update((DataTable)binding.DataSource);
            String strErrorList = "";
            if (m_bLinear)
            {
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            strQuery = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE ROUTES<>'' ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
							break;
						case "ORACLE":
                            strQuery = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE ROUTES LIKE '_%' ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for OnVerify()" );
							//break;
					}

				ds = DBMgr.ExecuteQuery(strQuery);
                String strRoute = "";
                String strDirection = "";
                String strTrimDirection;
                String strTrimRoute;
                //TODO: Move message box to output window. Output window address should be global.
                int nRow = 0;
                int nError = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    nRow++;

                    if (row.ItemArray[1].ToString() == strRoute && row.ItemArray[4].ToString() == strDirection)
                    {
                        strRoute = row.ItemArray[1].ToString();
                        strDirection = row.ItemArray[4].ToString();
                        nError++;
                        strErrorList += "Error: Duplicate Route: " + strRoute + "  Direction:" + strDirection + "\n";
                        continue;
                    }
                    try
                    {
                        if (float.Parse(row.ItemArray[2].ToString()) >= float.Parse(row.ItemArray[3].ToString()))
                        {
                            strRoute = row.ItemArray[1].ToString();
                            strDirection = row.ItemArray[4].ToString();
                            nError++;
                            strErrorList += "Error: Begin station is greater to or equal to end station for  Route: " + strRoute + "  Direction:" + strDirection + "\n";
                            continue;
                        }
                    }
                    catch
                    {
                        strRoute = row.ItemArray[1].ToString();
                        strDirection = row.ItemArray[4].ToString();
                        nError++;
                        strErrorList += "Error: Postive numbers must be entered for begin and end station. Route: " + strRoute + "  Direction:" + strDirection + "\n";
                        continue;


                    }

                    strTrimDirection = row.ItemArray[4].ToString().Trim();
                    strTrimRoute = row.ItemArray[1].ToString().Trim();

                    if (strTrimDirection == "")
                    {
                        strRoute = row.ItemArray[1].ToString();
                        strDirection = row.ItemArray[4].ToString();
                        nError++;
                        strErrorList += "Error: Direction is blank for row " + nRow.ToString() + " Route: " + strTrimRoute + "\n";
                        continue;
                    }

                    if (strTrimRoute == "")
                    {
                        strTrimRoute = row.ItemArray[1].ToString();
                        strDirection = row.ItemArray[4].ToString();
                        nError++;
                        strErrorList += "Error: Direction is blank for row " + nRow.ToString() + "\n";
                        continue;
                    }

                    if (strTrimRoute.Contains("'") || strTrimDirection.Contains("'"))
                    {
                        strTrimDirection = row.ItemArray[4].ToString().Trim();
                        strTrimRoute = row.ItemArray[1].ToString().Trim();
                        nError++;
                        strErrorList += "Error: Neither ROUTE or DIRECTION may contain a single quote or apostrophe(') Route:" + strRoute + " Direction:" + strDirection + " row: " + nRow.ToString() + "\n";
                        continue;
                    }


                    strRoute = row.ItemArray[1].ToString();
                    strDirection = row.ItemArray[4].ToString();

                }
                strErrorList += nRow.ToString() + " rows checked.  Number of errors: " + nError.ToString() + "\n";
            }
            else
            {
                //Things to check.
                //Blank facility.
                //Blank section.
                //Non number area.
                //Area less than 0
                //Repeat section
                if (comboFacility.Text != "All")
                {
                    strErrorList += "Network verifacation is performed on all sections not just those currently displayed." + "\n";

                }


                String strLastSection = "";
                String strLastFacility = "";
                String strSection = "";
                String strFacility = "";
                String strArea;


					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY <>'' ORDER BY FACILITY, SECTION";
							break;
						case "ORACLE":
                            strQuery = "SELECT ID_,FACILITY,SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName + " WHERE FACILITY LIKE '_%' ORDER BY FACILITY, SECTION";
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for OnVerify()" );
							//break;
					}

				ds = DBMgr.ExecuteQuery(strQuery);
                int nRow = 0;
                int nError = 0;
                float fArea;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    strFacility = row.ItemArray[1].ToString().Trim();
                    strSection = row.ItemArray[2].ToString().Trim();
                    strArea = row.ItemArray[3].ToString().Trim();
                    nRow++;
                    if (strFacility == "")
                    {
                        strLastFacility = strFacility;
                        strLastSection = strSection;
                        nError++;
                        strErrorList += "Error: Blank facility row: " + nRow.ToString() + "\n";
                        continue;
                    }

                    if (strSection == "")
                    {
                        strLastFacility = strFacility;
                        strLastSection = strSection;
                        nError++;
                        strErrorList += "Error: Blank Section for Facility:" + strFacility + " row: " + nRow.ToString()+ "\n";
                        continue;
                    }

                    if (strFacility == strLastFacility && strSection == strLastSection)
                    {
                        strLastFacility = strFacility;
                        strLastSection = strSection;
                        nError++;
                        strErrorList += "Error: Duplicate Facility:" + strFacility + " Section:" + strSection + " row: " + nRow.ToString()+ "\n";
                        continue;
                    }

                    if (strFacility.Contains("'") || strSection.Contains("'"))
                    {
                        strLastFacility = strFacility;
                        strLastSection = strSection;
                        nError++;
                        strErrorList += "Error: Neither FACILITY or SECTION may contain a single quote or apostrophe(') Facility:" + strFacility + " Section:" + strSection + " row: " + nRow.ToString()+ "\n";
                        continue;
                    }

                    try
                    {
                        fArea = float.Parse(strArea);
                        if (fArea <= 0)
                        {
                            strLastFacility = strFacility;
                            strLastSection = strSection;
                            nError++;
                            strErrorList += "Error: Area is less than or equal to zero for Facility:" + strFacility + " Section:" + strSection + " row: " + nRow.ToString()+ "\n";
                            continue;

                        }
                    }
                    catch
                    {
                        strLastFacility = strFacility;
                        strLastSection = strSection;
                        nError++;
                        strErrorList += "Error: Area not a number for  Facility:" + strFacility + " Section:" + strSection + " row: " + nRow.ToString()+ "\n";
                        continue;
                    }
                }
                strErrorList += nRow.ToString() + " rows checked.  Number of errors: " + nError.ToString()+ "\n";
            }
            Global.WriteOutput(strErrorList);
        }

        //private void buttonExport_Click(object sender, EventArgs e)
        //{

        //    TextWriter tw = new StreamWriter(@"c:\out.html");

        //    tw.WriteLine("<HTML>");
        //    tw.WriteLine("<HEAD>");
        //    tw.WriteLine("<TITLE>Linear Route Definition</TITLE>");
        //    tw.WriteLine("</HEAD>");
        //    tw.WriteLine("<BODY>");
        //    tw.WriteLine("<H1>Linear Route Definition</H1>");
        //    tw.WriteLine("<H3>Report Date:" + DateTime.Now.ToShortDateString() + "</BR>");
        //    tw.WriteLine("Generated by:" + "Global.UserID" + "</BR>");
        //    tw.WriteLine("Database:" + "Global.DataSource" + "</H3>");
        //    tw.WriteLine("</BR>");



        //    tw.WriteLine("<TABLE>");
        //    tw.WriteLine("<TR>");
        //    String strCol = "";

        //    foreach(DataGridViewTextBoxColumn col in dgvNetworkDefinition.Columns)
        //    {
        //        if (col.Width > 5)
        //        {
        //            strCol += "<TD><H3>" + col.Name + "</H3></TD>";
        //        }
        //    }
        //    tw.WriteLine(strCol);
        //    tw.WriteLine("</TR>");

        //    foreach (DataGridViewRow row in dgvNetworkDefinition.Rows)
        //    {
        //        tw.WriteLine("<TR>");
        //        strCol = "";
        //        foreach (DataGridViewCell cell in row.Cells)
        //        {
        //            if (cell.Value != null)
        //            {
        //                if (cell.Size.Width > 5)
        //                {
        //                    strCol += "<TD align='center'>" + cell.Value.ToString() + "</TD>";
        //                }
        //            }
        //        }
        //        tw.WriteLine(strCol);

        //        tw.WriteLine("</TR>");
        //    }
        //    tw.WriteLine("</TABLE>");            
        //    tw.WriteLine("</BODY>");  
        //    tw.WriteLine("</HTML>");  
        //    tw.Close();
        //}

        private void checkBoxAllowEdit_CheckedChanged(object sender, EventArgs e)
        {
	
			if( checkBoxAllowEdit.Checked )
			{
				if( Global.SecurityOperations.CanModifyNetworkDefinitionData() )
				{
					UnlockDataGridViewForModify( dgvNetworkDefinition );
					UnlockComboBox( cbUnits );

					if( Global.SecurityOperations.CanCreateNetworkDefinitionData() )
					{
						UnlockButton( btnImportGeometries );
						UnlockButton( btnImportShapefile );

						UnlockDataGridViewForCreate( dgvNetworkDefinition );

						//UnlockToolStripButton( bindingNavigatorAddNewItem );
						//bindingNavigatorAddNewItem.Enabled = true;
						//bindingNavigatorNetworkDefinition.AddNewItem = bindingNavigatorAddNewItem;

						//cmsNetworkDefintion.Items.Add( tsmiPaste );
						UnlockToolStripMenuItem( tsmiPaste );

						UnlockBindingNavigatorForCreate( bindingNavigatorNetworkDefinition );

						if( Global.SecurityOperations.CanDeleteNetworkDefintionData() )
						{
							UnlockDataGridViewForCreateDestroy( dgvNetworkDefinition );

							UnlockBindingNavigatorForCreateDestroy( bindingNavigatorNetworkDefinition );

							//UnlockToolStripButton( bindingNavigatorDeleteItem );
							//bindingNavigatorDeleteItem.Enabled = true;
							//bindingNavigatorNetworkDefinition.DeleteItem = bindingNavigatorDeleteItem;

							//cmsNetworkDefintion.Items.Add( tsmiDelete );
							UnlockToolStripMenuItem( tsmiDelete );
						}
					}
				}
			}
			else
			{
				SecureForm();
				//dgvNetworkDefinition.ReadOnly = true;
				//cbUnits.Enabled = false;

				//btnImportGeometries.Enabled = false;
				//btnImportShapefile.Enabled = false;

				//dgvNetworkDefinition.AllowUserToAddRows = false;
				//bindingNavigatorAddNewItem.Enabled = false;
				//bindingNavigatorNetworkDefinition.AddNewItem = null;

				//dgvNetworkDefinition.AllowUserToDeleteRows = false;
				//bindingNavigatorDeleteItem.Enabled = false;
				//bindingNavigatorNetworkDefinition.DeleteItem = null;

				//cmsNetworkDefintion.Items.Remove( tsmiPaste );
				//cmsNetworkDefintion.Items.Remove( tsmiDelete );
			}
        }

        private void btnImportGeometries_Click(object sender, EventArgs e)
        {
			ImportGeometries();
        }

        private void btnImportShapefile_Click(object sender, EventArgs e)
        {
			ImportShapefile();
        }

        private void cmsAddShapeFileRows_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
			//dsmelser
			//TODO: this function may or may not require security checks, need to talk to chad about it

            String strKey;
            NetworkDefinitionData ndd;
            TextWriter tw = null;
            String strOutFile = "";

			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			strOutFile = strMyDocumentsFolder + "\\paste.txt";
            try
            {
                tw = new StreamWriter(strOutFile);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
                return;
            }

            if (e.ClickedItem.Name == "tsmiAddSelected")
            {
                foreach (DataGridViewRow dgvRow in dgvShapeFileOnlyNDDs.SelectedRows)
                {
                    if (dgvRow.Cells[0].Value == null)
                    {
                        continue;
                    }
					strKey = dgvRow.Cells["FACILITY"].Value.ToString() + '\t' + dgvRow.Cells["SECTION"].Value.ToString();
                    if (m_htSFDs.Contains(strKey))
                    {
                        ndd = (NetworkDefinitionData)m_htSFDs[strKey];
						tw.WriteLine(System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + ndd.Facility + '\t' + ndd.Section + '\t' + ndd.Area + '\t' + ndd.Geometry + '\t' + ndd.EnvelopeMinX + '\t' + ndd.EnvelopeMinY + '\t' + ndd.EnvelopeMaxX + '\t' + ndd.EnvelopeMaxY);
                    }
                }
                tw.Close();
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
                            DBMgr.SQLBulkLoad(_netDefNode.NetworkDefinition.TableName, strOutFile, '\t');
							break;
						case "ORACLE":
							throw new NotImplementedException( "TODO: figure out columns for cmsAddShapeFileRows_ItemClicked()" );
							//DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "NETWORK_DEFINITION", strOutFile, 
							//break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
							//break;
					}
                CreateGridView();
                foreach (DataGridViewRow dgvRow in dgvShapeFileOnlyNDDs.SelectedRows)
                {
                    dgvShapeFileOnlyNDDs.Rows.Remove(dgvRow);
                }
                return;
            }
            if (e.ClickedItem.Name == "tsmiAddAll")
            {
                foreach (DataGridViewRow dgvRow in dgvShapeFileOnlyNDDs.Rows)
                {
                    if (dgvRow.Cells[0].Value == null)
                    {
                        continue;
                    }
					if (!m_bLinear)
					{
						strKey = dgvRow.Cells["FACILITY"].Value.ToString() + '\t' + dgvRow.Cells["SECTION"].Value.ToString();
						if (m_htSFDs.Contains(strKey))
						{
							ndd = (NetworkDefinitionData)m_htSFDs[strKey];
							switch (DBMgr.NativeConnectionParameters.Provider)
							{
								case "ORACLE":
									tw.WriteLine(System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + ndd.Facility + '\t' + ndd.Section + '\t' + ndd.Area + '\t' + ndd.Geometry + '\t' + ndd.EnvelopeMinX + '\t' + ndd.EnvelopeMinY + '\t' + ndd.EnvelopeMaxX + '\t' + ndd.EnvelopeMaxY);
									//tw.WriteLine(ndd.Facility + '\t' + ndd.Section + '\t' + ndd.Area + '\t' + ndd.Geometry + '\t' + ndd.EnvelopeMinX + '\t' + ndd.EnvelopeMinY + '\t' + ndd.EnvelopeMaxX + '\t' + ndd.EnvelopeMaxY);
									break;
								case "MSSQL":
									tw.WriteLine(System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + ndd.Facility + '\t' + ndd.Section + '\t' + ndd.Area + '\t' + ndd.Geometry + '\t' + ndd.EnvelopeMinX + '\t' + ndd.EnvelopeMinY + '\t' + ndd.EnvelopeMaxX + '\t' + ndd.EnvelopeMaxY);
									break;
								default:
									throw new NotImplementedException( "TODO: Implment ANSI version of cmsAddShapeFileRows_ItemClicked()" );
							}
						}
					}
					else
					{
						strKey = dgvRow.Cells["ROUTES"].Value.ToString() + '\t' + dgvRow.Cells["BEGIN_STATION"].Value.ToString() + '\t' + dgvRow.Cells["END_STATION"].Value.ToString() + '\t' + dgvRow.Cells["DIRECTION"].Value.ToString();
						if (m_htSFDs.Contains(strKey))
						{
							ndd = (NetworkDefinitionData)m_htSFDs[strKey];
							switch (DBMgr.NativeConnectionParameters.Provider)
							{
								case "ORACLE":
									tw.WriteLine(ndd.Routes + '\t' + ndd.Begin_Station + '\t' + ndd.End_Station + '\t' + ndd.Direction + "\t\t\t\t" + ndd.Geometry + '\t' + ndd.EnvelopeMinX + '\t' + ndd.EnvelopeMinY + '\t' + ndd.EnvelopeMaxX + '\t' + ndd.EnvelopeMaxY);
									break;
								case "MSSQL":
									tw.WriteLine('\t' + ndd.Routes + '\t' + ndd.Begin_Station + '\t' + ndd.End_Station + '\t' + ndd.Direction + "\t\t\t\t" + ndd.Geometry + '\t' + ndd.EnvelopeMinX + '\t' + ndd.EnvelopeMinY + '\t' + ndd.EnvelopeMaxX + '\t' + ndd.EnvelopeMaxY);
									break;
								default:
									throw new NotImplementedException( "TODO: Implment ANSI version of cmsAddShapeFileRows_ItemClicked()" );
							}
						}
					}
                }
                tw.Close();
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
                        DBMgr.SQLBulkLoad(_netDefNode.NetworkDefinition.TableName, strOutFile, '\t');
						break;
					case "ORACLE":
						List<string> columnNames = new List<string>();
						columnNames.Add("ROUTES");
						columnNames.Add("BEGIN_STATION");
						columnNames.Add("END_STATION");
						columnNames.Add("DIRECTION");
						columnNames.Add("FACILITY");
						columnNames.Add("SECTION");
						columnNames.Add("AREA");
						columnNames.Add("GEOMETRY");
						columnNames.Add("ENVELOPE_MINX");
						columnNames.Add("ENVELOPE_MINY");
						columnNames.Add("ENVELOPE_MAXX");
						columnNames.Add("ENVELOPE_MAXY");
                        DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, _netDefNode.NetworkDefinition.TableName, strOutFile, columnNames, "\\t");
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
                CreateGridView();
                foreach (DataGridViewRow dgvRow in dgvShapeFileOnlyNDDs.Rows)
                {
                    dgvShapeFileOnlyNDDs.Rows.Remove(dgvRow);
                }
                return;
            }
            tw.Close();
        }

        private void cbUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
			if( Global.SecurityOperations.CanModifyNetworkDefinitionData() )
			{
				String strUpdate = "UPDATE OPTIONS SET OPTION_VALUE = '" + cbUnits.Text + "' WHERE OPTION_NAME = 'NETWORK_DEFINITION_UNITS'";
				try
				{
					DBMgr.ExecuteNonQuery( strUpdate );
				}
				catch( Exception exc )
				{
					Global.WriteOutput( "Error: NETWORK_DEFINITION_UNITS not updated correctly. " + exc.Message );
				}
			}
        }

        private void tsmiPaste_Click(object sender, EventArgs e)
        {
			if( Global.SecurityOperations.CanCreateNetworkDefinitionData() )
			{
				ImportRows();
			}
        }

        private void toolStripButtonVerifyNetwork_Click(object sender, EventArgs e)
        {
            OnVerify();
        }

		private void tsmiDelete_Click(object sender, EventArgs e)
		{
			try
			{
				if (checkBoxAllowEdit.Checked)
				{
					if( Global.SecurityOperations.CanDeleteNetworkDefintionData() )
					{
						foreach( DataGridViewRow dr in dgvNetworkDefinition.SelectedRows )
						{
							dgvNetworkDefinition.Rows.Remove( dr );
						}
						dataAdapter.Update( ( DataTable )binding.DataSource );
					}
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not update the database table. " + exc.Message);
				return;
			}
		}

		private void dgvNetworkDefinition_Leave( object sender, EventArgs e )
		{
			//dsmelser
			//we don't want to update no matter what here
			//I'm adding this if statement for the time being until we can get proper security checks as we go.
			if( checkBoxAllowEdit.Checked )
			{
				dataAdapter.Update( ( DataTable )binding.DataSource );
			}
		}

		private void bindingNavigatorDeleteItem_Click( object sender, EventArgs e )
		{
			try
			{
				if( checkBoxAllowEdit.Checked )
				{
					if( Global.SecurityOperations.CanDeleteNetworkDefintionData() )
					{
						foreach( DataGridViewRow dr in dgvNetworkDefinition.SelectedRows )
						{
							dgvNetworkDefinition.Rows.Remove( dr );
						}
						dataAdapter.Update( ( DataTable )binding.DataSource );
					}
				}
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: Could not update the database table. " + exc.Message );
				return;
			}
		}

		private void tsmiCopy_Click( object sender, EventArgs e )
		{
			string copyText = "";
			foreach( DataGridViewRow dr in dgvNetworkDefinition.SelectedRows )
			{
				foreach( DataGridViewColumn dc in dgvNetworkDefinition.Columns )
				{
					copyText += dr.Cells[dc.Index].Value.ToString() + '\t';
				}
				copyText += '\r' + '\n';
			}
			//dsmelser
			//If the user doesn't actually click on the left side row buttons, SelectedRows will not contain anything
			if( copyText != "" )
			{
				Clipboard.SetText( copyText );
			}
			else
			{
				Global.WriteOutput( "Error: no rows selected" );
			}
		}

		public void ImportShapefile()
		{
			dataAdapter.Update((DataTable)binding.DataSource);
			FormImportShapeFile formImportShapefile;
			
			// Import geo-spatial information into the RC database.
			// Start by launching a new user interface window.
			if (!FormManager.IsFormImportShapefileOpen(out formImportShapefile))
			{
				formImportShapefile = new FormImportShapeFile(m_bLinear);
				FormManager.AddFormImportShapeFile(formImportShapefile);
				if (formImportShapefile.ShowDialog(FormManager.GetDockPanel()) == DialogResult.OK)
				{
					int iHeight = this.Height;
					dgvNetworkDefinition.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					if (iHeight > 30)
					{
						dgvNetworkDefinition.Height = (iHeight - 30) / 2;
					}
					if (iHeight > 60)
					{
						dgvShapeFileOnlyNDDs.Location = new System.Drawing.Point(-1, ((iHeight - 80) / 2) + 90);
						dgvShapeFileOnlyNDDs.Height = (iHeight - 100) / 2;
					}
					this.Refresh();
					dgvShapeFileOnlyNDDs.Visible = true;
					CreateGridView();

					// Bulk load the files back into the TEMP_SHAPEFILE table
					m_listSFD = formImportShapefile.GetShapeFileOnlyNDDs();
					m_htSFDs = formImportShapefile.GetSFDHash();
					if (m_bLinear)
					{
						dgvShapeFileOnlyNDDs.Columns.Add("ROUTES", "ROUTE");
						dgvShapeFileOnlyNDDs.Columns.Add("BEGIN_STATION", "BEGIN_STATION");
						dgvShapeFileOnlyNDDs.Columns["BEGIN_STATION"].DefaultCellStyle.Format = "f4";
						dgvShapeFileOnlyNDDs.Columns.Add("END_STATION", "END_STATION");
						dgvShapeFileOnlyNDDs.Columns["END_STATION"].DefaultCellStyle.Format = "f4";
						dgvShapeFileOnlyNDDs.Columns.Add("DIRECTION", "DIRECTION");
						object[] oLNDValues = new object[4];
						string linearKey = "";
						for (int i = 0; i < m_listSFD.Count; i++)
						{
							oLNDValues[0] = m_listSFD[i].Routes;
							oLNDValues[1] = m_listSFD[i].Begin_Station;
							oLNDValues[2] = m_listSFD[i].End_Station;
							oLNDValues[3] = m_listSFD[i].Direction;
							linearKey = m_listSFD[i].Routes + '\t' +
										m_listSFD[i].Begin_Station + '\t' +
										m_listSFD[i].End_Station + '\t' +
										m_listSFD[i].Direction;

							dgvShapeFileOnlyNDDs.Rows.Add(oLNDValues);
						}
						
					}
					else
					{
						dgvShapeFileOnlyNDDs.Columns.Add("FACILITY", "FACILITY");
						dgvShapeFileOnlyNDDs.Columns.Add("SECTION", "SECTION");
						dgvShapeFileOnlyNDDs.Columns.Add("AREA", "AREA");
						object[] oSFDValues = new object[3];
						String strFacilitySection = "";
						for (int i = 0; i < m_listSFD.Count; i++)
						{
							oSFDValues[0] = m_listSFD[i].Facility;
							oSFDValues[1] = m_listSFD[i].Section;
							strFacilitySection = m_listSFD[i].Facility + '\t' + m_listSFD[i].Section;
							oSFDValues[2] = m_listSFD[i].Area;
							dgvShapeFileOnlyNDDs.Rows.Add(oSFDValues);
						}
					}
				}
				else
				{
					formImportShapefile.Close();
				}
			}

		}

		public void ImportGeometries()
		{
			FormImportGeometries formImportGeometries;

			// Import geo-spatial information into the RC database.
			// Start by launching a new user interface window.
			if (!FormManager.IsFormImportGeometriesOpen(out formImportGeometries))
			{
				FormImportGeometries formGeometries;
				formGeometries = new FormImportGeometries(m_bLinear);
				FormManager.AddFormImportGeometries(formGeometries);
				formGeometries.Show(FormManager.GetDockPanel());
			}
		}
        ///// <summary>
        ///// Update for ImageView
        ///// </summary>
        //public override void UpdateNode(DockPanel dockPanel)
        //{
        //    if (!this.ImageView)
        //    {
        //        this.ImageView = true;
        //        this.HideOnClose = true;
        //    }
        //    this.Show(dockPanel); 
        //}

        private void FormNetworkDefinition_VisibleChanged(object sender, EventArgs e)
        {
        }

        private void FormNetworkDefinition_Deactivate(object sender, EventArgs e)
        {
            if (this.ImageView)
            {
                if (checkBoxAllowEdit.Checked) dataAdapter.Update((DataTable)binding.DataSource);
            }
        }

		private void tsbJoinMultilinestring_Click(object sender, EventArgs e)
		{
			FormMultiLineStringJoin formMLSJ = new FormMultiLineStringJoin();
			DockPanel dp = FormManager.GetDockPanel();
			formMLSJ.Show(dp);
			
		}

		private void tsbDefineMultiLineStrings_Click(object sender, EventArgs e)
		{
			FormMultiLineStringDefinition formMLSD = new FormMultiLineStringDefinition();
			DockPanel dp = FormManager.GetDockPanel();
			formMLSD.Show(dp);
		}


        private void comboBoxUpdateNetworkDefinition_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (comboBoxUpdateNetworkDefinition.SelectedItem.ToString() == "Add New...")
            {
                // Create a new network connection in the connection params table.
                FormCreateRemoteNetworkDefinition formNewNetDef = new FormCreateRemoteNetworkDefinition();
                if (formNewNetDef.ShowDialog() == DialogResult.OK)
                {
                    ConnectionParameters cp = formNewNetDef.ConnectionParameters;

                    // Update the selection box with the new connection name.
                    comboBoxUpdateNetworkDefinition.Items.Add(cp.ConnectionName);
                }
                this.Refresh();
            }
            else
            {
                // Warn the user of making changes to the network definition table.
                if (MessageBox.Show("Using this utility will wipe out the existing NETWORK_DEFINITION and use imported values.  Do you wish to continue?", "Warning!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
                    Directory.CreateDirectory(strMyDocumentsFolder);

                    String strOutFile = strMyDocumentsFolder + "\\NetworkDefinitionRemote.txt";
                    TextWriter tw = new StreamWriter(strOutFile);
                    try
                    {
                        ConnectionParameters cp = DBMgr.GetConnectionParameter(DBMgr.ConnectionParameterType.NETWORK, comboBoxUpdateNetworkDefinition.Text);
                        if (cp != null)
                        {
                            DataSet ds = DBMgr.ExecuteQuery(cp.ViewStatement, cp);
                            // Create a bulk import file for the network definition table with the following format
                            // ROUTES BEGIN_STATION END_STATION, DIRECTION, FACILITY, SECTION, AREA, GEOMETRY
                            // Use the imported geometries (if they exist) to create the envelopes for the imported section
                            // and add them to the file as EnvelopeMinX EnvelopeMinY EnvelopeMaxX EnvelopeMaxY.

                            string deleteND = "DELETE FROM " + _netDefNode.NetworkDefinition.TableName;
                            DBMgr.ExecuteNonQuery(deleteND);
                            NetworkDefinitionData ndd = new NetworkDefinitionData();
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                if (!String.IsNullOrEmpty(dr["GEOMETRY"].ToString()))
                                {
                                    Geometry ndGeo = Geometry.GeomFromText(dr["GEOMETRY"].ToString());
                                    BoundingBox geoBox = ndGeo.GetBoundingBox();
                                    ndd.EnvelopeMaxX = geoBox.Max.X;
                                    ndd.EnvelopeMinX = geoBox.Min.X;
                                    ndd.EnvelopeMaxY = geoBox.Max.Y;
                                    ndd.EnvelopeMinY = geoBox.Min.Y;
                                }

                                tw.WriteLine("\t" + dr["ROUTES"].ToString() + "\t" +
                                    dr["BEGIN_STATION"].ToString() + "\t" +
                                    dr["END_STATION"].ToString() + "\t" +
                                    dr["DIRECTION"].ToString() + "\t" +
                                    dr["FACILITY"].ToString() + "\t" +
                                    dr["SECTION"].ToString() + "\t" +
                                    dr["AREA"].ToString() + "\t" +
                                    dr["GEOMETRY"].ToString() + "\t" +
                                    ndd.EnvelopeMinX + "\t" +
                                    ndd.EnvelopeMinY + "\t" +
                                    ndd.EnvelopeMaxX + "\t" +
                                    ndd.EnvelopeMaxY);
                            }
                            tw.Close();

                            switch (cp.Provider)
                            {
                                case "MSSQL":
                                    DBMgr.SQLBulkLoad(_netDefNode.NetworkDefinition.TableName, strOutFile, '\t');
                                    break;
                                case "ORACLE":
                                    //throw new NotImplementedException("Remote network definition not implemented for Oracle yet.");
                                    List<string> columnNames = new List<string>();
                                    columnNames.Add("ID_");
                                    columnNames.Add("ROUTES");
                                    columnNames.Add("BEGIN_STATION");
                                    columnNames.Add("END_STATION");
                                    columnNames.Add("DIRECTION");
                                    columnNames.Add("FACILITY");
                                    columnNames.Add("SECTION");
                                    columnNames.Add("AREA");
                                    columnNames.Add("GEOMETRY");
                                    columnNames.Add("ENVELOPE_MINX");
                                    columnNames.Add("ENVELOPE_MINY");
                                    columnNames.Add("ENVELOPE_MAXX");
                                    columnNames.Add("ENVELOPE_MAXY");
                                    DBMgr.OracleBulkLoad(cp, _netDefNode.NetworkDefinition.TableName, strOutFile, columnNames, "\\t");
                                    break;
                            }
                            binding = new BindingSource();

                            if (m_bLinear)
                            {
                                dataAdapter = new DataAdapter("SELECT ROUTES, BEGIN_STATION, END_STATION, DIRECTION FROM " + _netDefNode.NetworkDefinition.TableName);
                            }
                            else
                            {
                                dataAdapter = new DataAdapter("SELECT FACILITY, SECTION, AREA FROM " + _netDefNode.NetworkDefinition.TableName);
                            }
                            // Populate a new data table and bind it to the BindingSource.
                            table = new DataTable();

                            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                            dataAdapter.Fill(table);

                            binding.DataSource = table;
                            dgvNetworkDefinition.DataSource = binding;
                            if (checkBoxAllowEdit.Checked) dgvNetworkDefinition.ReadOnly = false;
                            else dgvNetworkDefinition.ReadOnly = true;
                        }
                        else
                        {
                            Global.WriteOutput("Error updating remote network definition. Could not find CONNECTION_PARAMETERS " + comboBoxUpdateNetworkDefinition.Text + ".");
                            tw.Close();
                        }
                    }
                    catch (Exception exc)
                    {
                        tw.Close();
                        Global.WriteOutput("Error during remote network update. " + exc.Message);
                    }
                }
            }
            this.Cursor = Cursors.Default;
            this.Close();
        }
	}
}
