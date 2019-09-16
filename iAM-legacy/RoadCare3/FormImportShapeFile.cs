using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpMap.Data.Providers;
using SharpMap.Data;
using PropertyGridEx;
using System.IO;
using System.Data.OleDb;
using DatabaseManager;
using Microsoft.SqlServer.Management.Smo;
using System.Collections;
using SharpMap.Geometries;

namespace RoadCare3
{
    public partial class FormImportShapeFile : Form
    {
        private String m_strShapeFilePath;
        private String m_connectionString;

        private List<NetworkDefinitionData> m_listNDD = new List<NetworkDefinitionData>();
        private List<NetworkDefinitionData> m_listSFD = new List<NetworkDefinitionData>();
        private Hashtable m_htNDD = new Hashtable();
        private Hashtable m_htSFD = new Hashtable();
		private bool m_bIsLinear;


        public FormImportShapeFile(bool bIsLinear)
        {
            InitializeComponent();
			m_bIsLinear = bIsLinear;
        }

        private void FormImportShapeFile_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveFormImportShapeFile(this);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = "ESRI Shapefiles (*.shp)|*.shp";
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                String strFilePath = dlgOpen.FileName;
                int iParseFileName = dlgOpen.FileName.LastIndexOf('\\');
                String strFileName = dlgOpen.FileName.Substring(iParseFileName + 1);
                m_strShapeFilePath = strFilePath;
                strFilePath = strFilePath.Substring(0, iParseFileName + 1);
                tbShapeFileLocation.Text = m_strShapeFilePath;

                // Set the DBF database connection string
                m_connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + ";Extended Properties=dBASE IV;User ID=Admin;Password=;";

                // Open the shapefile at the given path
                ShapeFile shapefile = null;
                try
                {
                    shapefile = new SharpMap.Data.Providers.ShapeFile(m_strShapeFilePath);
                    shapefile.Open();
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: Couldn't open shapefile." + exc.Message);
                    return;
                }
                FeatureDataRow fdr = shapefile.GetFeature(0);
                lbColumns.Items.Add("GEOMETRY");
                for (int i = 0; i < fdr.Table.Columns.Count; i++)
                {
                    lbColumns.Items.Add(fdr.Table.Columns[i].ColumnName);
                }
                shapefile.Close();

            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
			// Handles the case of SRS vs. LRS imports.
			if (!m_bIsLinear)
			{
				CreateTempShapeFileSRS();
			}
			else
			{
				CreateTempShapeFileLRS();
			}
        }

		private void CreateTempShapeFileSRS()
        {
            Cursor.Current = Cursors.WaitCursor;

            lblProgress.Text = "Creating TEMP_SHAPEFILE table...";
            this.Refresh();

            Global.CreateTempShapeFile(m_strShapeFilePath);

            lblProgress.Text = "Reading from NETWORK_DEFINITION table...";
            this.Refresh();

            // Now, from NETWORK_DEFINITION get all rows where FACILITY IS NOT NULL and GEOMETRY IS NULL.
            // this should pull from the NETWORK_DEFINITION table all rows that are non linear based, and
            // are missing a geometry.
            NetworkDefinitionData nddTemp = null;
            String strKey;
            String strQuery = "SELECT FACILITY, SECTION, AREA, GEOMETRY, Envelope_MinX, Envelope_MinY, Envelope_MaxX, Envelope_MaxY FROM NETWORK_DEFINITION WHERE FACILITY IS NOT NULL AND (GEOMETRY IS NULL OR AREA IS NULL)";
            DataReader drNetworkDefinition = new DataReader(strQuery);
            while (drNetworkDefinition.Read())
            {
                nddTemp = new NetworkDefinitionData();
                nddTemp.Facility = drNetworkDefinition["FACILITY"].ToString();
                nddTemp.Section = drNetworkDefinition["SECTION"].ToString();
                nddTemp.Area = drNetworkDefinition["AREA"].ToString();
                nddTemp.Geometry = drNetworkDefinition["GEOMETRY"].ToString();

                m_listNDD.Add(nddTemp);

                strKey = nddTemp.Facility + '\t' + nddTemp.Section;
                m_htNDD.Add(strKey, nddTemp); 
            }
            drNetworkDefinition.Close();

            lblProgress.Text = "Populating geometries and envelopes...";
            this.Refresh();

            // Now loop through the Shapefile result set, and see if a FACILITY|SECTION match occurs in the m_htNDD
            // hashtable.  If a match occurs, grab the object out of the hash and populate its .Geometry field (as
            // it should be empty).  Then move on to the next row.
            try
            {
                //DataReader drTempShapeFile = new DataReader(tbSelect.Text);
                DataSet ds = DBMgr.ExecuteQuery(tbSelect.Text);
                nddTemp = null;
                bool bIncludesArea = false;
                if (ds.Tables[0].Columns.Contains("AREA"))
                {
                    bIncludesArea = true;
                }
                foreach (DataRow drTempShapeFile in ds.Tables[0].Rows)
                {
                    // Extract and create FACILITY|SECTION field for lookup in the m_htNDD.
					strKey = drTempShapeFile["FACILITY"].ToString() + '\t' + drTempShapeFile["SECTION"].ToString();
                    if (m_htNDD.Contains(strKey))
                    {
                        nddTemp = (NetworkDefinitionData)m_htNDD[strKey];
                        if (bIncludesArea)
                        {
                            if (nddTemp.Area == "")
                            {
                                nddTemp.Area = drTempShapeFile["AREA"].ToString();
                            }
                        }
                        if (nddTemp.Geometry == "")
                        {
                            nddTemp.Geometry = drTempShapeFile["GEOMETRY"].ToString();
							if (nddTemp.Geometry != "")
							{
								nddTemp.Geo = Geometry.GeomFromText(drTempShapeFile["GEOMETRY"].ToString());
							}
                        }
						if (nddTemp.Geometry != "")
						{
							nddTemp.CreateEnvelope();
						}
                    }
                    else
                    {
                        // Put into seperate list, to view in DGV later
                        nddTemp = new NetworkDefinitionData();
                        nddTemp.Facility = drTempShapeFile["FACILITY"].ToString();
                        nddTemp.Section = drTempShapeFile["SECTION"].ToString();
                        if (bIncludesArea)
                        {
                            nddTemp.Area = drTempShapeFile["AREA"].ToString();
                        }
                        if (nddTemp.Facility.Trim() != "" && nddTemp.Section.Trim() != "")
                        {
                            nddTemp.Geometry = drTempShapeFile["GEOMETRY"].ToString();
							try
							{
								if (nddTemp.Geometry != "")
								{
									nddTemp.Geo = Geometry.GeomFromText(drTempShapeFile["GEOMETRY"].ToString());
									nddTemp.CreateEnvelope();
								}
							}
							catch//(Exception exc)
							{

							}
                            if (!m_htSFD.Contains(strKey))
                            {
                                m_listSFD.Add(nddTemp);
                                m_htSFD.Add(strKey, nddTemp);    // This hash contains every ndd in the shapefile.
                            }
                            else
                            {
                                Global.WriteOutput("Warning: " + strKey + " - Duplicate Facility/Section combination detected. Ignoring duplicate Facility/Section");
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not read from TEMP_SHAPEFILE. " + exc.Message);
                return;
            }

            // Run a query for NDDs that have a valid FACILITY SECTION and GEOMETRY the results of this query are
            // compared to the m_htSFD and if a match occurs, then we remove them from the m_htSFD hash. This prevents
            // duplicate sections in the NETWORK_DEFINITION table from being entered via the shapefile.
            strQuery = "SELECT FACILITY, SECTION FROM NETWORK_DEFINITION WHERE FACILITY IS NOT NULL AND GEOMETRY IS NOT NULL";
            try
            {
                DataReader dr = new DataReader(strQuery);
                while (dr.Read())
                {
					strKey = dr["FACILITY"].ToString() + '\t' + dr["SECTION"].ToString();
                    if (m_htSFD.Contains(strKey))
                    {
                        m_listSFD.Remove((NetworkDefinitionData)m_htSFD[strKey]);
                        m_htSFD.Remove(strKey);
                    }
                }
                dr.Close();
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Problem resolving duplicate sections." + exc.Message);
                return;
            }

            lblProgress.Text = "Outputting new data to file...";
            this.Refresh();

            // Write each FACILITY SECTION GEOMETRY out to a file, then SQLBulkLoad into the NETWORK_DEFINITION table.
            TextWriter tw = null;
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\paste.txt";
            try
            {
                tw = new StreamWriter(strOutFile);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
                return;
            }

            for (int i = 0; i < m_listNDD.Count; i++)
            {
				nddTemp = (NetworkDefinitionData)m_htNDD[m_listNDD[i].Facility + '\t' + m_listNDD[i].Section];
                tw.Write(System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + System.DBNull.Value.ToString() + '\t' + nddTemp.Facility + '\t' + nddTemp.Section + '\t' + nddTemp.Area + '\t' + nddTemp.Geometry + '\t' + nddTemp.EnvelopeMinX + '\t' + nddTemp.EnvelopeMinY + '\t' + nddTemp.EnvelopeMaxX + '\t' + nddTemp.EnvelopeMaxY);
                tw.Write("\r\n");
            }
            tw.Close();

            // Delete the old data from the NETWORK_DEFINITION table where FACILITY IS NOT NULL AND GEOMETRY IS NULL
            try
            {
                String strSqlCmd = "DELETE FROM NETWORK_DEFINITION WHERE FACILITY IS NOT NULL AND (GEOMETRY IS NULL OR AREA IS NULL)";
                DBMgr.ExecuteNonQuery(strSqlCmd);
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
                return;
            }

            // Perform the BULK INSERT into the NETWORK_DEFINITION table.
			try
			{
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad( "NETWORK_DEFINITION", strOutFile, '\t' );
						break;
					case "ORACLE":
						//throw new NotImplementedException( "TODO: figure out columns for CreateTempShapeFileSRS()" );
						List<string> columnNames = new List<string>();
						columnNames.Add( "ROUTES" );
						columnNames.Add( "BEGIN_STATION" );
						columnNames.Add( "END_STATION" );
						columnNames.Add( "DIRECTION" );
						columnNames.Add( "FACILITY" );
						columnNames.Add( "SECTION" );
						columnNames.Add( "AREA" );
						columnNames.Add( "GEOMETRY" );
						columnNames.Add( "ENVELOPE_MINX" );
						columnNames.Add( "ENVELOPE_MINY" );
						columnNames.Add( "ENVELOPE_MAXX" );
						columnNames.Add( "ENVELOPE_MAXY" );
						DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "NETWORK_DEFINITION", strOutFile, columnNames, "\\t" );
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//break;
				}
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: " + exc.Message );
				return;
			}
            Cursor.Current = Cursors.Arrow;
            lblProgress.Text = "";
            this.Close();
        }

		private void CreateTempShapeFileLRS()
		{
			Cursor.Current = Cursors.WaitCursor;
			lblProgress.Text = "Creating TEMP_SHAPEFILE table...";
			this.Refresh();

			Global.CreateTempShapeFile(m_strShapeFilePath);

			lblProgress.Text = "Reading from NETWORK_DEFINITION table...";
			this.Refresh();

			// Now, from NETWORK_DEFINITION get all rows where LRS IS NOT NULL and GEOMETRY IS NULL.
			// this should pull from the NETWORK_DEFINITION table all rows that are non linear based, and
			// are missing a geometry.
			NetworkDefinitionData nddTemp = null;
			String strKey;
			String strQuery = "SELECT ROUTES, DIRECTION, BEGIN_STATION, END_STATION, GEOMETRY, Envelope_MinX, Envelope_MinY, Envelope_MaxX, Envelope_MaxY FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL AND (GEOMETRY IS NULL)";
			DataReader drNetworkDefinition = new DataReader(strQuery);
			while (drNetworkDefinition.Read())
			{
				nddTemp = new NetworkDefinitionData();
				nddTemp.Routes = drNetworkDefinition["ROUTES"].ToString();
				nddTemp.Begin_Station = double.Parse(drNetworkDefinition["BEGIN_STATION"].ToString());
				nddTemp.End_Station = double.Parse(drNetworkDefinition["END_STATION"].ToString());
				nddTemp.Direction = drNetworkDefinition["DIRECTION"].ToString();
				nddTemp.Geometry = drNetworkDefinition["GEOMETRY"].ToString();

				m_listNDD.Add(nddTemp);

				strKey = nddTemp.Routes + '\t' + nddTemp.Begin_Station.ToString() + '\t' + nddTemp.End_Station.ToString() + '\t' + nddTemp.Direction;
				m_htNDD.Add(strKey, nddTemp);
			}
			drNetworkDefinition.Close();

			lblProgress.Text = "Populating geometries and envelopes...";
			this.Refresh();

			// Now loop through the Shapefile result set, and see if a LRS match occurs in the m_htNDD
			// hashtable.  If a match occurs, grab the object out of the hash and populate its .Geometry field (as
			// it should be empty).  Then move on to the next row.
			//DataReader drTempShapeFile = new DataReader(tbSelect.Text);
				DataSet ds = DBMgr.ExecuteQuery(tbSelect.Text);
				nddTemp = null;
				foreach (DataRow drTempShapeFile in ds.Tables[0].Rows)
				{
					try
					{
						// Extract and create LRS field for lookup in the m_htNDD.
						strKey = drTempShapeFile["ROUTES"].ToString() + '\t' + drTempShapeFile["BEGIN_STATION"].ToString()
									+ '\t' + drTempShapeFile["END_STATION"].ToString() + '\t' + drTempShapeFile["DIRECTION"].ToString();
						if (m_htNDD.Contains(strKey))
						{
							nddTemp = (NetworkDefinitionData)m_htNDD[strKey];
							if (nddTemp.Geometry == "")
							{
								nddTemp.Geometry = drTempShapeFile["GEOMETRY"].ToString();
								nddTemp.Geo = Geometry.GeomFromText(nddTemp.Geometry);
							}
							nddTemp.CreateEnvelope();
						}
						else
						{
							// Put into seperate list, to view in DGV later
							nddTemp = new NetworkDefinitionData();
							nddTemp.Routes = drTempShapeFile["ROUTES"].ToString();
							nddTemp.Begin_Station = double.Parse(drTempShapeFile["BEGIN_STATION"].ToString());
							nddTemp.End_Station = double.Parse(drTempShapeFile["END_STATION"].ToString());



							nddTemp.Direction = drTempShapeFile["DIRECTION"].ToString();
							if (nddTemp.Routes.Trim() != "" && nddTemp.Begin_Station.ToString().Trim() != "" &&
								nddTemp.End_Station.ToString().Trim() != "" && nddTemp.Direction.Trim() != "")
							{
								nddTemp.Geometry = drTempShapeFile["GEOMETRY"].ToString();
								if (nddTemp.Geometry != "")
								{
									nddTemp.Geo = Geometry.GeomFromText(nddTemp.Geometry);

									if (nddTemp.Geo.AsText() != "MULTILINESTRING EMPTY")
									{
										nddTemp.CreateEnvelope();
									}
								}

								if (!m_htSFD.Contains(strKey))
								{
									m_listSFD.Add(nddTemp);
									m_htSFD.Add(strKey, nddTemp);    // This hash contains every ndd in the shapefile.
								}
								else
								{
									Global.WriteOutput("Warning: " + strKey + " - Duplicate ROUTE/BEGIN-END-DIRECTION combination detected. Ignoring duplicate Facility/Section");
								}

							}
						}
					}
					catch (Exception exc)
					{
						Global.WriteOutput("Error: Could not read from TEMP_SHAPEFILE. " + exc.Message);
						//return;
					}
				}
			

			// Run a query for NDDs that have a valid LRS and GEOMETRY the results of this query are
			// compared to the m_htSFD and if a match occurs, then we remove them from the m_htSFD hash. This prevents
			// duplicate sections in the NETWORK_DEFINITION table from being entered via the shapefile.
			strQuery = "SELECT ROUTES, BEGIN_STATION, END_STATION, DIRECTION FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL AND GEOMETRY IS NOT NULL";
			try
			{
				DataReader dr = new DataReader(strQuery);
				while (dr.Read())
				{
					strKey = dr["ROUTES"].ToString() + '\t' + dr["BEGIN_STATION"].ToString()
								+ '\t' + dr["END_STATION"].ToString() + '\t' + dr["DIRECTION"].ToString();
					if (m_htSFD.Contains(strKey))
					{
						m_listSFD.Remove((NetworkDefinitionData)m_htSFD[strKey]);
						m_htSFD.Remove(strKey);
					}
				}
				dr.Close();
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: Problem resolving duplicate sections." + exc.Message);
				return;
			}

			lblProgress.Text = "Outputting new data to file...";
			this.Refresh();

			// Write each ROUTE BEGIN_STATION END_STATION GEOMETRY out to a file, then SQLBulkLoad into the NETWORK_DEFINITION table.
			TextWriter tw = null;
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\paste.txt";
			try
			{
				tw = new StreamWriter(strOutFile);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: " + exc.Message);
				return;
			}

			for (int i = 0; i < m_listNDD.Count; i++)
			{
				nddTemp = (NetworkDefinitionData)m_htNDD[m_listNDD[i].Routes + '\t' +
					m_listNDD[i].Begin_Station.ToString() + '\t' +
					m_listNDD[i].End_Station.ToString() + '\t' + 
					m_listNDD[i].Direction];
				
				tw.Write(System.DBNull.Value.ToString() + '\t' + 
					nddTemp.Routes + '\t' + 
					nddTemp.Begin_Station + '\t' + 
					nddTemp.End_Station + '\t' + 
					nddTemp.Direction + '\t' + 
					System.DBNull.Value.ToString() + '\t' + 
					System.DBNull.Value.ToString() + '\t' + 
					System.DBNull.Value.ToString() + '\t' + 
					nddTemp.Geometry + '\t' + 
					nddTemp.EnvelopeMinX + '\t' + 
					nddTemp.EnvelopeMinY + '\t' + 
					nddTemp.EnvelopeMaxX + '\t' + 
					nddTemp.EnvelopeMaxY);

				tw.Write("\r\n");
			}
			tw.Close();

			// Delete the old data from the NETWORK_DEFINITION table where FACILITY IS NOT NULL AND GEOMETRY IS NULL
			try
			{
				String strSqlCmd = "DELETE FROM NETWORK_DEFINITION WHERE ROUTES IS NOT NULL AND (GEOMETRY IS NULL)";
				DBMgr.ExecuteNonQuery(strSqlCmd);
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: " + exc.Message);
				return;
			}

			// Perform the BULK INSERT into the NETWORK_DEFINITION table.
			try
			{
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad( "NETWORK_DEFINITION", strOutFile, '\t' );
						break;
					case "ORACLE":
						List<string> columnNames = new List<string>();
						columnNames.Add( "ROUTES" );
						columnNames.Add( "BEGIN_STATION" );
						columnNames.Add( "END_STATION" );
						columnNames.Add( "DIRECTION" );
						columnNames.Add( "FACILITY" );
						columnNames.Add( "SECTION" );
						columnNames.Add( "AREA" );
						columnNames.Add( "GEOMETRY" );
						columnNames.Add( "ENVELOPE_MINX" );
						columnNames.Add( "ENVELOPE_MINY" );
						columnNames.Add( "ENVELOPE_MAXX" );
						columnNames.Add( "ENVELOPE_MAXY" );
						DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "NETWORK_DEFINITION", strOutFile, columnNames, "\\t" );
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//break;
				}
			}
			catch( Exception exc )
			{
				Global.WriteOutput( "Error: " + exc.Message );
				return;
			}
			Cursor.Current = Cursors.Arrow;
			lblProgress.Text = "";
			this.Close();
		}

        private void FormImportShapeFile_Load(object sender, EventArgs e)
        {
            tbSelect.Text = "SELECT QUADRANT + '-' + STREET AS FACILITY, FROMINTERS + '-' + TOINTERSEC AS SECTION, SHAPE_LENG AS AREA, GEOMETRY FROM TEMP_SHAPEFILE";
        }

        public List<NetworkDefinitionData> GetShapeFileOnlyNDDs()
        {
            return m_listSFD;
        }

        public Hashtable GetSFDHash()
        {
            return m_htSFD;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
