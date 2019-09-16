using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using DatabaseManager;
using System.Windows.Forms;

namespace RoadCare3
{
    public class DoImport
    {
        private String m_strSQL;
        private List<String> m_listOutputs = new List<String>();
        private bool m_bIsLinear;
        private bool m_bIsString;
        private List<String> m_listRouteFacility;
        private String m_strAttributeName;
        private ConnectionParameters m_cp;

        public DoImport(String strSQL, bool bIsLinear, bool bIsString, List<String> listFacility, String strAttributeName, ConnectionParameters cp)
        {
            m_strSQL = strSQL;
            m_bIsLinear = bIsLinear;
            m_bIsString = bIsString;
            m_listRouteFacility = listFacility;
            m_strAttributeName = strAttributeName;
            m_cp = cp;
        }

        /// <summary>
        /// Attribute name
        /// </summary>
        public String Name
        {
            get { return m_strAttributeName; }
            set { m_strAttributeName = value; }
        }

        /// <summary>
        /// List of facilities/routes that make up the network
        /// </summary>
        public List<String> Facility
        {
            get { return m_listRouteFacility; }
            set { m_listRouteFacility = value; }
        }

        /// <summary>
        /// IsString is true if the attribute being imported is of type String
        /// </summary>
        public bool IsString
        {
            get { return m_bIsString; }
            set { m_bIsString = value; }
        }

        /// <summary>
        /// IsLinear is true if the attribute being imported is linear
        /// </summary>
        public bool IsLinear
        {
            get { return m_bIsLinear; }
            set { m_bIsLinear = value; }
        }

        /// <summary>
        /// SQL string to perform import.
        /// </summary>
        public String SQL
        {
            get { return m_strSQL; }
            set { m_strSQL = value; }
        }

        /// <summary>
        /// Most recent error(s) to occur in the import thread.
        /// </summary>
        public List<String> Errors
        {
            get { return m_listOutputs; }
            set { m_listOutputs = value; }
        }

        public ConnectionParameters CP
        {
            get { return m_cp; }
            set { m_cp = value; }
        }

        /// <summary>
        /// Takes the SQL query from the text box and runs it against the data server,
        /// the function then writes the data out to a file, and bulk inserts it into the database.
        /// </summary>
        public void Import()
        {
            // Get the SQL statement results and write them out to a file,
            // then bulk load the file into the appropriate attribute table.
            String strMyDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            strMyDocumentsFolder += "\\RoadCare Projects\\Temp";
            Directory.CreateDirectory(strMyDocumentsFolder);
            
            DateTime start = DateTime.Now;

            String strOutFile = strMyDocumentsFolder + "\\AttributeImport.txt";
            TextWriter tw = new StreamWriter(strOutFile);

            String strQuery = m_strSQL;
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery, m_cp);
                string[] strList;
                if (m_bIsLinear)
                {
                    strList = new string[6];
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        strList[0] = dataRow["ROUTES"].ToString();
                        strList[1] = dataRow["BEGIN_STATION"].ToString();
                        strList[2] = dataRow["END_STATION"].ToString();
                        strList[3] = dataRow["DIRECTION"].ToString();
                        strList[4] = dataRow["DATE_"].ToString();
                        strList[5] = dataRow["DATA_"].ToString();

                        String strOut = SQLBulkLoadRawAttributeCheck(strList, m_bIsLinear, m_listRouteFacility, m_bIsString);
                        if (strOut != null)
                        {
                            tw.WriteLine(strOut);
                        }
                    }
                }
                else
                {
                    strList = new string[5];
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {
                        strList[0] = dataRow["FACILITY"].ToString();
                        strList[1] = dataRow["SECTION"].ToString();
                        strList[2] = dataRow["SAMPLE_"].ToString();
                        strList[3] = dataRow["DATE_"].ToString();
                        strList[4] = dataRow["DATA_"].ToString();

                        String strOut = SQLBulkLoadRawAttributeCheck(strList, m_bIsLinear, m_listRouteFacility, m_bIsString);
                        if (strOut != null)
                        {
                            tw.WriteLine(strOut);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
            }

            
            tw.Close();
            TimeSpan span = DateTime.Now - start;
            start = DateTime.Now;
            TimeSpan span2 = new TimeSpan();

            switch (DBMgr.NativeConnectionParameters.Provider)
            {
                case "MSSQL":
                    DBMgr.SQLBulkLoad(m_strAttributeName, strOutFile, '\t');
                    break;
                case "ORACLE":
                    //throw new NotImplementedException( "TODO: Figure out columns for Import()" );
                    List<string> columnNames = DBMgr.GetTableColumns(m_strAttributeName);
                    columnNames.Remove("ID_");
                    span2 = DateTime.Now - start;
                    start = DateTime.Now;
                    DBMgr.OracleBulkLoad(DBMgr.NativeConnectionParameters, m_strAttributeName, strOutFile, columnNames, "\\t");
                    break;
                default:
                    throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                //break;
            }

            TimeSpan span3 = DateTime.Now - start;
            //MessageBox.Show("Span creating text file =" + span.ToString() + "\r\n Get columns =" + span2.ToString() + "\r\n Bulk loading =" + span3.ToString());
        }

        public void ClearErrorList()
        {
            m_listOutputs.Clear();
        }

        /// <summary>
        /// Adds str to the output list to be read from the main thread.
        /// </summary>
        /// <param name="str">Error or warning to hand to main thread.</param>
        private void WriteOutput(String str)
        {
            m_listOutputs.Add(str);
        }

        public String SQLBulkLoadRawAttributeCheck(string[] cells, bool bLinear, List<String> listRouteFacility, bool bIsStringField)
        {
            String strRow = "";
            for (int i = 0; i < cells.Length; i++)
            {
                strRow += cells[i].ToString() + '\t';
            }
            if (bLinear)
            {
                if (cells.Length != 6)
                {
                    WriteOutput("Error: Import aborted, more than six fields detected.");
                    return null;
                }
                //Trim white space.
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = cells[i].Trim();
                }
                if (!listRouteFacility.Contains(cells[0]))
                {
                    WriteOutput(strRow + " not imported because it does not exist in Facility/Route list");
                    return null;
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
                    WriteOutput(strRow + " not imported because BEGIN_STATION and END_STATION must be positive numbers");
                    return null;
                }


                if (fBegin > fEnd)
                {
                    WriteOutput(strRow + " not imported because BEGIN_STATION must be less or equal to END_STATION");
                    return null;
                }


                if (fBegin < 0 || fEnd < 0)
                {
                    WriteOutput(strRow + " not imported because BEGIN_STATION and END_STATION must be positive numbers");
                    return null;
                }

                if (cells[3] == "")
                {
                    WriteOutput(strRow + " not imported because DIRECTION must be included.");
                    return null;
                }

                DateTime date;
                try
                {
                    date = DateTime.Parse(cells[4]);
                }
                catch
                {
                    WriteOutput(strRow + " not imported because DATE format is incorrect.");
                    return null;
                }

                float fNumber;
                if (!bIsStringField)
                {
                    try
                    {
                        fNumber = float.Parse(cells[5]);
                    }
                    catch
                    {
                        WriteOutput(strRow + " not imported because number expected for data column.");
                        return null;
                    }

                }
                String strOut = "";

				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						for (int i = 0; i < cells.Length; i++)
						{
							strOut += "\t";
							if (i == 4)
								strOut += "\t\t\t";
							strOut += cells[i];
						}
						break;
					case "ORACLE":
						for (int i = 0; i < cells.Length; i++)
						{
							if (i == 4)
							{
								strOut += "\t\t\t" + DateTime.Parse(cells[i]).ToString("dd/MMM/yyyy") + "\t";
							}
							else
							{
								strOut += cells[i] + "\t";
							}
						}
						strOut = strOut.Remove(strOut.Length - 1);
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
                return strOut;
            }
            else //SRS
            {
                if (cells.Length != 5)
                {
                    WriteOutput(strRow + " Paste aborted because more/less than 5 columns detected");
                    return null;
                }

                //Trim white space.
                for (int i = 0; i < cells.Length; i++)
                {
                    cells[i] = cells[i].Trim();
                }


                if (!listRouteFacility.Contains(cells[0]))
                {
                    WriteOutput(strRow + " not imported because it does not exist in Facility/Route list");
                    return null;
                }

                if (cells[1] == "")
                {
                    WriteOutput(strRow + " not imported because SECTION must not be blank.");
                    return null;
                }

                DateTime date;
                try
                {
                    date = DateTime.Parse(cells[3]);
                }
                catch
                {
                    WriteOutput(strRow + " not imported because DATE format is incorrect.");
                    return null;
                }

                float fNumber;
                if (!bIsStringField)
                {
                    try
                    {
                        fNumber = float.Parse(cells[4]);
                    }
                    catch
                    {
                        WriteOutput(strRow + " not imported because number expected for data column.");
                        return null;
                    }

                }
                String strOut = "";
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						for (int i = 0; i < cells.Length; i++)
						{
							strOut += "\t";
							if (i == 0)
								strOut += "\t\t\t\t";
							strOut += cells[i];
						}
						break;
					case "ORACLE":
						for (int i = 0; i < cells.Length; i++)
						{
							if (i == 0)
							{
								strOut += "\t\t\t\t";
							}
							if (i != 4)
							{
								strOut += cells[i] + "\t";
							}
							else
							{
								strOut += DateTime.Parse(cells[i]).ToString("dd/MMM/yyyy") + "\t";
							}
						}
						strOut = strOut.Remove(strOut.Length - 1);
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
                return strOut;
            }

        }


    }
}
