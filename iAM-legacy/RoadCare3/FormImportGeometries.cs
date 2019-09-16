using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using SharpMap.Data.Providers;
using SharpMap.Data;
using DataEntryChecker;
using SharpMap.Geometries;
using System.IO;
using DatabaseManager;
using System.Data.SqlClient;

namespace RoadCare3
{
    /// <summary>
    /// This class is to be used for linear pavement inputs only.  A seperate class has been created to deal with
    /// shapefile imports.  The fields in the datagridview on the main form of this class must match exactly with
    /// the fields being pasted in.  The create geometries button orders the data by Route Direction Milepost
    /// and then generates a Linestring from the lat long data.  It then puts the parsed Linestrings into the network
    /// definition table in the databse according to its Route direction milepost descriptors.
    /// </summary>
    public partial class FormImportGeometries : DockContent
    {
        private bool m_bIsLinear;

        public FormImportGeometries(bool bIsLinear)
        {
            InitializeComponent();
            m_bIsLinear = bIsLinear;
        }

        private void ToggleLinearOrSectionView()
        {
            if (m_bIsLinear == true)
            {
                dgvImportGeometries.Columns["colSection"].Visible = false;
                dgvImportGeometries.Columns["colGeo"].Visible = false;

                dgvImportGeometries.Columns["colRoute"].Visible = true;
                dgvImportGeometries.Columns["colDirection"].Visible = true;
                dgvImportGeometries.Columns["colMilepost"].Visible = true;
                dgvImportGeometries.Columns["colLat"].Visible = true;
                dgvImportGeometries.Columns["colLong"].Visible = true;
            }
            else
            {
                dgvImportGeometries.Columns["colSection"].Visible = true;
                dgvImportGeometries.Columns["colGeo"].Visible = true;

                dgvImportGeometries.Columns["colRoute"].Visible = false;
                dgvImportGeometries.Columns["colDirection"].Visible = false;
                dgvImportGeometries.Columns["colMilepost"].Visible = false;
                dgvImportGeometries.Columns["colLat"].Visible = false;
                dgvImportGeometries.Columns["colLong"].Visible = false;
            }
            
        }

        private void FormImportGeometries_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveFormImportGeometries(this);
        }

        private void FormImportGeometries_Load(object sender, EventArgs e)
        {
            // Change the datagridview to match a linear or section referencing system
            ToggleLinearOrSectionView();
        }

        private void dgvImportGeometries_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                DataObject d = dgvImportGeometries.GetClipboardContent();
                Clipboard.SetDataObject(d);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                // Show the user the pasted data
                string s = Clipboard.GetText();
                char[] delimiterChars = { '\r', '\n' };
                string[] lines = s.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                int row = dgvImportGeometries.CurrentCell.RowIndex;
                int col = dgvImportGeometries.CurrentCell.ColumnIndex;
                dgvImportGeometries.RowCount = lines.Length + 1;
                foreach (string line in lines)
                {
                    if (row < dgvImportGeometries.RowCount && line.Length > 0)
                    {
                        string[] cells = line.Split('\t', ',');
                        for (int i = 0; i < cells.GetLength(0); ++i)
                        {
                            if (col + i < this.dgvImportGeometries.ColumnCount)
                            {
                                dgvImportGeometries[col + i, row].Value = Convert.ChangeType(cells[i], dgvImportGeometries[col + i, row].ValueType);
                            }
                            else
                            {
                                break;
                            }
                        }
                        row++;
                    }
                }
            }
        }

        private void CreateGeometries()
        {
            // Delete any records in the temp geom table
            try
            {
                DBMgr.ExecuteNonQuery("DELETE FROM TEMPORARY_GEOMETRY");
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Couldn't delete data from TEMPORARY_GEOMETRY. " + sqlE.Message);
            }

            // TODO: This is for LINEAR networks ONLY, generate algorithm to handle section based as well.

            // Puts the copied data into the SQL data table
			String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
			Directory.CreateDirectory(strMyDocumentsFolder);

			String strOutFile = strMyDocumentsFolder + "\\paste.txt";
			//dsmelser 2009.01.08
			//needed for Oracle
			List<string> columnNames = new List<string>();
			try
            {
                TextWriter tw = new StreamWriter(strOutFile);
                String strOut = "";
                for (int i = 0; i < dgvImportGeometries.Rows.Count; i++)
                {
                    if (dgvImportGeometries.Rows[i].Cells[0].Value == null ||
                        dgvImportGeometries.Rows[i].Cells[1].Value == null ||
                        dgvImportGeometries.Rows[i].Cells[2].Value == null ||
                        dgvImportGeometries.Rows[i].Cells[3].Value == null ||
                        dgvImportGeometries.Rows[i].Cells[4].Value == null)
                    {
                        continue;
                    }
                    if (dgvImportGeometries.Rows[i].Cells[0].Value.ToString() == "" ||
                        dgvImportGeometries.Rows[i].Cells[1].Value.ToString() == "" ||
                        dgvImportGeometries.Rows[i].Cells[2].Value.ToString() == "" ||
                        dgvImportGeometries.Rows[i].Cells[3].Value.ToString() == "" ||
                        dgvImportGeometries.Rows[i].Cells[4].Value.ToString() == "")
                    {
                        strOut += "Row: " + i + " has empty fields. " +
                                           dgvImportGeometries.Rows[i].Cells[0].Value.ToString() + '\t' +
                                           dgvImportGeometries.Rows[i].Cells[1].Value.ToString() + '\t' +
                                           dgvImportGeometries.Rows[i].Cells[2].Value.ToString() + '\t' +
                                           dgvImportGeometries.Rows[i].Cells[3].Value.ToString() + '\t' +
                                           dgvImportGeometries.Rows[i].Cells[4].Value.ToString() + "\r\n";
                        continue;
                    }
                    for (int j = 0; j < dgvImportGeometries.Columns.Count; j++)
                    {
						//dsmelser 2009.01.08
						//needed for Oracle
						columnNames.Add( dgvImportGeometries.Columns[j].Name );

                        if (j != 0)
                        {
                            tw.Write('\t');
                        }
                        if (dgvImportGeometries[j, i].Value != null)
                        {
                            tw.Write(dgvImportGeometries[j, i].Value.ToString());
                        }
                    }
                    tw.Write("\r\n");
                }
                Global.WriteOutput(strOut);
                tw.Close();
            }
            catch (IOException ioE) { Global.WriteOutput("Error: Writing to paste file caused an error. " + ioE.Message); }
            catch (ObjectDisposedException odE) { Global.WriteOutput("Error: Writing to file caused an error. " + odE.Message); }


            try
            {
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "MSSQL":
							DBMgr.SQLBulkLoad( "TEMPORARY_GEOMETRY", strOutFile, "\\t" );
							break;
						case "ORACLE":
							DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, "TEMPORARY_GEOMETRY", strOutFile, columnNames, "\\t" );
							break;
						default:
							throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
							//break;
					}
			}
            catch (Exception exc)
            {
                Global.WriteOutput("Error loading SQL data into temp geom table. " + exc.Message);
            }

            // SELECT FROM ORDERBY Statement
            //SqlDataReader dr = DBMgr.CreateDataReader("SELECT * FROM TEMPORARY_GEOMETRY ORDER BY ROUTE, DIRECTION, MILEPOST");
            DataReader dr = new DataReader("SELECT * FROM TEMPORARY_GEOMETRY ORDER BY ROUTE, DIRECTION, MILEPOST");

            String strRoute = "";
            String strDir = "";
            String strUpdate = "";

            float fLat;
            float fLong;

            float fMinLat = 99999999;
            float fMaxLat = -99999999;
            float fMinLong = 99999999;
            float fMaxLong = -99999999;

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("LINESTRING(");
            String strPrevRoute = "";
            String strPrevDir = "";

            // Get first row of values.
            if (dr.Read())
            {
                strPrevRoute = dr[0].ToString();
                strPrevDir = dr[1].ToString();

                try
                {
                    fLat = float.Parse(dr[3].ToString());
                    fLong = float.Parse(dr[4].ToString());
                    strBuilder.Append(fLong.ToString() + " " + fLat.ToString() + ", ");
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.Write(exc.Message + " FLAT: " + dr[3].ToString());
                    System.Diagnostics.Debug.WriteLine("FLONG: " + dr[4].ToString());
                }


            }
            else
            {
                // No values to read in.
                return;
            }
            int iCount = 0;

            bool bError;
            while (dr.Read())
            {
                bError = false;

                // Parse new read into corresponding fields
                iCount++;

                if (dr[0] != null && dr[1] != null)
                {
                    strRoute = dr[0].ToString();
                    strDir = dr[1].ToString();
                }
                else
                {
                    bError = true;
                }

                try
                {
                    fLat = float.Parse(dr[3].ToString());
                    fLong = float.Parse(dr[4].ToString());
                }
                catch (Exception exc)
                {
                    System.Diagnostics.Debug.Write(exc.Message + " FLAT: " + dr[3].ToString());
                    System.Diagnostics.Debug.WriteLine("FLONG: " + dr[4].ToString());
                    fLat = 0;
                    fLong = 0;
                }

                if (fLat == 0 || fLong == 0)
                {
                    // Error converting the lat long values, so skip this spot?
                    bError = true;
                }

                if (bError == false)
                {
                    // Set max and min lat and long values for the envelope columns.
                    if (fMinLat > fLat) { fMinLat = fLat; }
                    if (fMaxLat < fLat) { fMaxLat = fLat; }
                    if (fMinLong > fLong) { fMinLong = fLong; }
                    if (fMaxLong < fLong) { fMaxLong = fLong; }

                    // Check for definition changes
                    if (strRoute != strPrevRoute || strDir != strPrevDir)
                    {
                        // Hack off the extra comma
                        strBuilder.Remove(strBuilder.Length - 2, 2);
                        strBuilder.Append(")");

                        // Route change, Perform an insert into the NETWORK_DEFINITION table
                        strUpdate = "UPDATE NETWORK_DEFINITION SET GEOMETRY = '" + strBuilder.ToString() + "', Envelope_MinX = '" + fMinLat.ToString() + "', Envelope_MinY = '" + fMinLong.ToString() + "', Envelope_MaxX = '" + fMaxLat.ToString() + "', Envelope_MaxY = '" + fMaxLong.ToString() + "'" +
                                        " WHERE ROUTES = '" + strPrevRoute + "' AND DIRECTION = '" + strPrevDir + "'";
                        if (!checkBoxOverwriteGeometries.Checked)
                        {
                            strUpdate += " AND GEOMETRY IS NULL";
                        }
                        try
                        {
                            DBMgr.ExecuteNonQuery(strUpdate);
                        }
                        catch (Exception sqlE)
                        {
                            Global.WriteOutput("Error: Could not update NETWORK_DEFINITION table. " + sqlE.Message);
                        }

                        // Reset the geom string
                        strBuilder = new StringBuilder();
                        strBuilder.Append("LINESTRING(");

                        // Reset the MIN and MAX values for the envelope.
                        fMinLat = 99999999;
                        fMaxLat = -99999999;
                        fMinLong = 99999999;
                        fMaxLong = -99999999;
                    }

                    // Set current values to the previous values before the next iteration
                    // Add to geom string
                    strBuilder.Append(fLong.ToString() + " " + fLat.ToString() + ", ");
                    strPrevRoute = strRoute;
                    strPrevDir = strDir;
                }
            }
            dr.Close();
            strBuilder = strBuilder.Remove(strBuilder.Length - 2, 2);
            strBuilder.Append(")");

            // Perform last update
            strUpdate = "UPDATE NETWORK_DEFINITION SET GEOMETRY = '" + strBuilder.ToString() + "', Envelope_MinX = '" + fMinLat + "', Envelope_MinY = '" + fMinLong + "', Envelope_MaxX = '" + fMaxLat + "', Envelope_MaxY = '" + fMaxLong + "'" +
                        " WHERE ROUTES = '" + strPrevRoute + "' AND DIRECTION = '" + strPrevDir + "'";
            try
            {
                DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception sqlE)
            {
                Global.WriteOutput("Error: Could not update NETWORK_DEFINITION table. " + sqlE.Message);
            }
        }

        private void btnCreateGeometries_Click(object sender, EventArgs e)
        {
            CreateGeometries();
        }
    }
}
