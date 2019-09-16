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
    public class BudgetPerDistrictReport
    {
        string m_strNetworkID, m_strSimulationID, m_strNetwork, m_strSimulation, m_strBudgetPer;

        public BudgetPerDistrictReport(string strNetworkID, string strSimulationID, string strNetwork, string strSimulation, string strBudgetPer)
        {
            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            strBudgetPer = strBudgetPer.ToLower();
            m_strBudgetPer = char.ToUpper(strBudgetPer[0]) + strBudgetPer.Substring(1);
        }

        public void CreateBudgetPerReport()
        {
            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            Report.SheetPageSetup(oSheet, "Budget Per " + m_strBudgetPer, 50d, 20d, 10d, m_strNetwork + " - " + m_strSimulation, DateTime.Now.ToLongDateString(), "Page &P", 1);
            Range oR = oSheet.get_Range("A1", "A1");

            object oEndCell = new object();
            string strFilter = "";
            string strReportName = "Budget Per District Report";    // keep this constant to avoid having to add
                                                                    // a row to the Reports Table for each permutation 
                                                                    // of this "Budget Per [attribute]" style report

            //  QUICK TEST of COUNTY attribute...
            //m_strBudgetPer = "County";
            //oSheet.Name = "Budget Per " + m_strBudgetPer;

            int sheetRow;
            int ndx;
            DataSet dsPage = null, dsSimulations = null;

            try
            {
                dsPage = DBOp.QueryPageHeader(strReportName);
                dsSimulations = DBOp.QuerySimulations(m_strSimulationID);

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
            if (strMajorTitle.IndexOf("@1") > 0) strMajorTitle = strMajorTitle.Replace("@1", m_strBudgetPer);   // stuff the attribute into title

            #region default column widths
            // set some column widths
            oR = oSheet.get_Range("A1:A1", Missing.Value);
            oR.ColumnWidth = 18;
            #endregion

            #region place agency graphic
            //string strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + drPage["reportGraphicFile"].ToString();
			string strPath =  ".\\" + drPage["reportGraphicFile"].ToString();
            Report.PlaceReportGraphic(strPath, oSheet.get_Range("A1", Missing.Value), oSheet);
            #endregion

            #region write Major Title
            int aryCols = 2;
            int aryRows = 3;
            object[,] oData = new object[aryRows, aryCols];
            Report.ClearDataArray(ref oData);

            oData[0, 1] = strMajorTitle;
            sheetRow = 4;

            oEndCell = "A1";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
            #endregion

            #region get Budget Order
            DataSet dsInvestments = DBOp.QueryInvestments(m_strSimulationID);   // get the budgetorder
            string[] strBudgetOrder = dsInvestments.Tables[0].Rows[0].ItemArray[5].ToString().Split(',');
            #endregion

            #region get Budget Years
			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					strFilter = "SELECT DISTINCT [Year_] FROM YearlyInvestment WHERE simulationID = " + m_strSimulationID + "ORDER BY [Year_]";
					break;
				case "ORACLE":
					strFilter = "SELECT DISTINCT Year_ FROM YearlyInvestment WHERE simulationID = " + m_strSimulationID + "ORDER BY Year_";
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for CreateBudgetPerReport()");
					//break;
			}
            DataSet dsBudgetYears = DBMgr.ExecuteQuery(strFilter);
            int numYears = dsBudgetYears.Tables[0].Rows.Count;
            #endregion

            #region get Number of Districts
            List<string> strJuris = DBOp.GetJurisdiction(m_strNetworkID, m_strBudgetPer);
            strJuris.Sort();
            int numDistricts = strJuris.Count;
            #endregion

            aryCols = numYears + 1;
            aryRows = numDistricts + 3;

            #region build Column Headers array
            // Set up column header once, for multiple uses
            object[] oColumnHeader = new object[aryCols];
            oColumnHeader[0] = m_strBudgetPer;
            ndx = 1;
            foreach(DataRow dr in dsBudgetYears.Tables[0].Rows)
            {
                oColumnHeader[ndx++] = dr["Year_"].ToString();
            }
            #endregion

            #region build Budget tables
            object[,] oTotalBudget = new object[aryRows, aryCols]; // Annual Total Budget array
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            int totalRow = oData.GetUpperBound(0);
            string strRange, strGroups, strColHdrs, strTotals, strGrids, strMoney;
            strColHdrs = strGrids = strGroups = strMoney = strRange = strTotals = "";

            // ranges needed for formatting spreadsheet
            List<string> groupList = new List<string>();
            List<string> columnList = new List<string>();
            List<string> totalsList = new List<string>();
            List<string> gridsList = new List<string>();
            List<string> moneyList = new List<string>();

            Report.ClearDataArray(ref oTotalBudget);
            sheetRow = 4;
            int nCol, nGridStart, nGridEnd;
            double nTmp = 0d;
            foreach (string strBudget in strBudgetOrder)
            {
                oEndCell = "A" + sheetRow.ToString();
                Report.BuildRange(ref strGroups, ref groupList, sheetRow, 'A', aryCols - 1);

                Report.ClearDataArray(ref oData);

                ndx = 0;
                oData[ndx, 0] = "Annual " + strBudget + " Budget"; // Group Header
                oTotalBudget[ndx, 0] = "Annual Total Budget";
                sheetRow++;
                ndx++;
                oData[totalRow, 0] = oTotalBudget[totalRow, 0] = "Total";
                Report.BuildRange(ref strColHdrs, ref columnList, sheetRow, 'A', aryCols - 1);
                nGridStart = sheetRow;
                
                for (int i = 0; i < aryCols; i++)
                {
                    oData[ndx, i] = oColumnHeader[i];     // Column Header
                    oTotalBudget[ndx, i] = oColumnHeader[i];
                }
                sheetRow++;
                ndx++;

                foreach(string strDistrict in strJuris) {
                    nCol = 1;
                    oData[ndx, 0] = oTotalBudget[ndx,0] = strDistrict;
                    foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
                    {
                        Hashtable hBudgets = DBOp.GetBudgetTotals(m_strNetworkID, m_strSimulationID, dr["Year_"].ToString(), strBudget, m_strBudgetPer, m_strBudgetPer + " = " + strDistrict);
                        string strHash = (string)hBudgets[strDistrict];
                        double nBudget = Convert.ToDouble(strHash);
                        if ( oTotalBudget[ndx, nCol].ToString() == "" ) oTotalBudget[ndx, nCol] = 0d;    // initialize array element
                        if (oData[totalRow, nCol].ToString() == "") oData[totalRow, nCol] = 0d;        // initalize array element
                        if (oTotalBudget[totalRow, nCol].ToString() == "") oTotalBudget[totalRow, nCol] = 0d;        // initalize array element
                        
                        // Accumulate total budget array
                        nTmp = (double) oTotalBudget[ndx, nCol];
                        nTmp += nBudget;
                        oTotalBudget[ndx, nCol] = nTmp;

                        nTmp = (double)oTotalBudget[totalRow, nCol];
                        nTmp += nBudget;
                        oTotalBudget[totalRow, nCol] = nTmp;  // accumulate fiscal year budget
                        
                        nTmp = (double)oData[totalRow, nCol];
                        nTmp += nBudget;
                        oData[totalRow, nCol] = nTmp;  // accumulate fiscal year budget
                        oData[ndx, nCol++] = nBudget;   // store budget value
                    }
                    ndx++;
                    sheetRow++;
                }
                Report.BuildRange(ref strTotals, ref totalsList, sheetRow, 'A', aryCols - 1);
                strGrids = "A" + nGridStart.ToString() + ":" + Report.GetColumnLetter(aryCols) + sheetRow.ToString();
                gridsList.Add(strGrids);
                strMoney = "B" + (nGridStart + 1).ToString() + ":" + Report.GetColumnLetter(aryCols) + sheetRow.ToString();
                moneyList.Add(strMoney);
                sheetRow += 2;
                oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);

            }

            // print the annual total budget array
            oEndCell = "A" + sheetRow.ToString();
            Report.BuildRange(ref strGroups, ref groupList, sheetRow, 'A', aryCols - 1);
            Report.BuildRange(ref strColHdrs, ref columnList, sheetRow + 1, 'A', aryCols - 1);
            Report.BuildRange(ref strTotals, ref totalsList, sheetRow + aryRows - 1, 'A', aryCols - 1);
            nGridStart = sheetRow + 1;
            nGridEnd = sheetRow + aryRows - 1;
            strGrids = "A" + nGridStart.ToString() + ":" + Report.GetColumnLetter(aryCols) + nGridEnd.ToString();
            gridsList.Add(strGrids);
            strMoney = "B" + (nGridStart + 1).ToString() + ":" + Report.GetColumnLetter(aryCols) + nGridEnd.ToString();
            moneyList.Add(strMoney);
            
            oEndCell = Report.WriteObjectArrayToExcel(oTotalBudget, oSheet, oEndCell, false, false);
            Report.EndRangeList(ref strGroups, ref groupList);
            Report.EndRangeList(ref strColHdrs, ref columnList);
            Report.EndRangeList(ref strTotals, ref totalsList);
            #endregion

            #region format pageheader
            // PAGEHEADER
            strRange = "B1:" + Report.GetColumnLetter(aryCols) + "1";
            DataRow drPgHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            #endregion

            #region format groupheader
            foreach (string s in groupList)
            {
                Report.FormatHeaders(oR, drPage, oSheet, "gh", s);
                //oR.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
                //oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                //oR.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                //oR.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            }
            #endregion

            #region format columnheader
            foreach (string s in columnList)
            {
                Report.FormatHeaders(oR, drPage, oSheet, "ch", s);
            }
            #endregion

            #region format totals rows
            foreach (string s in totalsList)
            {
                Report.FormatHeaders(oR, drPage, oSheet, "ch", s);
            }
            #endregion

            #region format grid data
            foreach (string s in gridsList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oR.Borders.LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.Weight = XlBorderWeight.xlThin;
                oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
                oR.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;

            }
            #endregion

            #region format Money cells
            foreach(string s in moneyList)
            {
                oR = oSheet.get_Range(s, Missing.Value);
                oR.NumberFormat = "$#,##0";
                oR.ColumnWidth = 15;
            }
            #endregion

            #region create column charts
            int top = 15;
            //bool bHasLegend = true;
            //string strCategoryAxis = "Fiscal Year";

            string strTitle = "";
            string sheetName;
            int budgetIndex = 0;
            Range oSourceData = oSheet.get_Range(moneyList[0], Missing.Value);
            int left = (int)Report.GetColumnWidthInPixels(oSourceData, oSheet);
            foreach (string s in moneyList)
            {
                int nPos = s.IndexOf(":");
                string sTmp = Report.Left(s, nPos + 2);
                
                string sRight = s.Substring(nPos+2);
                nTmp = int.Parse(sRight) - 1;
                oSourceData = oSheet.get_Range((sTmp + nTmp.ToString()), Missing.Value);

                sheetName = "='Budget Per " + m_strBudgetPer + "'!$B$5:$" + Report.GetColumnLetter(aryCols) + "$5";
                if (budgetIndex < strBudgetOrder.Count())
                    strTitle = "Annual " + strBudgetOrder[budgetIndex++] + " Budget Per " + m_strBudgetPer;
                else
                    strTitle = "Annual Total Budget Per " + m_strBudgetPer;

                Report.CreateColClusterBarGraph(left, top, 425, 315, oSheet, oSourceData, sheetName,
                  strTitle, 12, "Fiscal Year", 11, strJuris, "", 11, XlRowCol.xlRows);

                top += 330; // magic number

            }   // end moneyList
            #endregion

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
        }   // end CreateBudgetPerReport
    }   // end class
}   // end namespace
