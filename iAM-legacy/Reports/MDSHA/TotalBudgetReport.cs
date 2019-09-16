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
    public class TotalBudgetReport
    {
        string m_strNetworkID, m_strSimulationID, m_strNetwork, m_strSimulation;
        bool m_bDebug = false;

        public TotalBudgetReport(string strNetworkID, string strSimulationID, string strNetwork, string strSimulation)
        {
            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
        }

        public void CreateTotalBudgetReport()
        {
            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            Report.SheetPageSetup(oSheet, "Total Budget", 50d, 20d, 10d, m_strNetwork + " - " + m_strSimulation, DateTime.Now.ToLongDateString(), "Page &P", 1);
            Range oR = oSheet.get_Range("A1", "A1");

            object oEndCell = new object();
            string strReportName = "Total Budget Report", strFilter = "";
            int sheetRow;
            int ndx;
            DataSet dsPage = null, dsSimulations = null, dsInvestments = null;

            try
            {
                dsPage = DBOp.QueryPageHeader(strReportName);
                dsSimulations = DBOp.QuerySimulations(m_strSimulationID);
                dsInvestments = DBOp.QueryInvestments(m_strSimulationID);   // get the budgetorder

            }
            catch (Exception e)
            {
                throw e;
            }
            if (m_bDebug) WriteLogEntry("DataSet OK");
            Cursor c = Cursor.Current;
            Cursor.Current = new Cursor(Cursors.WaitCursor.Handle);

            DataRow drPage = dsPage.Tables[0].Rows[0];
            string strMajorTitle = drPage["phText"].ToString();
            string[] strBudgetOrder = dsInvestments.Tables[0].Rows[0].ItemArray[5].ToString().Split(',');

            #region default column widths
            // set some column widths
            oR = oSheet.get_Range("A1:A1", Missing.Value);
            oR.ColumnWidth = 18;
            #endregion

            if (m_bDebug) WriteLogEntry("Default column widths OK");

            #region place agency graphic
            //string strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + drPage["reportGraphicFile"].ToString();
			string strPath = ".\\" + drPage["reportGraphicFile"].ToString();
            Report.PlaceReportGraphic(strPath, oSheet.get_Range("A1", Missing.Value), oSheet);
            #endregion

            if (m_bDebug) WriteLogEntry("Place agency graphic OK");

            int aryCols = 2;
            int aryRows = 3;

            object[,] oData = new object[aryRows, aryCols];
            Report.ClearDataArray(ref oData);

            oData[0, 1] = strMajorTitle;
            
            oEndCell = "A1";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);

            #region budget
            int nFY = 1, nBudgetTotal = 1;
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					aryRows = DBMgr.ExecuteScalar("SELECT Count(DISTINCT [Year_]) FROM YearlyInvestment WHERE simulationID = " + m_strSimulationID) + 2;
					break;
				case "ORACLE":
					aryRows = DBMgr.ExecuteScalar("SELECT Count(DISTINCT Year_) FROM YearlyInvestment WHERE simulationID = " + m_strSimulationID) + 2;
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for CreateTotalBudgetReport()");
					//break;
			}
            aryCols = strBudgetOrder.Count() + nFY + nBudgetTotal;


            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);

            oData[0, 0] = "Total Budget - in Millions";
            oData[1,0] = "FY";
            sheetRow = 6;
            ndx = 1;
            foreach (string str in strBudgetOrder)
            {
                oData[1, ndx++] = str;
            }
            oData[1, ndx] = "Total";    // add total column
            ndx = 2;
            DataSet dsYearlyInvestment = DBOp.QueryYearlyInvestment(m_strSimulationID);
            strFilter = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE SimulationID = "+m_strSimulationID;
            DataSet dsYearlyYears = DBMgr.ExecuteQuery(strFilter);
            int nCol = 0;
            double budgetTotal;
            foreach (DataRow dr in dsYearlyYears.Tables[0].Rows)
            {
                oData[ndx, 0] = dr["Year_"].ToString();
                nCol = 1;
                budgetTotal = 0;
                foreach (string str in strBudgetOrder)
                {
                    strFilter = "Year_ = " + dr["Year_"].ToString() + " AND Budgetname = '" + str + "'";
                    foreach (DataRow dr1 in dsYearlyInvestment.Tables[0].Select(strFilter))
                    {
                        oData[ndx, nCol] = Convert.ToDouble(dr1["Amount"].ToString()) / 1000000;    // show millions
                        budgetTotal += double.Parse(dr1["Amount"].ToString()); // sum the budgets for a fiscal year
                    }
                    nCol++;
                }
                oData[ndx, nCol] = budgetTotal / 1000000;
                ndx++;
                sheetRow++;
            }

            oEndCell = "A4";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, true);
            #endregion

            if (m_bDebug) WriteLogEntry("Budget OK");

            #region format pageheader
            // PAGEHEADER
            string strRange = "B1:G1";
            DataRow drPgHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            #endregion

            if (m_bDebug) WriteLogEntry("Format pageheader OK");

            #region format groupheader
            strRange = "A4:" + Report.GetColumnLetter(aryCols) + "4";
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.MergeCells = true;
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
            oR.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
            oR.Font.Size = 11;
            oR.Font.Bold = true;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            #endregion

            if (m_bDebug) WriteLogEntry("Format groupheader OK");

            #region format columnheader
            strRange = "A5:" + Report.GetColumnLetter(aryCols) + "5";
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oR.VerticalAlignment = XlVAlign.xlVAlignBottom;
            if (m_bDebug) WriteLogEntry("Format columnheader VerticalAlignment OK");
            //oR.WrapText = true;
            oR.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
            oR.Font.Size = 11;
            oR.Font.Bold = true;

            oR.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            if (m_bDebug) WriteLogEntry("Format columnheader EdgeTop OK");
            oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            if (m_bDebug) WriteLogEntry("Format columnheader EdgeBottom OK");

            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

            //oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
            //if (m_bDebug) WriteLogEntry("Format columnheader InsideHorizontal OK");
            //oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            //if (m_bDebug) WriteLogEntry("Format columnheader InsideVertical OK");

            #endregion

            if (m_bDebug) WriteLogEntry("Format columnheader OK");

            #region format grid data
            strRange = "A6:A" + (sheetRow - 1).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;
            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
            //oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
            //oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;

            strRange = "B6:" + Report.GetColumnLetter(aryCols) + (sheetRow - 1).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.NumberFormat = "$#,##0.0";
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;

            oR.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
            
            //oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
            //oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            oR.ColumnWidth = 13.4;
            #endregion

            if (m_bDebug) WriteLogEntry("Format grid data OK");

            #region create column chart

            strRange = "B6:" + Report.GetColumnLetter(aryCols) + (sheetRow - 1).ToString();
            Range oSourceData = oSheet.get_Range(strRange, Missing.Value);      //B5:E10
            int left = (int)Report.GetColumnWidthInPixels(oSourceData, oSheet);

            string strTitle = "Yearly Budget Distribution ($ Millions)";
            string sheetName = "='Total Budget'!$A$6:$A$" + (sheetRow - 1).ToString();

            List<string> listBudgets = new List<string>();
            foreach (string s in strBudgetOrder)
            {
               listBudgets.Add(s);
            }
            listBudgets.Add("Total");

            Report.CreateColClusterBarGraph(left, 30, 425, 315, oSheet, oSourceData, sheetName,
              strTitle, 12, "Fiscal Year", 11, listBudgets, "", 11, XlRowCol.xlColumns);

            #endregion

            if (m_bDebug) WriteLogEntry("Create column chart OK");

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
        }

        public void WriteLogEntry(string msg)
        {
            StreamWriter wd;
            try
            {
                wd = new StreamWriter(@"c:\RoadcareReportLog.txt", true);
                wd.BaseStream.Seek(0, SeekOrigin.End);
                DateTime dt = DateTime.Now;
                string str = string.Format("{0:d2}-{1:d2}-{2:d4} {3:d2}:{4:d2}:{5:d2} {6}.", dt.Month, dt.Day, dt.Year, dt.Hour, dt.Minute, dt.Second, msg);
                wd.WriteLine(str);
                wd.WriteLine();
                wd.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }   // end class
}   // end namespace
