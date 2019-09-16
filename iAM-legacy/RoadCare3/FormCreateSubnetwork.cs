using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DatabaseManager;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using RoadCareDatabaseOperations;

namespace RoadCare3
{
    public partial class FormCreateSubnetwork : Form
    {
        private Hashtable m_hashAttributeYear;
        private String m_strNetworkID;
        private String m_strNetwork;
        private String m_strNewNetworkID;
        private String m_strNewNetworkName;

        public String NewNetworkID
        {
            set { m_strNewNetworkID = value; }
            get { return m_strNewNetworkID; }

        }

        public String NewNetworkName
        {
            set { m_strNewNetworkName = value; }
            get { return m_strNewNetworkName; }

        }

        public FormCreateSubnetwork(String strNetworkID, String strNetwork, Hashtable hashAttributeYear)
        {
            InitializeComponent();
            m_strNetworkID = strNetworkID;
            m_hashAttributeYear = hashAttributeYear;
            m_strNetwork = strNetwork;
        }


        private void FormCreateSubnetwork_Load(object sender, EventArgs e)
        {
            String strSelect = "SELECT COUNT(*) FROM SECTION_" + m_strNetworkID;
            try
            {
                 DataSet ds = DBMgr.ExecuteQuery(strSelect);

            }
            catch
            {
                Global.WriteOutput("Error: Network must be rolled up before creating a subnetwork.");
                this.Cursor = Cursors.Default;
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }

        }



        private void buttonCriteria_Click(object sender, EventArgs e)
        {
            FormAdvancedSearch form = new FormAdvancedSearch(m_strNetwork, m_hashAttributeYear,"", false);
            if (form.ShowDialog() == DialogResult.OK)
            {
                textBoxFilter.Text = form.Query;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            m_strNewNetworkName = textBoxNetworkName.Text;
            String strWhere = textBoxFilter.Text;
            this.Cursor = Cursors.WaitCursor;
            if (m_strNewNetworkName.Trim() == "")
            {
                Global.WriteOutput("Error: Network Name must be entered..");
                this.Cursor = Cursors.Default;
                return;
            }
            //Check if Network Name is already in use.

            String strSelect = "SELECT * FROM NETWORKS WHERE NETWORK_NAME='" + m_strNewNetworkName + "'";

            String strDescription="";
            String strDesignerUserID = "";
            String strDesignerUserName = "";
			//String strLock = "";
			//String strPrivate = "";
            
            
            
            
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    //Network with this name exists.
                    Global.WriteOutput("Error: Network with this name already exists.  Please select another.");
                    this.Cursor = Cursors.Default;
                    return;
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Checking Network Name uniqueness.   Please select a different Network Name." + exception.Message);
                this.Cursor = Cursors.Default;
                return;
            }



            strSelect = "SELECT * FROM NETWORKS WHERE NETWORKID=" + m_strNetworkID;
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);

                if (ds.Tables[0].Rows.Count == 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                
                    strDescription= dr["DESCRIPTION"].ToString();
                    strDesignerUserID = dr["DESIGNER_USERID"].ToString();
                    strDesignerUserName = dr["DESIGNER_NAME"].ToString();
					//strLock = dr["LOCK_"].ToString();
					//strPrivate = dr["PRIVATE_"].ToString();
                
                
                
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Checking Network Name uniqueness.   Please select a different Network Name." + exception.Message);
                this.Cursor = Cursors.Default;
                return;
            }







            String strInsert = "INSERT INTO NETWORKS (NETWORK_NAME";
            String strValues = " VALUES ('" + m_strNewNetworkName + "'";

            if (strDescription != "")
            {
                strInsert += ",DESCRIPTION";
                strValues += ",'" + strDescription + "'";
            }


            if (strDesignerUserID != "")
            {
                strInsert += ",DESIGNER_USERID";
                strValues += ",'" + strDesignerUserID + "'";
            }

            if (strDesignerUserName != "")
            {
                strInsert += ",DESIGNER_NAME";
                strValues += ",'" + strDesignerUserName + "'";
            }


			//if (strLock != "")
			//{
			//    strInsert += ",LOCK_";
			//    strValues += ",'" + strLock + "'";
			//}
            
			//if (strPrivate != "")
			//{
			//    strInsert += ",PRIVATE_";
			//    strValues += ",'" + strPrivate + "'";
			//}

            strInsert += ",DATE_CREATED";
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					strValues += ",'" + DateTime.Now.ToString() + "'";
					break;
				case "ORACLE":
					strValues += ",to_date('" + DateTime.Now.ToString( "MM/dd/yyyy" ) + "','MM/DD/YYYY')";
					break;
				default:
					throw new NotImplementedException( "TODO: Implement ANSI version of buttonCreate_Click()" );
			}
            
            strInsert += ",DATE_LAST_EDIT)";

			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					strValues += ",'" + DateTime.Now.ToString() + "')";
					break;
				case "ORACLE":
					strValues += ",to_date('" + DateTime.Now.ToString( "MM/dd/yyyy" ) + "','MM/DD/YYYY'))";
					break;
				default:
					throw new NotImplementedException( "TODO: Implement ANSI version of buttonCreate_Click()" );
			}
            



            strInsert += strValues;

            try
            {
                DBMgr.ExecuteNonQuery(strInsert);
            }
            catch(Exception exception)
            {
                Global.WriteOutput("Error: Creating new subnetwork NETWORKS entry" + exception.Message);
                this.Cursor = Cursors.Default;
                return;
            }


			String strIdentity = "";
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						strIdentity = "SELECT IDENT_CURRENT ('NETWORKS') FROM NETWORKS";
						break;
					case "ORACLE":
						//strIdentity = "SELECT NETWORKS_NETWORKID_SEQ.CURRVAL FROM DUAL";
						//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'NETWORKS_NETWORKID_SEQ'";
						strIdentity = "SELECT MAX(NETWORKID) FROM NETWORKS";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
						//break;
				}
			try
            {
                DataSet ds = DBMgr.ExecuteQuery(strIdentity);
                strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();

            }
            catch(Exception exception)
            {
                Global.WriteOutput("Error: Creating new subnetwork NETWORKS entry." + exception.Message);
                this.Cursor = Cursors.Default;
                return;
            }

            //Now have the new NetworkInserted.  Must create tables.  SECTION_NID  and SEGMENT_NID_NS0

            String strSelectSection = "SELECT SECTION_" + m_strNetworkID + ".SECTIONID,FACILITY,BEGIN_STATION,END_STATION,DIRECTION,SECTION,AREA,UNITS,GEOMETRY,Envelope_MinX,Envelope_MaxX,Envelope_MinY,Envelope_MaxY FROM SECTION_" + m_strNetworkID + " INNER JOIN SEGMENT_" + m_strNetworkID.ToString() + "_NS0 ON SECTION_" + m_strNetworkID.ToString() + ".SECTIONID=SEGMENT_" + m_strNetworkID + "_NS0.SECTIONID";
            if(strWhere.Trim() != "")
            {
                strSelectSection += " WHERE " + strWhere;
            }


            //String strSelectSegment = "SELECT * FROM SEGMENT_" + m_strNetworkID + "_NS0";

			String strSelectSegment = "SELECT * " + DBOp.BuildFromStatement(m_strNetworkID);
            if(strWhere.Trim() != "")
            {
                strSelectSegment += " WHERE " + strWhere;
                this.Cursor = Cursors.Default;
            }

            //In
            try
            {   // Create new SECTION_
                            // Create new tables
                // SEGMENT_networkid
                // This table is for SECTIONID, FACILITY, BEGIN_STATION, END_STATION, DIRECTION, SECTION,AREA,UNITS
                List<DatabaseManager.TableParameters> listColumn = new List<DatabaseManager.TableParameters>();
                listColumn.Add(new DatabaseManager.TableParameters("SECTIONID",DataType.Int,false,true));
                listColumn.Add(new DatabaseManager.TableParameters("FACILITY", DataType.VarChar(4000), false));
                listColumn.Add(new DatabaseManager.TableParameters("BEGIN_STATION", DataType.Float, true));
                listColumn.Add(new DatabaseManager.TableParameters("END_STATION", DataType.Float, true));
                listColumn.Add(new DatabaseManager.TableParameters("DIRECTION", DataType.VarChar(50), true));
				listColumn.Add(new DatabaseManager.TableParameters("SECTION", DataType.VarChar(4000), false));
                listColumn.Add(new DatabaseManager.TableParameters("AREA", DataType.Float, true));
                listColumn.Add(new DatabaseManager.TableParameters("UNITS", DataType.VarChar(50), true));
				listColumn.Add(new DatabaseManager.TableParameters("GEOMETRY", DataType.VarChar(-1), true));
				listColumn.Add( new DatabaseManager.TableParameters( "ENVELOPE_MINX", DataType.Float, true ) );
				listColumn.Add( new DatabaseManager.TableParameters( "ENVELOPE_MAXX", DataType.Float, true ) );
				listColumn.Add( new DatabaseManager.TableParameters( "ENVELOPE_MINY", DataType.Float, true ) );
				listColumn.Add( new DatabaseManager.TableParameters( "ENVELOPE_MAXY", DataType.Float, true ) );

				List<string> orderedOracleColumns = new List<string>();
				orderedOracleColumns.Add("SECTIONID");
				orderedOracleColumns.Add("FACILITY");
				orderedOracleColumns.Add("BEGIN_STATION");
				orderedOracleColumns.Add("END_STATION");
				orderedOracleColumns.Add("DIRECTION");
				orderedOracleColumns.Add("SECTION");
				orderedOracleColumns.Add("AREA");
				orderedOracleColumns.Add("UNITS");
				orderedOracleColumns.Add("GEOMETRY");
				orderedOracleColumns.Add("ENVELOPE_MINX");
				orderedOracleColumns.Add("ENVELOPE_MAXX");
				orderedOracleColumns.Add("ENVELOPE_MINY");
				orderedOracleColumns.Add("ENVELOPE_MAXY");

                String strTable = "SECTION_" + strIdentity;
                DBMgr.CreateTable(strTable, listColumn);

				//String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				//strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
				//Directory.CreateDirectory(strMyDocumentsFolder);
				string specialFolder = Directory.GetCurrentDirectory() + "\\Temp";
				Directory.CreateDirectory(specialFolder);
				String strOutFile = specialFolder + "\\subnetwork_section.txt";
                TextWriter tw = new StreamWriter(strOutFile);
                DataSet dsSection = DBMgr.ExecuteQuery(strSelectSection);
                foreach(DataRow dr in dsSection.Tables[0].Rows)
                {
                    String strOut = "";
                    for(int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        strOut += dr.ItemArray[i].ToString();
                        if(i == dr.ItemArray.Length - 1)
                        {
                            tw.WriteLine(strOut);
                        }
                        else
                        {
                            strOut += "\t";
                        }
                    }
                }
                tw.Close();
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad(strTable, strOutFile, '\t');
						break;
					case "ORACLE":
						DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, strTable, strOutFile, orderedOracleColumns, "\t");
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
			}
            catch(Exception exception)
            {
                Global.WriteOutput("Error: Creating SECTION_ table for Network = " + m_strNewNetworkName + ". " + exception.Message);
                this.Cursor = Cursors.Default;
                return;
            }


            try
            {
				DataSet dsSegment = DBMgr.ExecuteQuery("SELECT * FROM SEGMENT_" + m_strNetworkID + "_NS0");
                List<DatabaseManager.TableParameters> listColumn = new List<DatabaseManager.TableParameters>();
                foreach (DataColumn dc in dsSegment.Tables[0].Columns)
                {   
                    DataType dt = DataType.Int;
                    Microsoft.SqlServer.Management.Smo.DataType smoDT = Microsoft.SqlServer.Management.Smo.DataType.Int;
                    if(dc.DataType == typeof(int))smoDT = Microsoft.SqlServer.Management.Smo.DataType.Int;
                    if(dc.DataType == typeof(double))smoDT = Microsoft.SqlServer.Management.Smo.DataType.Float;
                    if(dc.DataType == typeof(string))smoDT = Microsoft.SqlServer.Management.Smo.DataType.VarChar(4000);
 

                    if (dc.ColumnName == "SECTIONID")
                    {
                        listColumn.Add(new DatabaseManager.TableParameters(dc.ColumnName, smoDT, false, true));
                    }
                    else
                    {
                        listColumn.Add(new DatabaseManager.TableParameters(dc.ColumnName, smoDT, true, false));
                    }

                }
                String strTable = "SEGMENT_" + strIdentity + "_NS0";
                DBMgr.CreateTable(strTable, listColumn);

				String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
				Directory.CreateDirectory(strMyDocumentsFolder);


				String strOutFile = strMyDocumentsFolder + "\\subnetwork_segment.txt";
                TextWriter tw = new StreamWriter(strOutFile);
                DataSet dsSection = DBMgr.ExecuteQuery(strSelectSection);

                foreach (DataRow dr in dsSegment.Tables[0].Rows)
                {
                    String strOut = "";
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        strOut += dr.ItemArray[i].ToString();
                        if (i == dr.ItemArray.Length - 1)
                        {
                            tw.WriteLine(strOut);
                        }
                        else
                        {
                            strOut += "\t";
                        }
                    }
                }
                tw.Close();
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						DBMgr.SQLBulkLoad(strTable, strOutFile, '\t');
						break;
					case "ORACLE":
						List<string> oracleSegmentColumns = new List<string>();
						foreach( DataColumn segmentColumn in dsSegment.Tables[0].Columns )
						{
							oracleSegmentColumns.Add( segmentColumn.ColumnName );
						}
						DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, strTable, strOutFile, oracleSegmentColumns, "\t" );
						//throw new NotImplementedException("TODO: figure out columns for buttonCreate_Click()");
						//DBMgr.OracleBulkLoad( DBMgr.NativeConnectionParameters, strTable, strOutFile,
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}

            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Creating SEGMENT_ table for Network = " + m_strNewNetworkName + ". " + exception.Message);
                this.Cursor = Cursors.Default;
                return;
            }

            strSelect = "SELECT * FROM SEGMENT_CONTROL WHERE NETWORKID = " + m_strNetworkID;

            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    String strTable = dr["SEGMENT_TABLE"].ToString();
                    strInsert = "";
                    if (strTable.Contains("SECTION"))
                    {
                        strInsert = "INSERT INTO SEGMENT_CONTROL (NETWORKID,SEGMENT_TABLE) VALUES (" + strIdentity + ",'SECTION_" + strIdentity + "')";


                    }
                    else
                    {
                        strInsert = "INSERT INTO SEGMENT_CONTROL (NETWORKID,SEGMENT_TABLE";
                        String strValue = " VALUES (" + strIdentity + ",'SEGMENT_" + strIdentity + "_NS0'";

                        if (dr["ATTRIBUTE_"].ToString() != "")
                        {
                            strInsert += ",ATTRIBUTE_";
                            strValue += ",'" + dr["ATTRIBUTE_"].ToString() + "'";
                        }

                        strInsert += ")" + strValue + ")";
                    }
                    DBMgr.ExecuteNonQuery(strInsert);
                }
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Inserting values into SEGMENT_CONTROL." + exception.Message);
            }

			Global.SecurityOperations.CopyNetworkPoliciesFromTo( m_strNetworkID, strIdentity );


            NewNetworkID = strIdentity;
            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Cursor = Cursors.Default;

        }
    }
}
