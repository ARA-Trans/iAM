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
using System.IO;
using RoadCare3.Properties;
using DataObjects;
namespace RoadCare3
{
    public partial class FormSegmentationResult : BaseForm
    {
        private String m_strNetwork;
        private String m_strNetworkID;
        BindingSource binding;
        DataAdapter dataAdapter;
        DataTable table;
        int m_nCount = 0;
        bool m_bInitial = true;
		bool loaded = false;

        /// <summary>
        /// Allows the user to modify the results of a segmented network.
        /// </summary>
        /// <param name="strNetwork">The name of the network on which to perform segmentation modifications</param>
        public FormSegmentationResult(String strNetwork)
        {
            InitializeComponent();
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
        }

        private void FormSegmentationResult_Load(object sender, EventArgs e)
        {
			SecureForm();

			FormLoad(Settings.Default.SEGMENTATION_RESULTS_IMAGE_KEY, Settings.Default.SEGMENTATION_RESULTS_IMAGE_KEY_SELECTED);

            this.TabText = "Segment Result-" +m_strNetwork;
            labelResults.Text = "Segmentation Results: " + m_strNetwork;
            FillRoutes();
            m_bInitial = false;
            LoadGridResults();
        }

		protected void SecureForm()
		{
			checkBoxNetworkSegment.Checked = false;
			LockCheckBox( checkBoxNetworkSegment );
			//currently this checkbox does nothing so I'm making it invisible for the time being
			checkBoxNetworkSegment.Visible = false;

			LockButton( buttonJoin );
			LockButton( buttonSplit );
			LockToolStripButton( tsbtn ); //this is the apply to all button...should probably be renamed
			LockToolStripButton( tsbtnApplyToDisplay );
			LockDataGridView( dgvResults );

			if( Global.SecurityOperations.CanModifySegmentationResults( m_strNetworkID ) )
			{
				UnlockButton( buttonJoin );
				UnlockButton( buttonSplit );
				UnlockDataGridViewForModify( dgvResults );
				if( Global.SecurityOperations.CanBatchModifySegmentationResults( m_strNetworkID ) )
				{
					UnlockToolStripButton( tsbtn ); //this is the apply to all button...should probably be renamed
					UnlockToolStripButton( tsbtnApplyToDisplay );
				}
			}
		}

        public String NetworkID
        {
            get { return m_strNetworkID; }
        }
       
        private void LoadGridResults()
        {
            if (dataAdapter != null)
            {
                dataAdapter.Dispose();
            }
            String strWhere = " WHERE NETWORKID ='" + m_strNetworkID + "' ";

            String strRoute = comboRoute.Text;
            if (strRoute != "All")
            {
                strWhere = strWhere + " AND ROUTES='" + strRoute + "' ";
            }


			String strQuery = "";
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					strQuery = "SELECT DYNAMIC_SEGMENTATION_ID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE AS Reason FROM DYNAMIC_SEGMENTATION " + strWhere + " ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
					break;
				case "ORACLE":
					strQuery = "SELECT ROWID, ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE AS Reason FROM DYNAMIC_SEGMENTATION " + strWhere + " ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
					break;
				default:
					throw new NotImplementedException( "TODO: implement ANSI version of LoadGridResult()" );
					//break;
			}

            try
            {

                binding = new BindingSource();
                dataAdapter = new DataAdapter(strQuery);

                // Populate a new data table and bind it to the BindingSource.
                table = new DataTable();

                table.Locale = System.Globalization.CultureInfo.InvariantCulture;


                dataAdapter.Fill(table);
                binding.DataSource = table;
                dgvResults.DataSource = binding;
                bindingNavigatorSegmentationResult.BindingSource = binding;
				table.TableName = "DYNAMIC_SEGMENTATION";
                dgvResults.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                //dgvResults.ReadOnly = true;
                dgvResults.Columns[0].ReadOnly = true;
                dgvResults.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvResults.Columns[1].ReadOnly = true;
                dgvResults.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvResults.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvResults.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

				dgvResults.Columns["BEGIN_STATION"].DefaultCellStyle.Format = "G";
				dgvResults.Columns["END_STATION"].DefaultCellStyle.Format = "G";


				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						dgvResults.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
						dgvResults.Columns["DYNAMIC_SEGMENTATION_ID"].Visible = false;
						break;
					case "ORACLE":
						dgvResults.Columns["ROWID"].Visible = false;
						//dgvResults.Columns["BREAKCAUSE"].Name = "Reason";
						dgvResults.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
						dgvResults.Columns["Reason"].HeaderText = "Reason";
						break;
					default:
						throw new NotImplementedException( "TODO: implement ANSI version of LoadGridResult()" );
						//break;
				}


			
			}
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Connecting contruction history table. SQL message is " + exception.Message);
            }

			loaded = true;
        }

        private void FillRoutes()
        {
            comboRoute.Items.Add("All");
            DataSet ds;
            String strSelect = "SELECT DISTINCT ROUTES FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID + "' ORDER BY ROUTES";

            try
            { 
                ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboRoute.Items.Add(row[0].ToString());
                }
            }
            catch (Exception exception) { Global.WriteOutput("Error: Filling route combo. " + exception.Message); }

            m_nCount = 0;
            strSelect = "SELECT COUNT(*) FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID + "'";
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
                int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(), out m_nCount);
            }
            catch (Exception exception) { Global.WriteOutput("Error: Getting return count. " + exception.Message); }

            if (m_nCount < Global.MaximumReturn)
            {
                comboRoute.Text = "All";
            }
            else
            {
                comboRoute.Text = comboRoute.Items[1].ToString();
            }
        }

        private void comboRoute_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitial )
            {
                if (m_nCount > Global.MaximumReturn && comboRoute.Text == "All")
                {
                    if (MessageBox.Show("Selecting All from the Route Box will display more\n than " + Global.MaximumReturn.ToString() + " records, which may take time to load. Continue?", "Large Record Set Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        LoadGridResults();
                    }
                }
                else
                {
                    LoadGridResults();
                }
            }

        }

        private void buttonJoin_Click(object sender, EventArgs e)
        {
			List<int> selectedRowIndicies = new List<int>();
			foreach( DataGridViewRow selectedRow in dgvResults.SelectedRows )
			{
				selectedRowIndicies.Add( selectedRow.Index );
			}
			if (dataAdapter != null)
            {
				( ( DataTable )binding.DataSource ).Columns["Reason"].ColumnName = "BREAKCAUSE";
                dataAdapter.Update((DataTable)binding.DataSource);
				( ( DataTable )binding.DataSource ).Columns["BREAKCAUSE"].ColumnName = "Reason";
			}

			foreach( int selectedRowIndex in selectedRowIndicies )
			{
				dgvResults.Rows[selectedRowIndex].Selected = true;
			}
			this.Cursor = Cursors.WaitCursor;
            if (dgvResults.SelectedRows.Count < 2) return;
            
            String strRoute="";
            String strBegin = "";
            String strEnd = "";
            String strDirection = "";

            String strRouteLast = "";
            String strBeginLast = "";
            String strEndLast = "";
            String strDirectionLast = "";

            String strDelete;
            String strInsert;
            bool bInsert = false;
            bool bAnyInsert = false;
            foreach (DataGridViewRow row in dgvResults.Rows)
            {
                if (strRoute == "" && row.Selected == true)
                {
                    strRoute = row.Cells["ROUTES"].Value.ToString();
                    strBegin = row.Cells["BEGIN_STATION"].Value.ToString();
                    strEnd = row.Cells["END_STATION"].Value.ToString();
                    strDirection = row.Cells["DIRECTION"].Value.ToString();
                    continue;
                }

                if(row.Selected == true)
                {
                    if (row.Cells[0].Value == null)//If last (NULL) row is selected.
                    {
                        if (bInsert) // Insert current route/begin/end/direction
                        {
                            strInsert = "INSERT INTO DYNAMIC_SEGMENTATION (NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE) VALUES ('"
                                        + m_strNetworkID + "','"
                                        + strRoute + "','"
                                        + strBegin + "','"
                                        + strEnd + "','"
                                        + strDirection + "','"
                                        + "Joined')";

                            try { DBMgr.ExecuteNonQuery(strInsert); }
                            catch (Exception exception) { Global.WriteOutput("Error:Inserting joined sections failed - " + exception.Message); this.Cursor = Cursors.Default;  return; }
                            Global.WriteOutput("Joined Route: " + strRoute + " Begin:" + strBegin + " End:" + strEnd + " Direction:" + strDirection);
                        }
                        continue;

                    }
                    


                    strRouteLast = row.Cells["ROUTES"].Value.ToString();
                    strBeginLast = row.Cells["BEGIN_STATION"].Value.ToString();
                    strEndLast = row.Cells["END_STATION"].Value.ToString();
                    strDirectionLast = row.Cells["DIRECTION"].Value.ToString();

                    if (strRoute == strRouteLast || strDirection == strDirectionLast)
                    {
                        //These rows are on the same route direction.
                        // Check if begin is equal to end.
                        if (strEnd == strBeginLast)
                        {
                            //Delete both rows - this first delete will only occur first time.
                            strDelete = "DELETE FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID
                                + "' AND ROUTES='" + strRoute
                                + "' AND BEGIN_STATION='" + strBegin
                                + "' AND END_STATION='" + strEnd
                                + "' AND DIRECTION='" + strDirection + "'";

                            DBMgr.ExecuteNonQuery(strDelete);
                     
                            strDelete = "DELETE FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID
                                + "' AND ROUTES='" + strRouteLast
                                + "' AND BEGIN_STATION='" + strBeginLast
                                + "' AND END_STATION='" + strEndLast
                                + "' AND DIRECTION='" + strDirectionLast + "'";

                            DBMgr.ExecuteNonQuery(strDelete);
                            strEnd = strEndLast;
                            bInsert = true;
                            bAnyInsert = true;
                        }
                        else //End of next and Beginning of first do not match
                        {

                            if (bInsert) // Insert current route/begin/end/direction
                            {
                                strInsert = "INSERT INTO DYNAMIC_SEGMENTATION (NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE) VALUES ('"
                                            + m_strNetworkID + "','"
                                            + strRoute + "','"
                                            + strBegin + "','"
                                            + strEnd + "','"
                                            + strDirection + "','"
                                            + "Joined')";

                                try { DBMgr.ExecuteNonQuery(strInsert); }
                                catch (Exception exception) { Global.WriteOutput("Error:Inserting joined sections failed - " + exception.Message); this.Cursor = Cursors.Default; return; }
                                Global.WriteOutput("Joined Route: " + strRoute + " Begin:" + strBegin + " End:" + strEnd + " Direction:" + strDirection);
                            }
                            bInsert = false;
                            strRoute = strRouteLast;
                            strBegin = strBeginLast;
                            strEnd = strEndLast;
                            strDirection = strDirectionLast;
                        }
                    }
                    else //Route and/or direction changed.
                    {
                        if (bInsert) // Insert current route/begin/end/direction
                        {
                            strInsert = "INSERT INTO DYNAMIC_SEGMENTATION (NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE) VALUES ('"
                                        + m_strNetworkID + "','"
                                        + strRoute + "','"
                                        + strBegin + "','"
                                        + strEnd + "','"
                                        + strDirection + "','"
                                        + "Joined')";

                            try { DBMgr.ExecuteNonQuery(strInsert); }
                            catch (Exception exception) { Global.WriteOutput("Error:Inserting joined sections failed - " + exception.Message); this.Cursor = Cursors.Default; return; }
                            Global.WriteOutput("Joined Route: " + strRoute + " Begin:" + strBegin + " End:" + strEnd + " Direction:" + strDirection);
                        }
                        bInsert = false;
                        strRoute = strRouteLast;
                        strBegin = strBeginLast;
                        strEnd = strEndLast;
                        strDirection = strDirectionLast;
                     }
                }
                else// Row not selected.
                {
                    if (bInsert) // Insert current route/begin/end/direction
                    {
                        strInsert = "INSERT INTO DYNAMIC_SEGMENTATION (NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE) VALUES ('"
                                    + m_strNetworkID + "','"
                                    + strRoute + "','"
                                    + strBegin + "','"
                                    + strEnd + "','"
                                    + strDirection + "','"
                                    + "Joined')";

                        try { DBMgr.ExecuteNonQuery(strInsert); }
                        catch (Exception exception) { Global.WriteOutput("Error:Inserting joined sections failed - " + exception.Message); this.Cursor = Cursors.Default; return; }
                        Global.WriteOutput("Joined Route: " + strRoute + " Begin:" + strBegin + " End:" + strEnd + " Direction:" + strDirection);
                    }
                    bInsert = false;
                    strRoute = "";
                    strBegin = "";
                    strEnd = "";
                    strDirection = "";
                }
            }
            if (bAnyInsert) LoadGridResults();
            this.Cursor = Cursors.Default;
        }

        private void buttonSplit_Click(object sender, EventArgs e)
        {
            if (dataAdapter != null)
            {
                dataAdapter.Update((DataTable)binding.DataSource);
            }
            this.Cursor = Cursors.WaitCursor;
            String strSplit = textBoxSplit.Text;
            int nSplit = 0;
            //Get the split number.
            try
            {
                nSplit = int.Parse(strSplit);

            }
            catch
            {
                Global.WriteOutput("Error: SPLIT INTO number SECTIONS must be a positive integer.");
                this.Cursor = Cursors.Default;
                return;
            }

            if (nSplit <= 1)
            {
                Global.WriteOutput("Error: SPLIT INTO number SECTIONS must be a positive integer.");
                this.Cursor = Cursors.Default;
                return;
            }

            
            String strRoute = "";
            String strBegin = "";
            String strEnd = "";
            String strDirection = "";
            //For each selected cell.
            String strDelete;
            String strInsert;
            

            foreach (DataGridViewRow row in dgvResults.SelectedRows)
            {
                String strBeginNew = "";
                String strEndNew = "";
                

                strRoute = row.Cells["ROUTES"].Value.ToString();
                strBegin = row.Cells["BEGIN_STATION"].Value.ToString();
                strEnd = row.Cells["END_STATION"].Value.ToString();
                strDirection = row.Cells["DIRECTION"].Value.ToString();

                //Delete current row.
                strDelete = "DELETE FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID
                    + "' AND ROUTES='" + strRoute
                    + "' AND BEGIN_STATION='" + strBegin
                    + "' AND END_STATION='" + strEnd
                    + "' AND DIRECTION='" + strDirection + "'";
                DBMgr.ExecuteNonQuery(strDelete);

                


                strBeginNew = strBegin;// New section will start with the same begin.
                float fBegin = float.Parse(strBegin);
                float fEnd = float.Parse(strEnd);
                float fDelta = (fEnd-fBegin)/(float)nSplit;

                //Delete this row.  Insert 
                for (int i = 1; i < nSplit; i++)
                {
                    fBegin += fDelta;
                    strEndNew = fBegin.ToString("G");
                    // INSERT new NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION
                    strInsert = "INSERT INTO DYNAMIC_SEGMENTATION (NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE) VALUES ('"
                                + m_strNetworkID + "','"
                                + strRoute + "','"
                                + strBegin + "','"
                                + strEndNew + "','"
                                + strDirection + "','"
                                + "Split')";

                    try { DBMgr.ExecuteNonQuery(strInsert); }
                    catch (Exception exception) { Global.WriteOutput("Error:Inserting split sections failed - " + exception.Message); this.Cursor = Cursors.Default; return; }

                    strBegin = strEndNew;
                    fBegin = float.Parse(strBegin);

                }

                //Insert the last of the split using the original strEnd


                strInsert = "INSERT INTO DYNAMIC_SEGMENTATION (NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE) VALUES ('"
                            + m_strNetworkID + "','"
                            + strRoute + "','"
                            + strBegin + "','"
                            + strEnd + "','"
                            + strDirection + "','"
                            + "Split')";

                try { DBMgr.ExecuteNonQuery(strInsert); }
                catch (Exception exception) { Global.WriteOutput("Error:Inserting split sections failed - " + exception.Message); this.Cursor = Cursors.Default; return; }
            }
            LoadGridResults();
            this.Cursor = Cursors.Default;

        }
        /// <summary>
        /// Reads through all displayed row. If the length of the segment is greater than
        /// the maximum.  The section will be split.  If EVEN is chosen, it is a simple split (like the split button)
        /// If EXACT is chosen, it will be split into MAXIMUM increments with a remainder.  The remainder will be added if it is less than minimum.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void ApplyToDisplay()
		{

			String strMinimum = tstbMinimum.Text;
			String strMaximum = tstbMaximum.Text;
			float fMinimum = 0;
			float fMaximum = 0;

			try
			{
				fMinimum = float.Parse( strMinimum );
				fMaximum = float.Parse( strMaximum );
			}
			catch
			{
				Global.WriteOutput( "Error: Minimum and maximum values must be positive numbers." );
				Cursor = Cursors.Default;

				return;
			}

			if( fMinimum <= 0 || fMaximum < 0 )
			{
				Global.WriteOutput( "Error: Minimum and maximum values must be positive numbers." );
				return;
			}

			if( fMinimum >= fMaximum )
			{
				Global.WriteOutput( "Error: The minimum value must be less than the maximum value." );
				return;
			}

			String strRoute;
			String strBegin;
			String strEnd;
			String strDirection;
			float fEnd = 0;
			float fBegin = 0;

			foreach( DataGridViewRow row in dgvResults.Rows )
			{
				if( row.Cells[0].Value != null )
				{
					strBegin = row.Cells["BEGIN_STATION"].Value.ToString();
					strEnd = row.Cells["END_STATION"].Value.ToString();
					try
					{
						fBegin = float.Parse( strBegin );
						fEnd = float.Parse( strEnd );
					}
					catch
					{
						Global.WriteOutput( "Error: Could not parse BEGIN_STATION and/or END_STATION for Row =" + row.Index.ToString() );
						return;

					}
					if( fEnd - fBegin <= 0 )
					{
						Global.WriteOutput( "Error: STATION_BEGIN must be less than STATION_END" );
						return;
					}
				}
			}


			String strDelete;
			String strWhere;

			// Delete all ROUTES on screen.
			strRoute = comboRoute.Text;
			if( strRoute == "All" )
			{
				strWhere = " WHERE NETWORKID='" + m_strNetworkID + "'";
			}
			else
			{
				strWhere = " WHERE NETWORKID='" + m_strNetworkID + "' AND ROUTES = '" + strRoute + "'";
			}

			strDelete = "DELETE FROM DYNAMIC_SEGMENTATION" + strWhere;
			DBMgr.ExecuteNonQuery( strDelete );

			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\UpdateDyanamic.txt";
			TextWriter tw = new StreamWriter( strOutFile );
			String strOut;
			String strReason;
			float fDelta = 0;
			String strEndNew;

			if( tscbIncrements.Text == "Minimum Only" )
			{
				List<LRSObjectSegmentationResult> listResults = new List<LRSObjectSegmentationResult>();
				foreach( DataGridViewRow row in dgvResults.Rows )
				{
					try
					{
						if( row.Cells[0].Value != null )
						{
							listResults.Add( new LRSObjectSegmentationResult( row ) );
						}
					}
					catch( Exception except )
					{
						Global.WriteOutput( "___ERROR: ApplyToDisplay()___" );
						Global.WriteOutput( "Attempted to create LRSObjectSegmentationResult object" );
						Global.WriteOutput( "row.Cells[\"BEGIN_STATION\"].Value.ToString() = " + row.Cells["BEGIN_STATION"].Value.ToString() );
						Global.WriteOutput( "row.Cells[\"END_STATION\"].Value.ToString( = " + row.Cells["END_STATION"].Value.ToString() );
						Global.WriteOutput( "row.Cells[\"DIRECTION\"].Value.ToString()) = " + row.Cells["DIRECTION"].Value.ToString() );
						Global.WriteOutput( "row.Cells[\"REASON\"].Value.ToString() = " + row.Cells["REASON"].Value.ToString() );
						Global.WriteOutput( "except.Message = " + except.Message );

						//Global.WriteOutput( "Error: Parsing segmentation results.  Are BEGIN and END Station numbers?" + except.Message );
					}
				}

				for( int i = 0; i < listResults.Count; i++ )
				{
					LRSObjectSegmentationResult result = listResults[i];
					if( i != listResults.Count - 1 )
					{
						LRSObjectSegmentationResult resultNext = listResults[i + 1];
						if( result.Delta < ( double )fMinimum && result.EndStation == resultNext.BeginStation && result.Direction == resultNext.Direction )
						{
							resultNext.BeginStation = result.BeginStation;
							resultNext.Reason += " \t Minimum Join";
							result.Delete = true;
						}
						if( result.Direction != resultNext.Direction )//Last in particular direction
						{

							LRSObjectSegmentationResult resultPrevious = listResults[i - 1];

							if( !resultPrevious.Delete && result.Delta < ( double )fMinimum && result.BeginStation == resultPrevious.EndStation && result.Direction == resultPrevious.Direction )
							{
								resultPrevious.EndStation = result.EndStation;
								resultPrevious.Reason += " \t Minimum Join";
								result.Delete = true;
							}

						}
					}
					else //Last object
					{
						if( listResults.Count > 1 )
						{
							LRSObjectSegmentationResult resultPrevious = listResults[i - 1];

							if( !resultPrevious.Delete && result.Delta < ( double )fMinimum && result.BeginStation == resultPrevious.EndStation && result.Direction == resultPrevious.Direction )
							{
								resultPrevious.EndStation = result.EndStation;
								resultPrevious.Reason += " \t Minimum Join";
								result.Delete = true;
							}
						}
					}
				}
				listResults.RemoveAll( delegate( LRSObjectSegmentationResult lrs )
				{
					return lrs.Delete;
				} );

				foreach( LRSObjectSegmentationResult result in listResults )
				{
					//This is why you don't change the semantics of a variable.
					//strRoute is "All" if it's set to all and your click Minimum Only and you 
					//click Apply to Display
					//wrong wrong wrong.
					//strOut = m_strNetworkID + "," + strRoute + "," + result.BeginStation.ToString() + "," + result.EndStation.ToString() + "," + result.Direction + "," + result.Reason;
					strOut = m_strNetworkID + "\t" + result.Route + "\t" + result.BeginStation.ToString() + "\t" + result.EndStation.ToString() + "\t" + result.Direction + "\t" + result.Reason;
					tw.WriteLine( strOut );
				}

			}
			else
			{
				foreach( DataGridViewRow row in dgvResults.Rows )
				{
					if( row.Cells[0].Value == null )
						continue;
					strRoute = row.Cells["ROUTES"].Value.ToString();
					strBegin = row.Cells["BEGIN_STATION"].Value.ToString();
					strEnd = row.Cells["END_STATION"].Value.ToString();
					strDirection = row.Cells["DIRECTION"].Value.ToString();
					strReason = row.Cells["Reason"].Value.ToString();
					fBegin = float.Parse( strBegin );
					fEnd = float.Parse( strEnd );

					fDelta = fEnd - fBegin;

					if( tscbIncrements.Text == "Even" )
					{
						int nSplit = ( int )( fDelta / fMaximum );
						nSplit++;

						if( nSplit > 1 )
						{
							//Split into event number of rows and insert.
							fDelta = fDelta / ( float )nSplit;
							//Delete this row.  Insert 
							for( int i = 1; i < nSplit; i++ )
							{
								fBegin += fDelta;
								strEndNew = fBegin.ToString("G");
								// INSERT new NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION
								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEndNew + "\t" + strDirection + "\t Maximum(even) \t";
								tw.WriteLine( strOut );
								strBegin = strEndNew;
								fBegin = float.Parse( strBegin );
							}
							strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + " \t Maximum(even) \t";
							tw.WriteLine( strOut );

						}

						else //Just insert the row
						{
							strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t" + strReason + "\t";
							tw.WriteLine( strOut );

						}

					}
					else if( tscbIncrements.Text == "Exact" )
					{
						if( fMaximum > fDelta )//Just write the row
						{
							strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t" + strReason + "\t";
							tw.WriteLine( strOut );
						}
						else
						{
							int nSplit = ( int )( fDelta / fMaximum );

							for( int i = 1; i < nSplit; i++ )
							{
								fBegin += fMaximum;
								fDelta -= fMaximum;
								strEndNew = fBegin.ToString("G");
								// INSERT new NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION
								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEndNew + "\t" + strDirection + " \t Maximum(exact) \t";
								tw.WriteLine( strOut );
								strBegin = strEndNew;
								fBegin = float.Parse( strBegin );
							}

							//At this point we need to determin if one or two rows will be added
							//If maximum+minimum > delta add one.

							if( ( fMaximum + fMinimum ) > fDelta )
							{
								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + " \t Below minimum (exact) \t";
								tw.WriteLine( strOut );
							}
							else
							{
								fBegin += fMaximum;
								strEndNew = fBegin.ToString("G");

								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEndNew + "\t" + strDirection + " \t Maximum (exact) \t";
								tw.WriteLine( strOut );
								strBegin = strEndNew;

								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + " \t Remainder (exact) \t";
								tw.WriteLine( strOut );
							}
						}
					}
				}

			}


			tw.Close();
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					DBMgr.SQLBulkLoad( "DYNAMIC_SEGMENTATION", strOutFile, "\\t" );
					break;
				case "ORACLE":
					List<string> columnNames = new List<string>();
					columnNames.Add( "NETWORKID" );
					columnNames.Add( "ROUTES" );
					columnNames.Add( "BEGIN_STATION" );
					columnNames.Add( "END_STATION" );
					columnNames.Add( "DIRECTION" );
					columnNames.Add( "BREAKCAUSE" );
					DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "DYNAMIC_SEGMENTATION", strOutFile, columnNames, "\\t" );
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				//break;
			}
			LoadGridResults();
		}


        /// <summary>
        /// Similar to Increment Display except data comes from Datareader.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void ApplyToAll()
		{
			//bool bEven = true; //bEven's value is never read from
			//if (tscbIncrements.Text != "Even") bEven = false;

			String strMinimum = tstbMinimum.Text;
			String strMaximum = tstbMaximum.Text;
			float fMinimum = 0;
			float fMaximum = 0;

			try
			{
				fMinimum = float.Parse( strMinimum );
				fMaximum = float.Parse( strMaximum );

			}
			catch
			{
				Global.WriteOutput( "Error: Minimum and maximum values must be positive numbers." );
				this.Cursor = Cursors.Default;

				return;
			}

			if( fMinimum <= 0 || fMaximum < 0 )
			{
				Global.WriteOutput( "Error: Minimum and maximum values must be positive numbers." );
				return;
			}

			if( fMinimum >= fMaximum )
			{
				Global.WriteOutput( "Error: The minimum value must be less than the maximum value." );
				return;
			}

			String strRoute;
			String strBegin;
			String strEnd;
			String strDirection;
			float fEnd = 0;
			float fBegin = 0;

			foreach( DataGridViewRow row in dgvResults.Rows )
			{
				if( row.Cells[0].Value == null )
					continue;
				strBegin = row.Cells["BEGIN_STATION"].Value.ToString();
				strEnd = row.Cells["END_STATION"].Value.ToString();
				try
				{
					if( float.TryParse( strBegin, out fBegin ) )
					{
						if( !float.TryParse( strEnd, out fEnd ) )
						{
							throw new Exception( "Error: could not parse END_STATION: [" + strEnd + "]" );
						}
					}
					else
					{
						throw new Exception( "Error: could not parse BEGIN_STATION: [" + strBegin + "]" );
					}
					//fBegin = float.Parse( strBegin );
					//fEnd = float.Parse( strEnd );
				}
				catch( Exception ex )
				{
					Global.WriteOutput( ex.Message );
					return;
				}
				if( fEnd - fBegin <= 0 )
				{
					Global.WriteOutput( "Error: STATION_BEGIN must be less than STATION_END" );
					return;
				}
			}


			String strDelete;
			String strWhere;
			String strSelect = "SELECT ROUTES,BEGIN_STATION,END_STATION,DIRECTION,BREAKCAUSE FROM DYNAMIC_SEGMENTATION WHERE NETWORKID='" + m_strNetworkID + "' ORDER BY ROUTES,DIRECTION,BEGIN_STATION";
			//SqlDataReader dr = DBMgr.CreateDataReader(strSelect);
			DataReader dr = new DataReader( strSelect );

			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\UpdateDyanamic.txt";
			TextWriter tw = new StreamWriter( strOutFile );
			String strOut;
			String strReason;
			float fDelta = 0;
			String strEndNew;


			if( tscbIncrements.Text == "Minimum Only" )
			{
				List<LRSObjectSegmentationResult> listResults = new List<LRSObjectSegmentationResult>();
				while( dr.Read() )
				{
					try
					{
						listResults.Add( new LRSObjectSegmentationResult( dr ) );

					}
					catch( Exception except )
					{
						Global.WriteOutput( "___ERROR: ApplyToAll()___" );
						Global.WriteOutput( "Attempted to create LRSObjectSegmentationResult object" );
						Global.WriteOutput( "dr[\"BEGIN_STATION\"].ToString() = " + dr["BEGIN_STATION"].ToString() );
						Global.WriteOutput( "dr[\"END_STATION\"].ToString() = " + dr["END_STATION"].ToString() );
						Global.WriteOutput( "dr[\"DIRECTION\"].ToString() = " + dr["DIRECTION"].ToString() );
						Global.WriteOutput( "dr[\"REASON\"].ToString() = " + dr["REASON"].ToString() );
						Global.WriteOutput( "except.Message = " + except.Message );
						//Global.WriteOutput( "Error: Parsing segmentation results.  Are BEGIN and END Station numbers?" + except.Message );
					}
				}

				for( int i = 0; i < listResults.Count; i++ )
				{
					LRSObjectSegmentationResult result = listResults[i];
					if( i != listResults.Count - 1 )
					{
                        try
                        {
                            LRSObjectSegmentationResult resultNext = listResults[i + 1];
                            if (result.Delta < (double)fMinimum && result.EndStation == resultNext.BeginStation && result.Direction == resultNext.Direction && result.Route == resultNext.Route)
                            {
                                resultNext.BeginStation = result.BeginStation;
                                resultNext.Reason += " \t Minimum Join";
                                result.Delete = true;
                            }
                            if (result.Direction != resultNext.Direction)//Last in particular direction
                            {
                                if (i > 0)
                                {
                                    LRSObjectSegmentationResult resultPrevious = listResults[i - 1];
                                    if (!resultPrevious.Delete && result.Delta < (double)fMinimum && result.BeginStation == resultPrevious.EndStation && result.Direction == resultPrevious.Direction && result.Route == resultPrevious.Route)
                                    {
                                        resultPrevious.EndStation = result.EndStation;
                                        resultPrevious.Reason += " \t Minimum Join";
                                        result.Delete = true;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            Global.WriteOutput("Error on " + result.Route + " " + result.Direction + " " + result.BeginStation.ToString() + " - " + result.EndStation.ToString());
                        }
					}
					else //Last object
					{
                        try
                        {
                            if (listResults.Count > 1)
                            {
                                LRSObjectSegmentationResult resultPrevious = listResults[i - 1];

                                if (!resultPrevious.Delete && result.Delta < (double)fMinimum && result.BeginStation == resultPrevious.EndStation && result.Direction == resultPrevious.Direction && result.Route == resultPrevious.Route)
                                {
                                    resultPrevious.EndStation = result.EndStation;
                                    resultPrevious.Reason += " \t Minimum Join";
                                    result.Delete = true;
                                }
                            }
                        }
                        catch
                        {
                            Global.WriteOutput("Error on " + result.Route + " " + result.Direction + " " + result.BeginStation.ToString() + " - " + result.EndStation.ToString());
                        }
					}
				}
				listResults.RemoveAll( delegate( LRSObjectSegmentationResult lrs )
				{
					return lrs.Delete;
				} );

				foreach( LRSObjectSegmentationResult result in listResults )
				{
					strOut = m_strNetworkID + "\t" + result.Route + "\t" + result.BeginStation.ToString() + "\t" + result.EndStation.ToString() + "\t" + result.Direction + "\t" + result.Reason + "\t";
					tw.WriteLine( strOut );
				}

			}
			else
			{

				while( dr.Read() )
				{
					strRoute = dr["ROUTES"].ToString();
					strBegin = dr["BEGIN_STATION"].ToString();
					strEnd = dr["END_STATION"].ToString();
					strDirection = dr["DIRECTION"].ToString();
					strReason = dr["BREAKCAUSE"].ToString();
					float.TryParse( strBegin, out fBegin );
					float.TryParse( strEnd, out fEnd );

					fDelta = fEnd - fBegin;

					if( tscbIncrements.Text == "Even" )
					{
						int nSplit = ( int )( fDelta / fMaximum );
						nSplit++;

						if( nSplit > 1 )
						{
							//Split into event number of rows and insert.
							fDelta = fDelta / ( float )nSplit;
							//Delete this row.  Insert 
							for( int i = 1; i < nSplit; i++ )
							{
								fBegin += fDelta;
								strEndNew = fBegin.ToString( "G" );
								// INSERT new NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION
								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEndNew + "\t" + strDirection + "\t Maximum(even) \t";
								tw.WriteLine( strOut );
								strBegin = strEndNew;
								fBegin = float.Parse( strBegin );
							}
							strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t Maximum(even) \t";
							tw.WriteLine( strOut );

						}

						else //Just insert the row
						{
							strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t" + strReason + "\t";
							tw.WriteLine( strOut );

						}

					}
					else if( tscbIncrements.Text == "Exact" )//Exact split.
					{
						if( fMaximum > fDelta )//Just write the row
						{
							strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t" + strReason + "\t";
							tw.WriteLine( strOut );
						}
						else
						{
							int nSplit = ( int )( fDelta / fMaximum );

							for( int i = 1; i < nSplit; i++ )
							{
								fBegin += fMaximum;
								fDelta -= fMaximum;
								strEndNew = fBegin.ToString( "G" );
								// INSERT new NETWORKID,ROUTES,BEGIN_STATION,END_STATION,DIRECTION
								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEndNew + "\t" + strDirection + "\t Maximum(exact) \t";
								tw.WriteLine( strOut );
								strBegin = strEndNew;
								fBegin = float.Parse( strBegin );
							}

							//At this point we need to determin if one or two rows will be added
							//If maximum+minimum > delta add one.

							if( ( fMaximum + fMinimum ) > fDelta )
							{
								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t Below minimum (exact) \t";
								tw.WriteLine( strOut );
							}
							else
							{
								fBegin += fMaximum;
								strEndNew = fBegin.ToString( "G" );

								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEndNew + "\t" + strDirection + "\t Maximum (exact) \t";
								tw.WriteLine( strOut );
								strBegin = strEndNew;

								strOut = m_strNetworkID + "\t" + strRoute + "\t" + strBegin + "\t" + strEnd + "\t" + strDirection + "\t Remainder (exact) \t";
								tw.WriteLine( strOut );
							}
						}
					}
				}
			}
			tw.Close();
			dr.Close();
			// Delete all ROUTES for this NetworkID.
			//This is totally wrong.
			//Apply to all is supposed to apply to the entire network.
			//we've already created rows for the entire network
			//by only deleting the rows for the route we're looking at,
			//we end up creating duplicate rows for every row that didn't change
			//on the route we don't have selected
			//Copy and Paste error?
			//strRoute = comboRoute.Text;
			//if( strRoute == "All" )
			//{
			strWhere = " WHERE NETWORKID='" + m_strNetworkID + "'";
			//}
			//else
			//{
			//    strWhere = " WHERE NETWORKID='" + m_strNetworkID + "' AND ROUTES = '" + strRoute + "'";
			//}

			strDelete = "DELETE FROM DYNAMIC_SEGMENTATION" + strWhere;
			DBMgr.ExecuteNonQuery( strDelete );


			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					DBMgr.SQLBulkLoad( "DYNAMIC_SEGMENTATION", strOutFile, "\\t" );
					break;
				case "ORACLE":
					List<string> columnNames = new List<string>();
					columnNames.Add( "NETWORKID" );
					columnNames.Add( "ROUTES" );
					columnNames.Add( "BEGIN_STATION" );
					columnNames.Add( "END_STATION" );
					columnNames.Add( "DIRECTION" );
					columnNames.Add( "BREAKCAUSE" );
					DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "DYNAMIC_SEGMENTATION", strOutFile, columnNames, "\\t" );
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				//break;
			}
			LoadGridResults();

		}

		//private void FormSegmentationResult_FormClosed(object sender, FormClosedEventArgs e)
		//{

		//}

        private void tsbtnApplyToDisplay_Click(object sender, EventArgs e)
        {
            if (dataAdapter != null)
            {
                dataAdapter.Update((DataTable)binding.DataSource);
            }
            ApplyToDisplay();
        }

        private void tsbtn_Click(object sender, EventArgs e)
        {
            if (dataAdapter != null)
            {
                dataAdapter.Update((DataTable)binding.DataSource);
            }
            ApplyToAll();
        }

		private void dgvResults_CellValueChanged( object sender, DataGridViewCellEventArgs e )
		{
			if( dataAdapter != null && loaded )
			{
				btnCommit.Enabled = true;
			}
		}

		private void btnCommit_Click( object sender, EventArgs e )
		{
			if( dataAdapter != null )
			{
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						break;
					case "ORACLE":
						( ( DataTable )binding.DataSource ).Columns["Reason"].ColumnName = "BREAKCAUSE";
						break;
					default:
						throw new NotImplementedException( "TODO: implement ANSI verstion of btnCommit_Click()" );
				}
				dataAdapter.Update( ( DataTable )binding.DataSource );
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						break;
					case "ORACLE":
						( ( DataTable )binding.DataSource ).Columns["BREAKCAUSE"].ColumnName = "Reason";
						dgvResults.Columns[0].ReadOnly = true;
						dgvResults.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
						dgvResults.Columns[1].ReadOnly = true;
						dgvResults.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
						dgvResults.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
						dgvResults.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

						dgvResults.Columns["BEGIN_STATION"].DefaultCellStyle.Format = "G";
						dgvResults.Columns["END_STATION"].DefaultCellStyle.Format = "G";


						dgvResults.Columns["ROWID"].Visible = false;
						//dgvResults.Columns["BREAKCAUSE"].Name = "Reason";
						dgvResults.Columns["Reason"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
						dgvResults.Columns["Reason"].HeaderText = "Reason";
						break;
					default:
						throw new NotImplementedException( "TODO: implement ANSI verstion of btnCommit_Click()" );
				}
			}
			btnCommit.Enabled = false;
		}

		private void FormSegmentationResult_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( btnCommit.Enabled )
			{
				DialogResult verifyResult = MessageBox.Show( this, "Unsaved changes will not be kept.  Are you sure you want to close?", "Please verify", MessageBoxButtons.OKCancel );
				if( verifyResult != DialogResult.OK )
				{
					e.Cancel = true;
				}
			}
		}

		private void FormSegmentationResult_FormClosed( object sender, FormClosedEventArgs e )
		{
			FormUnload();
			FormManager.RemoveFormSegmentationResult( this );
		}

    }
}
