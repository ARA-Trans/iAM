using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Windows.Forms;

using System.Drawing;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using System.Data.SqlClient;
using RoadCareDatabaseOperations;
using DatabaseManager;

namespace Reports
{
    public class DetailedResultsReport
    {
        string m_strNetworkID, m_strSimulationID, m_strNetwork, m_strSimulation, m_strBudgetPer;

        public DetailedResultsReport(string strNetworkID, string strSimulationID, string strNetwork, string strSimulation, string strBudgetPer)
        {
            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            m_strBudgetPer = strBudgetPer;
        }
        public void CreateDetailedResultsReport()
        {
            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            string strReportName = "Detailed Results Report";
            Report.SheetPageSetup(oSheet, strReportName, 50d, 20d, 10d, m_strNetwork + " - " + m_strSimulation, DateTime.Now.ToLongDateString(), "Page &P", 1);
            //oSheet.PageSetup.RightFooter = m_strNetwork + " - " + m_strSimulation;
            //oSheet.PageSetup.LeftFooter = DateTime.Now.ToLongDateString();
            //oSheet.PageSetup.CenterFooter = "Page &P";
            //oSheet.PageSetup.FirstPageNumber = 1;
            //oSheet.PageSetup.LeftMargin = 50d;
            //oSheet.PageSetup.RightMargin = 20d;
            //oSheet.Columns.Font.Size = 10;
            //oSheet.Name = strReportName;
            Range oR = oSheet.get_Range("A1", "A1");

            object oEndCell = new object();
            string strFilter = "";

            int sheetRow;
            int ndx;
            DataSet dsPage = null, dsResults = null;

            try
            {
                dsPage = DBOp.QueryPageHeader(strReportName);
                dsResults = DBOp.QueryReport(m_strNetworkID, m_strSimulationID);
            }
            catch (Exception e)
            {
                throw e;
                // Circular reference, address later
                //Global.WriteOutput("Error: Could not fill dataset in CreateBudgetPEr. " + e.Message);

            }

            Cursor c = Cursor.Current;
            Cursor.Current = new Cursor(Cursors.WaitCursor.Handle);

            DataRow drPage = dsPage.Tables[0].Rows[0];
            string strMajorTitle = drPage["phText"].ToString();

            drPage = dsPage.Tables[0].Rows[2];
            string strMinorTitle = drPage["phText"].ToString();
            if (strMinorTitle.IndexOf("@1") > 0) strMinorTitle = strMinorTitle.Replace("@1", m_strNetwork + " (" + m_strNetworkID + ")");
            if (strMinorTitle.IndexOf("@2") > 0) strMinorTitle = strMinorTitle.Replace("@2", m_strSimulation + " (" + m_strSimulationID + ")");


            #region default column widths
            // set some column widths
            oR = oSheet.get_Range("A1:B1", Missing.Value);
            oR.ColumnWidth = 15;
            #endregion

            #region place agency graphic
            //string strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + drPage["reportGraphicFile"].ToString();
			string strPath = ".\\" + drPage["reportGraphicFile"].ToString();
            Report.PlaceReportGraphic(strPath, oSheet.get_Range("A1", Missing.Value), oSheet);
            #endregion

            #region write Major Title
            int aryCols = 2;
            int aryRows = 3;
            object[,] oData = new object[aryRows, aryCols];
            Report.ClearDataArray(ref oData);

            oData[0, 1] = strMajorTitle;
            oData[2, 1] = strMinorTitle;
            sheetRow = 4;

            oEndCell = "A1";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
            #endregion

            #region get Budget Years
			strFilter = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE simulationID = " + m_strSimulationID + "ORDER BY Year_";
            DataSet dsBudgetYears = DBMgr.ExecuteQuery(strFilter);

            int numYears = dsBudgetYears.Tables[0].Rows.Count;
            #endregion

            aryCols = numYears + 2;
            //aryRows = dsResults.Tables[0].Rows.Count / 5 + 2;
            aryRows = dsResults.Tables[0].Rows.Count / numYears + 2;

            #region build Column Headers array
            Report.Resize2DArray(ref oData, aryRows + 1, aryCols);
            Report.ClearDataArray(ref oData);

            object[] oColumnHeader = new object[aryCols];
            oColumnHeader[0] = "Facility";
            oColumnHeader[1] = "Section";
            ndx = 2;
            foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
            {
                oColumnHeader[ndx++] = dr["Year_"].ToString();
            }
            #endregion

            #region build Results table
            ndx = 0;
            oData[ndx, 0] = "Treatment Distribution per Section"; // Group Header

            sheetRow = 5;
            oEndCell = "A" + sheetRow.ToString();

            sheetRow++;
            ndx++;

            for (int i = 0; i < aryCols; i++)
            {
                oData[ndx, i] = oColumnHeader[i];     // Column Header
            }

            sheetRow++;
            ndx++;
            int rowsToColumns = 0, nCol = 2;
            string strGreen = "", strBlue = "", strRed = "";
            List<string> greenList = new List<string>();
            List<string> blueList = new List<string>();
            List<string> redList = new List<string>();

            foreach (DataRow dr in dsResults.Tables[0].Rows)
            {
                string sYear = dr["Years"].ToString();
                if (rowsToColumns < numYears)
                {
                    if (rowsToColumns == 0)
                    {
                        nCol = 2;
						if (ndx == 132)
						{
							string testString = dr["Facility"].ToString();
						}
						oData[ndx, 0] = dr["Facility"].ToString();
						
                        oData[ndx, 1] = dr["Section"].ToString();
                    }
                }

				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":

						// Test for Treatment selected per RoadCare logic (GREEN)
						if( dr["Treatment"].ToString() != "No Treatment" && dr["Iscommitted"].ToString().ToUpper() != "TRUE" && dr["Numbertreatment"].ToString() != "0" )
						{
							string s = Report.GetColumnLetter( nCol + 1 );
							char[] chr = s.ToCharArray( 0, 1 );
							char asciiChar = chr[0];

							Report.BuildRange( ref strGreen, ref greenList, sheetRow, asciiChar, 0 );
							oData[ndx, nCol] = dr["Treatment"].ToString();
						}
						// Test for Feasible treatment not funded/selected (BLUE)
						else if( dr["Treatment"].ToString() == "No Treatment" && dr["Iscommitted"].ToString().ToUpper() != "TRUE" && dr["Numbertreatment"].ToString() != "0" )
						{
							string s = Report.GetColumnLetter( nCol + 1 );
							char[] chr = s.ToCharArray( 0, 1 );
							char asciiChar = chr[0];
							Report.BuildRange( ref strBlue, ref blueList, sheetRow, asciiChar, 0 );
						}
						// Test for Treatment selected as Committed Project (RED)
						else if( dr["Iscommitted"].ToString().ToUpper() == "TRUE" )
						{
							string s = Report.GetColumnLetter( nCol + 1 );
							char[] chr = s.ToCharArray( 0, 1 );
							char asciiChar = chr[0];
							Report.BuildRange( ref strRed, ref redList, sheetRow, asciiChar, 0 );
							oData[ndx, nCol] = dr["Treatment"].ToString();
						}

						break;
					case "ORACLE":

						// Test for Treatment selected per RoadCare logic (GREEN)
						if( dr["Treatment"].ToString() != "No Treatment" && dr["Iscommitted"].ToString() != "1" && dr["Numbertreatment"].ToString() != "0" )
						{
							string s = Report.GetColumnLetter( nCol + 1 );
							char[] chr = s.ToCharArray( 0, 1 );
							char asciiChar = chr[0];

							Report.BuildRange( ref strGreen, ref greenList, sheetRow, asciiChar, 0 );
							oData[ndx, nCol] = dr["Treatment"].ToString();
						}
						// Test for Feasible treatment not funded/selected (BLUE)
						else if( dr["Treatment"].ToString() == "No Treatment" && dr["Iscommitted"].ToString() != "1" && dr["Numbertreatment"].ToString() != "0" )
						{
							string s = Report.GetColumnLetter( nCol + 1 );
							char[] chr = s.ToCharArray( 0, 1 );
							char asciiChar = chr[0];
							Report.BuildRange( ref strBlue, ref blueList, sheetRow, asciiChar, 0 );
						}
						// Test for Treatment selected as Committed Project (RED)
						else if( dr["Iscommitted"].ToString() == "1" )
						{
							string s = Report.GetColumnLetter( nCol + 1 );
							char[] chr = s.ToCharArray( 0, 1 );
							char asciiChar = chr[0];
							Report.BuildRange( ref strRed, ref redList, sheetRow, asciiChar, 0 );
							oData[ndx, nCol] = dr["Treatment"].ToString();
						}

						break;
					default:
						throw new NotImplementedException( "TODO: develop ANSI version of CreateDetailedResultsReport()" );
						//break;
				}

                nCol++;
                if (rowsToColumns + 1 >= numYears)
                {
                    rowsToColumns = 0;
                    ndx++;
                    sheetRow++;
                }
                else
                {
                    rowsToColumns++;
                }
            }

            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, true);
            #endregion

            #region format pageheader
            // PAGEHEADER
            string strRange = "B1:" + Report.GetColumnLetter(aryCols) + "1";
            DataRow drPgHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            strRange = "B2:" + Report.GetColumnLetter(aryCols) + "2";
            drPgHdr = dsPage.Tables[0].Rows[1];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            strRange = "B3:" + Report.GetColumnLetter(aryCols) + "3";
            drPgHdr = dsPage.Tables[0].Rows[2];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);

            // Place a color legend at top and bottom of report
            PlaceColorLegend(oSheet, aryCols, 3);
            PlaceColorLegend(oSheet, 1, sheetRow + 1);
            #endregion

            #region format groupheader
            strRange = "A5:" + Report.GetColumnLetter(aryCols) + "5";
            Report.FormatHeaders(oR, drPage, oSheet, "gh", strRange);
            #endregion

            #region format columnHeader
            strRange = "A6:" + Report.GetColumnLetter(aryCols) + "6";
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);
            #endregion

            #region apply color
            Report.EndRangeList(ref strGreen, ref greenList);
            Report.EndRangeList(ref strBlue, ref blueList);
            Report.EndRangeList(ref strRed, ref redList);
            foreach (string s in greenList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightGreen);
            }

            foreach (string s in blueList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightBlue);
            }

            foreach (string s in redList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightSalmon);
            }

            #endregion

            #region default column widths
            // set some column widths
            oR = oSheet.get_Range("A1:B1", Missing.Value);
            oR.ColumnWidth = 15;
            strRange = "C1:" + Report.GetColumnLetter(aryCols) + "1";
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.ColumnWidth = 12;
            #endregion

            #region make bold
            strRange = "A7:B" + (sheetRow - 1).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.Font.Bold = true;
            #endregion

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
        }

        private void PlaceColorLegend(_Worksheet oSheet, int startingCol, int startingRow)
        {
            string strRange = Report.GetColumnLetter(startingCol + 2) + startingRow.ToString() + ":" + Report.GetColumnLetter(startingCol + 5) + startingRow.ToString();;
            Range oR = oSheet.get_Range(strRange, Missing.Value);
            oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightGray);
            oR.MergeCells = true;
            oR.Value2 = "Legend";
            oR.Font.Size = 11;
            oR.Font.Bold = true;
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;
            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

            strRange = Report.GetColumnLetter(startingCol + 2) + (startingRow + 1).ToString() + ":" + Report.GetColumnLetter(startingCol + 5) + (startingRow + 1).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightBlue);
            oR.MergeCells = true;
            oR.Value2 = "Feasible treatment not funded/selected";
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;
            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

            strRange = Report.GetColumnLetter(startingCol + 2) + (startingRow + 2).ToString() + ":" + Report.GetColumnLetter(startingCol + 5) + (startingRow + 2).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightGreen);
            oR.MergeCells = true;
            oR.Value2 = "Treatment selected per RoadCare logic";
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;
            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
            
            strRange = Report.GetColumnLetter(startingCol + 2) + (startingRow + 3).ToString() + ":" + Report.GetColumnLetter(startingCol + 5) + (startingRow + 3).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.LightSalmon);
            oR.MergeCells = true;
            oR.Value2 = "Treatment selected as Committed Project";
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;
            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

            //strRange = Report.GetColumnLetter(startingCol + 2) + startingRow.ToString() + ":" + Report.GetColumnLetter(startingCol + 5) + (startingRow + 3).ToString();
            //oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            //oR.Borders.Weight = XlBorderWeight.xlThin;
            //oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

        }

    }   // end class
}   // end namespace
