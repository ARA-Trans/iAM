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
using System.Data.OleDb;
using System.IO;
using RoadCare3.Properties;
using RoadCareDatabaseOperations;
using System.Threading;
using DataObjects;
using RoadCareGlobalOperations;

namespace RoadCare3
{ 
	//TODO: Don't show ALL ROUTES or Years for Large TABLES
	public partial class FormAttributeDocument : BaseForm
    {
        private ConnectionParameters m_cp;
        public String m_strAttribute;
        private BindingSource binding;
        private DataAdapter dataAdapter;
        private DataTable table;
        private List<String> m_listFacility = new List<String>();
		private List<String> m_listFacilityUpper = new List<String>();
		private bool m_bString = true;
		private string m_strLinearYearFilter;
		private string m_strLinearRouteFilter;
		private string m_strSectionYearFilter;
		private string m_strSectionRouteFilter;
        private int m_nLastRowSelected = -1;

        public FormAttributeDocument(String strAttribute)
        {
            InitializeComponent();
            m_strAttribute = strAttribute;
			FormLoad(Settings.Default.ATTRIBUTE_IMAGE_KEY, Settings.Default.ATTRIBUTE_IMAGE_KEY_SELECTED);
        }

		protected void SecureForm()
		{
			checkAllowEdit.Checked = false;
			LockCheckBox( checkAllowEdit );
			LockToolStripButton( bindingNavigatorAddNewItem );
			LockToolStripButton( bindingNavigatorDeleteItem );
			LockToolStripButton( tsbImportFromDataSource );
			LockToolStripButton( tsbDeleteAll );

			//we don't want the menu to have these buttons set like that since it will add or remove rows from the datagridview
			//regardless of any checks in the item's click event (meaning that a stray update would commit those changes)
			LockDataGridView( dgvAttribute );
			pasteToolStripMenuItem.Enabled = false;

			LockToolStripMenuItem( pasteToolStripMenuItem );

			//contextMenuStripRawAttribute.Items.Remove( pasteToolStripMenuItem );

			if( Global.SecurityOperations.CanModifyRawAttributeData( m_strAttribute ) )
			{
				UnlockCheckBox( checkAllowEdit );
				//checkAllowEdit.Enabled = true;
			}
		}

		private void FormAttributeDocument_Load(object sender, EventArgs e)
        {
			SecureForm();


            // If the attribute is on a data server, then we need to check if the view created was a LRS or SRS based.
            // It is possible to have both, in which case we need not change the current (LRS) radio button.
            m_cp = DBMgr.GetAttributeConnectionObject(m_strAttribute);

            m_strLinearYearFilter = "";
			m_strLinearRouteFilter = "";
			m_strSectionYearFilter = "";
			m_strSectionRouteFilter = "";

            this.TabText = m_strAttribute;
            this.Text = m_strAttribute;


            if (m_cp == null)
            {
                Global.WriteOutput("Error:Could not connect to datasource.");
                return;
            }
			if (!m_cp.IsNative)
			{
				checkAllowEdit.CheckState = CheckState.Unchecked;
				checkAllowEdit.Enabled = false;

				dgvAttribute.ReadOnly = true;
			}
			List<String> attributeColumnHeaders = DBMgr.GetTableColumns(m_strAttribute, m_cp);
			if (m_cp.IsNative == false)
			{
				if (attributeColumnHeaders.Contains("SECTION"))
				{
					if (!attributeColumnHeaders.Contains("ROUTES"))
					{
						// Disable LRS
						rbLinearRef.Checked = false;
						rbSectionRef.Checked = true;
					}
				}
			}
			try
			{
				int iNumRecords;
				iNumRecords = DBMgr.GetTableCount(m_strAttribute, m_cp);

				LoadAttributeFilters();

				// Check to see if the number of records in the database is greater than 10k.
				// If it is, we dont want to display that many records on the screen unless we have to,
				// so we will default to the first ROUTE in the combo box as a filter.
				if (iNumRecords > 10000)
				{
					// Make sure there is more than just the "All" item in the combo box.
					// If there isnt, then there isnt much we can do about filtering the data anyway so...
					if (cbRoutes.Items.Count > 1)
					{
						cbRoutes.Text = cbRoutes.Items[1].ToString();
					}
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Couldn't get record count for attribute " + m_strAttribute + ". " + exc.Message);
			}
            CreateDataGridView();

            // Check for a native attribute in m_cp, then execute the proper execute query.
            String strSelect = "SELECT TYPE_ FROM ATTRIBUTES_ WHERE ATTRIBUTE_='" + m_strAttribute + "'";
            DataSet ds = null;
            ds = DBMgr.ExecuteQuery(strSelect);

            if (ds.Tables[0].Rows[0].ItemArray[0].ToString() != "STRING")
            {
                m_bString = false;
            }

            // Set default LRS/SRS radio button
            // TODO: Should be remembered from previous attributes.  Perhaps, placing a "global" variable in the DBManager
            // is a fantastic solution to this problem.  OR we could create a static class called GLOBAL that would hold
            // global variables for us.  <---This is the answer.
			tsbDeleteAll.ToolTipText = "Query based delete.";
        }

        private void LoadAttributeFilters()
        {
            labelAttribute.Text = m_strAttribute;
			
			labelAttribute.Refresh();
			while( labelAttribute.Size.Width > 150 )
			{
				labelAttribute.Font = new Font( labelAttribute.Font.FontFamily, labelAttribute.Font.Size * 0.99f );
				labelAttribute.Refresh();
			}

            // First clear the years combo box, allow an All option at the top of the combo box.
            cbYear.Items.Clear();

			if( rbLinearRef.Checked )
			{
				if( m_strLinearYearFilter != "" )
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
				if( m_strSectionYearFilter != "" )
				{
					cbYear.Text = m_strSectionYearFilter;
				}
				else
				{
					cbYear.Text = "All";
				}
			}

            cbYear.Items.Add("All");

			DataSet ds = DBOp.GetDistinctAttributeYears(m_strAttribute, m_cp);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cbYear.Items.Add(row.ItemArray[0].ToString());
            }

            // Now to fill in the routes cb, first we clear it though, and then we give and set its default value.
            cbRoutes.Items.Clear();

			if( rbLinearRef.Checked )
			{
				if( m_strLinearRouteFilter != "" )
				{
					cbRoutes.Text = m_strLinearRouteFilter;
				}
				else
				{
					cbRoutes.Text = "All";
				}
			}
			else
			{
				if( m_strSectionRouteFilter != "" )
				{
					cbRoutes.Text = m_strSectionRouteFilter;
				}
				else
				{
					cbRoutes.Text = "All";
				}
			}
			cbRoutes.Items.Add("All");
            if (rbLinearRef.Checked)
            {
				//network definition isn't stored remotely
				ds = DBOp.GetValidRoutes(DBMgr.NativeConnectionParameters);
				lblRouteFacility.Text = "Route:";
			}
            else
            {
				//network defintion isn't stored remotely.
				ds = DBOp.GetValidFacilities( DBMgr.NativeConnectionParameters );
                lblRouteFacility.Text = "Facility:";
            }
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cbRoutes.Items.Add(row[0].ToString());
                m_listFacility.Add(row[0].ToString());
				m_listFacilityUpper.Add(row[0].ToString().ToUpper());
            }
            
        }

        /// <summary>
        /// Fills the dgvAttribute control.  Keep an eye on this function for large databases, this function may be critical
        /// for performance.
        /// </summary>
        private void CreateDataGridView()
        {
            String strYear = cbYear.Text.ToString();
            String strRouteFacility = cbRoutes.Text.ToString();
			
            bool bLinear;

            if (rbLinearRef.Checked) bLinear = true;
            else bLinear = false;

            String strSearch = tbSearch.Text.ToString();
            String strWhere = "";

			if( strRouteFacility != "All" )
			{
				if( bLinear )
				{
					strWhere = " WHERE ROUTES ='" + strRouteFacility + "' ";
				}
				else
				{
					strWhere = " WHERE FACILITY ='" + strRouteFacility + "' ";
				}

			}
			else//ALL
			{
				switch( m_cp.Provider )
				{
					case "MSSQL":
						if( bLinear )
						{
							strWhere = " WHERE ROUTES <> '' ";
						}
						else
						{
							strWhere = " WHERE FACILITY <> ''";
						}
						break;
					case "ORACLE":
						if( bLinear )
						{
							strWhere = " WHERE ROUTES LIKE '_%' ";
						}
						else
						{
							strWhere = " WHERE FACILITY LIKE '_%'";
						}
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				}
			}

			if( strYear != "All" )
			{
				switch( m_cp.Provider )
				{
					case "MSSQL":
						strWhere += "AND year(DATE_) = '" + strYear + "' ";
						break;
					case "ORACLE":
						strWhere += "AND TO_CHAR( DATE_, 'YYYY' ) = '" + strYear + "' ";
						break;
					default:
						throw new NotImplementedException( "TODO: Add ansi implementation for CreateDataGridView()" );
				}

			}

            strSearch = strSearch.Trim();

            if (strSearch != "")
            {
                strWhere += " AND (";
                String strBracket = "[" + m_strAttribute + "]";
                strSearch = strSearch.Replace(strBracket, " DATA_ ");
                strWhere += strSearch + ")";
            }

            if (m_cp.Provider == "ORACLE")
            {
                strWhere = strWhere.Replace("[", "").Replace("]", "");
            }

            binding = new BindingSource();
            
            String strQuery;
            if (rbLinearRef.Checked)//bool for route or section
            {
				strQuery = "SELECT ID_,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,DATE_,DATA_ AS " + m_strAttribute + " FROM " + m_strAttribute + strWhere;
			}
            else
            {
				strQuery = "SELECT ID_, FACILITY,SECTION,SAMPLE_,DATE_,DATA_ AS " + m_strAttribute + " FROM " + m_strAttribute + strWhere;
            }
            try
            {
                dataAdapter = new DataAdapter(strQuery, m_cp);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Attribute SQL query could not be parsed. " + exc.Message);
                return;
            }

            // Populate a new data table and bind it to the BindingSource.
            table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            try
            {
                dataAdapter.Fill(table);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not fill data adapter. " + exc.Message);
                return;
            }
			switch (m_cp.Provider)
			{
				case "MSSQL":
					break;
				case "ORACLE":
					table.TableName = m_strAttribute;
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
			}

			binding.DataSource = table;
            dgvAttribute.DataSource = binding;
            bindingNavigatorAttributeRaw.BindingSource = binding;
            if (bLinear)
            {
                try
				{
					dgvAttribute.Columns["ID_"].Visible = false;
					dgvAttribute.Columns["DATE_"].AutoSizeMode = ( DataGridViewAutoSizeColumnMode.Fill );
					
					dgvAttribute.Columns["ROUTES"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dgvAttribute.Columns["BEGIN_STATION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
                    dgvAttribute.Columns["END_STATION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
                    dgvAttribute.Columns["DIRECTION"].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);
                    dgvAttribute.Columns[m_strAttribute].AutoSizeMode = (DataGridViewAutoSizeColumnMode.Fill);

					dgvAttribute.Columns["BEGIN_STATION"].DefaultCellStyle.Format = "G";
					dgvAttribute.Columns["END_STATION"].DefaultCellStyle.Format = "G";


					DataSet formatSet = DBMgr.ExecuteQuery("SELECT FORMAT FROM ATTRIBUTES_ WHERE ATTRIBUTE_ = '" + m_strAttribute + "'");
					if( !String.IsNullOrEmpty(formatSet.Tables[0].Rows[0][0].ToString()))
					{
						try
						{
							dgvAttribute.Columns[m_strAttribute].DefaultCellStyle.Format = formatSet.Tables[0].Rows[0][0].ToString();
						}
						catch( Exception ex )
						{
							Global.WriteOutput( "ERROR [Format string " + formatSet.Tables[0].Rows[0][0].ToString() + " is invalid.]: " + ex.Message );
						}
					}
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: Could not fill attribute datagridview. " + exc.Message);
                }
            }
            dgvAttribute.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
        }

        private void rbSectionRef_CheckedChanged(object sender, EventArgs e)
        {
			if( rbLinearRef.Checked )
			{
				m_strSectionRouteFilter = cbRoutes.Text;
				m_strSectionYearFilter = cbYear.Text;
			}
			else
			{
				m_strLinearRouteFilter = cbRoutes.Text;
				m_strLinearYearFilter = cbYear.Text;
			}
			try
			{
				if (binding != null && binding.DataSource != null)
				{
					dataAdapter.Update((DataTable)binding.DataSource);
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Could not update attribute data grid. " + exc.Message);
				return;
			}
            CreateDataGridView();
            LoadAttributeFilters();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            bool bLinear = rbLinearRef.Checked;
            String strQuery = tbSearch.Text;
            FormQueryRaw form = new FormQueryRaw(m_strAttribute, bLinear, strQuery);
            if (form.ShowDialog() == DialogResult.OK)
            {
                tbSearch.Text = form.m_strQuery;

            }
            dataAdapter.Update((DataTable)binding.DataSource);
			CreateDataGridView();
        }

        private void FormAttributeDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dgvAttribute.Columns.Count == 0) return;
			try
			{
				switch (m_cp.Provider)
				{
					case "MSSQL":
						break;
					case "ORACLE":
						((DataTable)binding.DataSource).TableName = m_strAttribute;
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					//break;
				}
				dataAdapter.Update( ( DataTable )binding.DataSource );
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Database commit error. " + exc.Message);
				e.Cancel = true;
			}
        }

        private void FormAttributeDocument_FormClosed(object sender, FormClosedEventArgs e)
        {
			FormUnload();
            FormManager.RemoveRawAttributeForm(this);
        }

        private void dgvAttribute_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteRows();
        }

        private void PasteRows()
        {
			bool error = false;
            Global.ClearOutputWindow();
            if(!checkAllowEdit.Checked)
            {
                Global.WriteOutput("Note: External data cannot be pasted into a Raw Attribute if Allow Attribute Edit is not checked.");
                return;
            }
			try
			{
				dataAdapter.Update( ( DataTable )binding.DataSource );
			}
			catch( Exception ex )
			{
				Global.WriteOutput( "ERROR: Could not update table prior to pasting data: " + ex.Message );
				error = true;
			}
			finally
			{
				dataAdapter.Dispose();
			}

			if( !error )
			{
				String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
				Directory.CreateDirectory(strMyDocumentsFolder);

				String strOutFile = strMyDocumentsFolder + "\\paste.txt";
				TextWriter tw = new StreamWriter( strOutFile );

				// List of errors during import to print out after PasteRows completes.
				List<String> listErrors = new List<String>();

				bool bLinear = rbLinearRef.Checked;

				string s = Clipboard.GetText();
				s = s.Replace( "\r\n", "\n" );
				string[] lines = s.Split( '\n' );
				try
				{
					foreach (string line in lines)
					{
						if (line.Length > 0)
						{
							//Split on either commas or tabs
							string[] cells = line.Split(new string[] { "\t" }, StringSplitOptions.None);

							//INSERT INTO table_name (column1, column2,...)VALUES (value1, value2,....)
							if (bLinear)
							{
								if (cells.Length != 6)
								{
									listErrors.Add(line + ": Paste aborted because more/less than 6 columns detected");
									return;
								}
								//Trim white space.
								for (int i = 0; i < cells.Length; i++)
								{
									cells[i] = cells[i].Trim();
								}
								if (!m_listFacility.Contains(cells[0]))
								{
									listErrors.Add(line + "\t not imported because it does not exist in Facility/Route list");
									continue;
								}

								float fBegin;
								float fEnd;
								try
								{
									fBegin = float.Parse(cells[1]);
									fEnd = float.Parse(cells[2]);
								}
								catch
								{
									listErrors.Add(line + "\t not imported because BEGIN_STATION and END_STATION must be positive numbers");
									continue;
								}


								if (fBegin > fEnd)
								{
									listErrors.Add(line + "\t not imported because BEGIN_STATION must be less or equal to END_STATION");
									continue;
								}


								if (fBegin < 0 || fEnd < 0)
								{
									listErrors.Add(line + "\t not imported because BEGIN_STATION and END_STATION must be positive numbers");
									continue;
								}

								if (cells[3] == "")
								{
									listErrors.Add(line + "\t not imported because DIRECTION must be included.");
									continue;
								}

								DateTime date;
								try
								{
									date = DateTime.Parse(cells[4]);
								}
								catch
								{
									listErrors.Add(line + "\t not imported because DATE format is incorrect.");
									continue;
								}

								float fNumber;
								if (!m_bString)
								{
									try
									{
										fNumber = float.Parse(cells[5]);
									}
									catch
									{
										listErrors.Add(line + "\t not imported because number expected for data column.");
										continue;
									}

								}
								String strOut = "";
								for (int i = 0; i < cells.Length; i++)
								{
									switch (m_cp.Provider)
									{
										case "MSSQL":
											strOut += "\t";
											if (i == 4)
												strOut += "\t\t\t";
											strOut += cells[i];
											break;
										case "ORACLE":
											if (i > 0)
											{
												strOut += "\t";
											}
											if (i == 4)
											{
												strOut += "\t\t\t";
												DateTime rowDate = DateTime.Parse(cells[i]);
												strOut += rowDate.ToString("dd/MMM/yyyy");
											}
											else
											{
												strOut += cells[i];
											}
											break;
										default:
											throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
									}
								}
								tw.WriteLine(strOut);
							}
							else //SRS
							{
								if (cells.Length != 5)
								{
									listErrors.Add(line + "Paste aborted because more/less than 5 columns detected");
									return;
								}

								//Trim white space.
								for (int i = 0; i < cells.Length; i++)
								{
									cells[i] = cells[i].Trim();
								}


								if (!m_listFacility.Contains(cells[0]))
								{
									if (!m_listFacilityUpper.Contains(cells[0].ToUpper()))
									{
										listErrors.Add(line + "\t not imported because it does not exist in Facility/Route list");
										continue;
									}
									else
									{
										cells[0] = m_listFacility[m_listFacilityUpper.IndexOf(cells[0].ToUpper())];
									}
								}

								if (cells[1] == "")
								{
									listErrors.Add(line + "\t not imported because SECTION must not be blank.");
									continue;
								}

								DateTime date;
								try
								{
									date = DateTime.Parse(cells[3]);
								}
								catch
								{
									listErrors.Add(line + "\t not imported because DATE format is incorrect.");
									continue;
								}

								float fNumber;
								if (!m_bString)
								{
									try
									{
										if (cells[4].ToString() != "")
										{
											fNumber = float.Parse(cells[4]);
										}
										else
										{
											fNumber = float.NaN;
										}
									}
									catch
									{
										listErrors.Add(line + "\t not imported because number expected for data column.");
										continue;
									}

								}
								String strOut = "";
								for (int i = 0; i < cells.Length; i++)
								{
									switch (m_cp.Provider)
									{
										case "MSSQL":
											strOut += "\t";
											break;
										case "ORACLE":
											if (i > 0)
											{
												strOut += "\t";
											}
											break;
										default:
											throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
									}
									if (i == 0)
										strOut += "\t\t\t\t";
									strOut += cells[i];
								}
								tw.WriteLine(strOut);
							}
						}
						else
						{
							continue;
						}
					}
				}
				catch (Exception exc)
				{
					Global.WriteOutput("Error pasting in data rows. " + exc.Message);
				}
				finally
				{
					tw.Close();
				}
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad(m_strAttribute, strOutFile, '\t');
						break;
					case "ORACLE":
						List<string> columnNames = new List<string>();
						columnNames.Add("ROUTES");
						columnNames.Add("BEGIN_STATION");
						columnNames.Add("END_STATION");
						columnNames.Add("DIRECTION");
						columnNames.Add("FACILITY");
						columnNames.Add("SECTION");
						columnNames.Add("SAMPLE_");
						columnNames.Add("DATE_");
						columnNames.Add("DATA_");
						try
						{
							DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, m_strAttribute, strOutFile, columnNames, "\\t");
						}
						catch (Exception ex)
						{
							Global.WriteOutput("Oracle bulk load failed: " + ex.Message);
						}
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
				}
				CreateDataGridView();

				// Create error output in Output window.
				String strError = "";
				Global.ClearOutputWindow();
				foreach( String str in listErrors )
				{
					strError += str + "\r\n";
				}
				if( strError != "" )
				{
					Global.WriteOutput( strError );
				}
			}
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(dgvAttribute.GetClipboardContent());
        }

        private void tsbImportFromDataSource_Click(object sender, EventArgs e)
        {
            dataAdapter.Update((DataTable)binding.DataSource);
            dataAdapter.Dispose();
            ImportFromDataSource();
            CreateDataGridView();
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>
        /// Imports data into the raw attribute from other data sources (SQL, ACCESS, ORACLE etc.)
        /// Opens a dialog box to perform the operation.
        /// </summary>
        public void ImportFromDataSource()
        {
            DialogImportData formDataImport = null;
            if (rbLinearRef.Checked == true)
            {
                formDataImport = new DialogImportData(true, m_bString, m_strAttribute, m_listFacility);
				formDataImport.SetExampleLabel( "Example:  SELECT ROAD AS ROUTES, DIR AS DIRECTION, BMP AS BEGIN_STATION, EMP AS END_STATION, ADT AS DATA_, DATE_ FROM ROAD_INFORMATION" );
			}
            else
            {
                formDataImport = new DialogImportData(false, m_bString, m_strAttribute, m_listFacility);
				formDataImport.SetExampleLabel( "Example:  SELECT ROAD AS FACILITY, SECTION, SAMPLE_, ADT AS DATA_, DATE_ FROM ROAD_INFORMATION" );
			}
            //The Ok button isn't always succcessful so we need to just Show() the form
			formDataImport.Show();
            //while( formDataImport.Visible )
            //{
            //    Application.DoEvents();
            //}
        }

        private void dgvAttribute_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteRows();
            }
        }

        private void cbYear_SelectedIndexChanged(object sender, EventArgs e)
        {
			//throw new Exception();
            if (binding != null)
            {
                dataAdapter.Update((DataTable)binding.DataSource);
            }
            CreateDataGridView();
        }

        private void cbRoutes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (binding != null)
            {
                dataAdapter.Update((DataTable)binding.DataSource);
            }
            CreateDataGridView();
        }

		protected override string GetPersistString()
		{
			return GetType().ToString() + "," + m_strAttribute;
		}

		private void tsbDeleteAll_Click( object sender, EventArgs e )
		{
			if( !checkAllowEdit.Checked )
			{
				Global.WriteOutput( "Note: Deleting data from a Raw Attribute is not allowed if Allow Attribute Edit is not checked." );
				return;
			}
			if( MessageBox.Show( "Delete all raw attributes in this table matching the input criteria. Blank query deletes all rows in Raw Attribute.  Yes to continue.", "Delete Raw Attribute with Query", MessageBoxButtons.YesNo ) == DialogResult.Yes )
			{
				if( binding != null )
				{
					dataAdapter.Update( ( DataTable )binding.DataSource );
				}

				String strWhere = tbSearch.Text;


				String strBackupSelect = "SELECT * FROM " + m_strAttribute;
				if( strWhere.Trim() != "" )
				{
					strWhere = strWhere.Replace( "[" + m_strAttribute + "]", "DATA_" );
					strBackupSelect += " WHERE ";
					strBackupSelect += strWhere;
				}
				try
				{
					DataSet BackupDataSet = DBMgr.ExecuteQuery( strBackupSelect, m_cp );
					FileStream backupFileStream;
					StreamWriter backupStreamWriter;
					if( !Directory.Exists( @".\AttributeBackup\" ) )
					{
						Directory.CreateDirectory( @".\AttributeBackup\" );
					}
					if( BackupDataSet.Tables[0].Rows.Count > 0 )
					{
						backupFileStream = new FileStream( @".\AttributeBackup\" + m_strAttribute + ".csv", FileMode.OpenOrCreate, FileAccess.ReadWrite );
						backupStreamWriter = new StreamWriter( backupFileStream );

						//first write a timestamp and the where clause
						string backupAttributeString = DateTime.Now.ToString() + ",";
						if( strWhere.Trim() != "" )
						{
							backupAttributeString += strWhere.Trim();
						}
						else
						{
							backupAttributeString += "ALL";
						}

						backupStreamWriter.WriteLine( backupAttributeString );

						//now write the column names
						backupAttributeString = "";
						foreach( DataColumn attributeDataColumn in BackupDataSet.Tables[0].Columns )
						{
							backupAttributeString += attributeDataColumn.ColumnName + ",";
						}
						backupAttributeString = backupAttributeString.Substring( 0, backupAttributeString.Length - 1 );
						backupStreamWriter.WriteLine( backupAttributeString );

						//then write the rows
						foreach( DataRow attributeDataRow in BackupDataSet.Tables[0].Rows )
						{
							backupAttributeString = "";
							foreach( DataColumn attributeDataColumn in attributeDataRow.Table.Columns )
							{
								backupAttributeString += attributeDataRow[attributeDataColumn].ToString() + ",";
							}
							backupAttributeString = backupAttributeString.Substring( 0, backupAttributeString.Length - 1 );
							backupStreamWriter.WriteLine( backupAttributeString );
						}
						backupFileStream.Close();
					}
				}
				catch( Exception ex )
				{
					Global.WriteOutput( "Error: Error in backup select query. " + ex.Message );
					return;
				}


				String strDelete = "DELETE FROM " + m_strAttribute;
				if( strWhere.Trim() != "" )
				{
					strDelete += " WHERE ";
					strDelete += strWhere;
				}
				try
				{
					DBMgr.ExecuteNonQuery( strDelete, m_cp );
					CreateDataGridView();
				}
				catch( Exception exception )
				{
					Global.WriteOutput( "Error: Error in query delete. " + exception.Message );
					return;
				}
			}
		}

		private void dgvAttribute_Leave(object sender, EventArgs e)
		{
			try
			{
				dataAdapter.Update((DataTable)binding.DataSource);
			}
			catch (DBConcurrencyException dbConEx)
			{
				Global.WriteOutput( "Error: Database commit failed, please validate changes:  " + dbConEx.Message );
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Database commit failed, please validate changes: " + exc.Message );
			}
		}

		private void checkAllowEdit_CheckedChanged( object sender, EventArgs e )
		{
			if( checkAllowEdit.Checked )
			{
				dgvAttribute.ReadOnly = false;
				if( Global.SecurityOperations.CanCreateRawAttributeData( m_strAttribute ) )
				{
					dgvAttribute.AllowUserToAddRows = true;
					tsbImportFromDataSource.Enabled = true;
					bindingNavigatorAddNewItem.Enabled = true;
					bindingNavigatorAttributeRaw.AddNewItem = bindingNavigatorAddNewItem;
					pasteToolStripMenuItem.Enabled = true;
					contextMenuStripRawAttribute.Items.Add( pasteToolStripMenuItem );
					if( Global.SecurityOperations.CanDeleteRawAttributeData( m_strAttribute ) )
					{
						dgvAttribute.AllowUserToDeleteRows = true;
						tsbDeleteAll.Enabled = true;
						bindingNavigatorDeleteItem.Enabled = true;
						bindingNavigatorAttributeRaw.DeleteItem = bindingNavigatorDeleteItem;
					}
				}
			}
			else
			{
				dgvAttribute.ReadOnly = true;

				dgvAttribute.AllowUserToAddRows = false;
				tsbImportFromDataSource.Enabled = false;
				bindingNavigatorAddNewItem.Enabled = false;
				bindingNavigatorAttributeRaw.AddNewItem = null;
				pasteToolStripMenuItem.Enabled = false;
				contextMenuStripRawAttribute.Items.Remove( pasteToolStripMenuItem );

				dgvAttribute.AllowUserToDeleteRows = false;
				tsbDeleteAll.Enabled = false;
				bindingNavigatorDeleteItem.Enabled = false;
				bindingNavigatorAttributeRaw.DeleteItem = null;
			}
		}

        private void FormAttributeDocument_Deactivate(object sender, EventArgs e)
        {
            if (binding != null && this.ImageView)
            {
                dataAdapter.Update((DataTable)binding.DataSource);
            }
        }

        private void dgvAttribute_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.ImageView)
            {
                int nIndex = e.RowIndex;
                if (rbLinearRef.Checked)
                {
                    String strRoute = dgvAttribute.Rows[nIndex].Cells["ROUTES"].Value.ToString();
                    String strDirection = dgvAttribute.Rows[nIndex].Cells["DIRECTION"].Value.ToString();
                    String strBeginStation = dgvAttribute.Rows[nIndex].Cells["BEGIN_STATION"].Value.ToString();
                    String strDate = dgvAttribute.Rows[nIndex].Cells["DATE"].Value.ToString();
                    DateTime dateTime;
                    String strYear = "";
                    try
                    {
                        dateTime = DateTime.Parse(strDate);
                        strYear = dateTime.Year.ToString();
                    }
                    catch
                    {
                        strYear = "";
                    }
                    NavigationEvent navigationEvent = new NavigationEvent(strRoute, strDirection, double.Parse(strBeginStation), strYear);
                    m_event.issueEvent(navigationEvent);
                }
                else
                {
                    String strRoute = dgvAttribute.Rows[nIndex].Cells["FACILITY"].Value.ToString();
                    String strDirection = dgvAttribute.Rows[nIndex].Cells["SECTION"].Value.ToString();
                    String strDate = dgvAttribute.Rows[nIndex].Cells["DATE"].Value.ToString();
                    DateTime dateTime;
                    String strYear = "";
                    try
                    {
                        dateTime = DateTime.Parse(strDate);
                        strYear = dateTime.Year.ToString();
                    }
                    catch
                    {
                        strYear = "";
                    }
                    NavigationEvent navigationEvent = new NavigationEvent(strRoute, strDirection, strYear);
                    m_event.issueEvent(navigationEvent);
                }
            }
        }

        public override void NavigationTick(NavigationObject navigationObject)
        {
            if(m_nLastRowSelected > dgvAttribute.Rows.Count || m_nLastRowSelected < 0)
            {
                UpdateSelection(navigationObject);
            }
            else
            {
                if (rbLinearRef.Checked)
                {
                    String strRoute = dgvAttribute.Rows[m_nLastRowSelected].Cells["ROUTES"].Value.ToString();
                    String strDirection = dgvAttribute.Rows[m_nLastRowSelected].Cells["DIRECTION"].Value.ToString();
                    String strBeginStation = dgvAttribute.Rows[m_nLastRowSelected].Cells["BEGIN_STATION"].Value.ToString();
                    String strEndStation = dgvAttribute.Rows[m_nLastRowSelected].Cells["END_STATION"].Value.ToString();
                    String strDate = dgvAttribute.Rows[m_nLastRowSelected].Cells["DATE_"].Value.ToString();

                    try
                    {
                        double dBegin = double.Parse(strBeginStation);
                        double dEnd = double.Parse(strEndStation);
                        DateTime dateTime = DateTime.Parse(strDate);
                        String strYear = dateTime.Year.ToString();

                        if (strRoute != navigationObject.CurrentImage.Facility || strDirection != navigationObject.CurrentImage.Direction || !(navigationObject.CurrentImage.Milepost <= dEnd && navigationObject.CurrentImage.Milepost >= dBegin) || strYear != navigationObject.CurrentImage.Year.ToString())
                        {
                            UpdateSelection(navigationObject);
                        }
                    }
                    catch
                    {
                        UpdateSelection(navigationObject);
                    }
                }
                else
                {
                    String strFacility = dgvAttribute.Rows[m_nLastRowSelected].Cells["FACILITY"].Value.ToString();
                    String strSection = dgvAttribute.Rows[m_nLastRowSelected].Cells["SECTION"].Value.ToString();
                    String strDate = dgvAttribute.Rows[m_nLastRowSelected].Cells["DATE"].Value.ToString();
                    try
                    {
                        DateTime dateTime = DateTime.Parse(strDate);
                        String strYear = dateTime.Year.ToString();

                        if (strFacility != navigationObject.Facility || strSection != navigationObject.Section || strYear != navigationObject.Year)
                        {
                            UpdateSelection(navigationObject);
                        }
                    }
                    catch
                    {
                        UpdateSelection(navigationObject);
                    }
                }
            }
        }
        
        private void UpdateSelection(NavigationObject navigationObject)  
        {
            if (navigationObject.IsLinear)
            {
                if (!rbLinearRef.Checked)
                {
                    rbLinearRef.Checked = true;
                }
                foreach (DataGridViewRow row in dgvAttribute.Rows)
                {
                    String strRoute = row.Cells["ROUTES"].Value.ToString();
                    String strDirection = row.Cells["DIRECTION"].Value.ToString();
                    String strBeginStation = row.Cells["BEGIN_STATION"].Value.ToString();
                    String strEndStation = row.Cells["END_STATION"].Value.ToString();
                    String strDate = row.Cells["DATE_"].Value.ToString();
                    try
                    {
                        double dBegin = double.Parse(strBeginStation);
                        double dEnd = double.Parse(strEndStation);
                        DateTime dateTime = DateTime.Parse(strDate);
                        String strYear = dateTime.Year.ToString();
                        if (strRoute == navigationObject.CurrentImage.Facility && strDirection == navigationObject.CurrentImage.Direction && navigationObject.CurrentImage.Milepost < dEnd && navigationObject.CurrentImage.Milepost > dBegin && strYear == navigationObject.CurrentImage.Year.ToString())
                        {
                            row.Selected = true;
                            m_nLastRowSelected = row.Index;
                            ScrollGrid();
                        }
                        else row.Selected = false;
                    }
                    catch
                    {
                        row.Selected = false;
                    }
                }
            }
        }

        private void ScrollGrid()
        {
            int halfWay = (dgvAttribute.DisplayedRowCount(false) / 2);
            if (dgvAttribute.FirstDisplayedScrollingRowIndex + halfWay > dgvAttribute.SelectedRows[0].Index ||
                (dgvAttribute.FirstDisplayedScrollingRowIndex + dgvAttribute.DisplayedRowCount(false) - halfWay) <= dgvAttribute.SelectedRows[0].Index)
            {
                int targetRow = dgvAttribute.SelectedRows[0].Index;

                targetRow = Math.Max(targetRow - halfWay, 0);
                dgvAttribute.FirstDisplayedScrollingRowIndex = targetRow;
            }
        }


    }
}
