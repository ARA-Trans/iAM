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
    public class TotalLaneMilesPerConditionReport
    {
        string m_strNetworkID, m_strSimulationID, m_strNetwork, m_strSimulation, m_strBudgetPer;
        bool m_bUseLaneMiles;
        public TotalLaneMilesPerConditionReport(string strNetworkID, string strSimulationID, string strNetwork, string strSimulation, string strBudgetPer,bool useLaneMiles)
        {
            string str = strBudgetPer;

            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            str = strBudgetPer.ToLower();
            m_strBudgetPer = char.ToUpper(str[0]) + str.Substring(1);
            if (strBudgetPer == "CONDITION_IRI")
            {
                m_strBudgetPer = m_strBudgetPer.Substring(0, 10) + "IRI";
            }
            m_bUseLaneMiles = useLaneMiles;
        }

        public void CreateTotalLaneMilesPerConditionReport()
        {
            //Test();
            //return;

            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            string strReportName = "Total Lane-Miles Per Condition Report";
            object oEndCell = new object();
            string strFilter = "";

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

            if(m_bUseLaneMiles) Report.SheetPageSetup(oSheet, "Total Lane-Miles Per Condition", 50d, 20d, 10d, m_strNetwork + " - " + m_strSimulation, DateTime.Now.ToLongDateString(), "Page &P", 1);
            else Report.SheetPageSetup(oSheet, "Total VLM Per Condition", 50d, 20d, 10d, m_strNetwork + " - " + m_strSimulation, DateTime.Now.ToLongDateString(), "Page &P", 1);
            Range oR = oSheet.get_Range("A1", "A1");



            Cursor c = Cursor.Current;
            Cursor.Current = new Cursor(Cursors.WaitCursor.Handle);

            DataRow drPage = dsPage.Tables[0].Rows[0];
            string strMajorTitle = drPage["phText"].ToString();
            if (strMajorTitle.IndexOf("@1") > 0) strMajorTitle = strMajorTitle.Replace("@1", m_strBudgetPer);   // stuff the attribute into title
            if (!m_bUseLaneMiles) strMajorTitle =  strMajorTitle.Replace("Lane-Miles","Vehicle Lane-Miles");

            #region default column widths
            // set some column widths
            oR = oSheet.get_Range("A1:A1", Missing.Value);
            oR.ColumnWidth = 18;
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
            sheetRow = 4;

            oEndCell = "A1";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
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
					throw new NotImplementedException("TODO: Create ANSI implementation for CreateTotalLaneMilesPerConditionReport()");
					//break;
			}
            DataSet dsBudgetYears = DBMgr.ExecuteQuery(strFilter);
            int numYears = dsBudgetYears.Tables[0].Rows.Count;
            #endregion

            int numConditions = 5, nTables = 2, rowSpacer = 2;
            aryCols = numYears + 1;
            aryRows = nTables * (numConditions + 3) + rowSpacer;

            #region build Column Headers array
            // Set up column header once, for multiple uses
            object[] oColumnHeader = new object[aryCols];
            oColumnHeader[0] = "Condition"; // m_strBudgetPer;
            ndx = 1;
            foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
            {
                oColumnHeader[ndx++] = dr["Year_"].ToString();
            }
            #endregion

            #region build Condition List
            // REVIST THIS REGION.  BUILD LIST FROM THE DATABASE WHEN POSSIBLE  xyzzy
            List<string> listCondition = new List<string>();
            listCondition.Add("Very Good");
            listCondition.Add("Good");
            listCondition.Add("Fair");
            listCondition.Add("Mediocre");
            listCondition.Add("Poor");
            #endregion

            #region build Budget and Percentage tables
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);

            int totalRow, nCol, budgetStartRowNdx, rowCounter = 0, acceptableRow;
            Hashtable laneMilesPerCondition;
            double nTmp, nTmp1;//, nAcceptible, nUnacceptible;
            ndx = 0;
            if(m_bUseLaneMiles) oData[ndx, 0] = "Total Lane-miles per Condition Category"; // Group Header
            else oData[ndx, 0] = "Total Vehicle Lane-miles per Condition Category"; // Group Header
            sheetRow = 4;
            totalRow = 2 + numConditions;
            acceptableRow = oData.GetUpperBound(0) - 1;
            oEndCell = "A" + sheetRow.ToString();

            sheetRow++;
            ndx++;
            oData[totalRow, 0] = "Total";

            for (int i = 0; i < aryCols; i++)
            {
                oData[ndx, i] = oColumnHeader[i];     // Column Header
            }

            sheetRow++;
            ndx++;
            budgetStartRowNdx = 2;   // percentRowNdx is used to pull a value out of oData then use it to calculate the percent table
            List<string> listAttributes;
            string strCriteria;
            string strLeftSide = "", strConnector = "", strRightSide = "";
            double nBudget = 0d;
            foreach (string s in listCondition)
            {
                oData[ndx, 0] = s;
                nCol = 1;
                foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
                {
                    ConditionCriteria(s, ref strLeftSide, ref strConnector, ref strRightSide);
                    strCriteria = m_strBudgetPer + "_" + dr["Year_"].ToString() + strLeftSide;
                    if(strConnector != "")
                        strCriteria += strConnector + m_strBudgetPer + "_" + dr["Year_"].ToString() + strRightSide;
                    strFilter = " YEARS = " + dr["Year_"].ToString() + " AND " + strCriteria;     //"BUDGET IS NOT NULL AND YEARS = " + dr["Year"].ToString();
                    //strFilter = " YEARS = " + dr["Year"].ToString();     //"BUDGET IS NOT NULL AND YEARS = " + dr["Year"].ToString();
                    laneMilesPerCondition = DBOp.GetPercentagePerStringAttribute(m_strNetworkID, m_strSimulationID, m_strBudgetPer.ToUpper(), dr["Year_"].ToString(), "AREA", strFilter,m_bUseLaneMiles, out listAttributes);
                    nBudget = 0d;
                    foreach (DictionaryEntry de in laneMilesPerCondition)
                    {
                        string entry = (string)de.Value;
                        if (entry != "")
                            nBudget += Convert.ToDouble(entry);
                    }
                    //string strHash = (string)laneMilesPerCondition[s.ToUpper()];
                    //double nBudget = Convert.ToDouble(strHash);
                    if (oData[totalRow, nCol].ToString() == "") oData[totalRow, nCol] = 0d;        // initalize array element

                    nTmp = (double)oData[totalRow, nCol];
                    nTmp += nBudget;
                    oData[totalRow, nCol] = nTmp;
                    oData[ndx, nCol++] = nBudget;
                }
                ndx++;
                sheetRow++;
            }

            sheetRow += 2;
            ndx += 2;

            if(m_bUseLaneMiles) oData[ndx, 0] = "Total Lane-Mile Years Distribution (%) per Condition Category";
            else oData[ndx, 0] = "Total Vehicle Lane-Mile Years Distribution (%) per Condition Category";
            ndx++;
            sheetRow++;

            for (int i = 0; i < aryCols; i++)
            {
                oData[ndx, i] = oColumnHeader[i];     // Column Header
            }

            sheetRow++;
            ndx++;
            rowCounter = 1;
            foreach (string s in listCondition)
            {
                oData[ndx, 0] = s;
                nCol = 1;
                foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
                {
                    nTmp = (double)oData[budgetStartRowNdx, nCol];
                    nTmp1 = (double)oData[totalRow, nCol];
                    double nPercent = nTmp / nTmp1;
                    oData[ndx, nCol] = nPercent;
                    if (rowCounter <= 3)
                    {   // sum the % Acceptable
                        if (oData[acceptableRow, nCol].ToString() == "") oData[acceptableRow, nCol] = 0d;        // initalize array element
                        nTmp = (double)oData[acceptableRow, nCol];
                        nTmp += nPercent;
                        oData[acceptableRow, nCol] = nTmp;
                    }
                    else
                    {   // sum the % Unacceptable
                        if (oData[acceptableRow + 1, nCol].ToString() == "") oData[acceptableRow + 1, nCol] = 0d;        // initalize array element
                        nTmp = (double)oData[acceptableRow + 1, nCol];
                        nTmp += nPercent;
                        oData[acceptableRow + 1, nCol] = nTmp;
                    }
                    nCol++;
                }
                ndx++;
                sheetRow++;
                budgetStartRowNdx++;
                rowCounter++;
            }
            oData[acceptableRow, 0] = "%Acceptable";
            sheetRow++;
            oData[acceptableRow + 1, 0] = "%Unacceptable";


            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
            #endregion

            #region format pageheader
            // PAGEHEADER
            string strRange = "B1:" + Report.GetColumnLetter(aryCols) + "1";
            DataRow drPgHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            #endregion

            #region format groupheader
            int startingRow = 4;
            int offSet = startingRow + numConditions + 2 + rowSpacer;
            strRange = "A4:" + Report.GetColumnLetter(aryCols) + "4";
            Report.FormatHeaders(oR, drPage, oSheet, "gh", strRange);
            strRange = "A" + offSet.ToString() + ":" + Report.GetColumnLetter(aryCols) + offSet.ToString();
            Report.FormatHeaders(oR, drPage, oSheet, "gh", strRange);
            #endregion

            #region format columnHeader
            strRange = "A5:" + Report.GetColumnLetter(aryCols) + "5" + ", A" + (offSet + 1).ToString() + ":" + Report.GetColumnLetter(aryCols) + (offSet + 1).ToString();
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);
            #endregion

            #region format totals row
            strRange = "A" + (totalRow + 4).ToString() + ":" + Report.GetColumnLetter(aryCols) + (totalRow + 4).ToString();
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);

            strRange = "A" + (acceptableRow + 4).ToString() + ":" + Report.GetColumnLetter(aryCols) + (acceptableRow + 4).ToString();
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);
            strRange = "A" + (acceptableRow + 5).ToString() + ":" + Report.GetColumnLetter(aryCols) + (acceptableRow + 5).ToString();
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);
            #endregion

            #region format grid data
            strRange = "A5:" + Report.GetColumnLetter(aryCols) + (totalRow + 4).ToString();
            strRange += ",A" + (acceptableRow - numConditions + startingRow - 1).ToString() + ":" + Report.GetColumnLetter(aryCols) + (acceptableRow + startingRow + 1).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oR.Borders.LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.Weight = XlBorderWeight.xlThin;
            oR.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            oR.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            #endregion

            #region format Lane Mile cells
            strRange = "B6:" + Report.GetColumnLetter(aryCols) + (totalRow + 4).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.NumberFormat = "#,###.0";
            oR.ColumnWidth = 15;
            #endregion

            #region format Percent cells
            strRange = "B" + (acceptableRow - numConditions + startingRow).ToString() + ":" + Report.GetColumnLetter(aryCols) + (acceptableRow + startingRow + 1).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.NumberFormat = "0.0%";
            #endregion

            #region create column charts
            strRange = "B6:" + Report.GetColumnLetter(aryCols) + (totalRow + startingRow - 1).ToString();

            Range oSourceData = oSheet.get_Range(strRange, Missing.Value);
            int left = (int)Report.GetColumnWidthInPixels(oSourceData, oSheet);

            if(m_bUseLaneMiles) Report.CreateColClusterBarGraph(left, 15, 425, 315, oSheet, oSourceData, "='Total Lane-Miles Per Condition'!$B$5:$" + Report.GetColumnLetter(aryCols) + "$5",
                "Annual Total Lane-Miles Per Condition", 12, "Fiscal Year", 11, listCondition, "", 11, XlRowCol.xlRows);
            else
                Report.CreateColClusterBarGraph(left, 15, 425, 315, oSheet, oSourceData, "='Total VLM Per Condition'!$B$5:$" + Report.GetColumnLetter(aryCols) + "$5",
                "Annual Total Vehicle Lane-Miles Per Condition", 12, "Fiscal Year", 11, listCondition, "", 11, XlRowCol.xlRows);
            #endregion

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
        }

        private void ConditionCriteria(string strCondition, ref string strLeftSide, ref string strConnector, ref string strRightSide)
        {
            strCondition = strCondition.ToUpper();

            if (strCondition == "VERY GOOD")
            {
                strLeftSide = " > 90 ";
                strConnector = "";
                strRightSide = "";
            }
            else if (strCondition == "GOOD")
            {
                strLeftSide = " > 80 ";
                strConnector = " AND ";
                strRightSide = " <= 90 ";
            }
            else if (strCondition == "FAIR")
            {
                strLeftSide = " >= 65 ";
                strConnector = " AND ";
                strRightSide = " <= 80 ";
            }
            else if (strCondition == "MEDIOCRE")
            {
                strLeftSide = " >= 50 ";
                strConnector = " AND ";
                strRightSide = " < 65 ";
            }
            else if (strCondition == "POOR")
            {
                strLeftSide = " < 50 ";
                strConnector = "";
                strRightSide = "";
            }
            return;
        }

        private void Test()
        {
            Report.XL.Visible = true;
            Report.XL.UserControl = true;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;


            int nRows = 4;
            int nColumns = 4;
            string upperLeftCell = "B3";
            int endRowNumber = System.Int32.Parse(upperLeftCell.Substring(1))
                + nRows - 1;
            char endColumnLetter = System.Convert.ToChar(
                Convert.ToInt32(upperLeftCell[0]) + nColumns - 1);
            string upperRightCell = System.String.Format("{0}{1}",
                endColumnLetter, System.Int32.Parse(upperLeftCell.Substring(1)));
            string lowerRightCell = System.String.Format("{0}{1}",
                endColumnLetter, endRowNumber);
            
            // Now create the chart.
            ChartObjects chartObjs = (ChartObjects)oSheet.ChartObjects(Type.Missing);
            ChartObject chartObj = chartObjs.Add(100, 120, 300, 300);
            Chart xlChart = chartObj.Chart;

            oSheet.Cells[1, 1] = "Data for chart";

            object[,] oData = new object[4, 4];
            oData[0, 0] = "Year";
            oData[0, 1] = "Construction";
            oData[0, 2] = "Maintenance";
            oData[0, 3] = "Rehabilitation";

            oData[1, 0] = "2008";
            oData[1, 1] = 13400000;
            oData[1, 2] = 25500000;
            oData[1, 3] = 49900000;

            oData[2, 0] = "2009";
            oData[2, 1] = 10400000;
            oData[2, 2] = 45500000;
            oData[2, 3] = 14500000;

            oData[3, 0] = "2010";
            oData[3, 1] = 8400000;
            oData[3, 2] = 75500000;
            oData[3, 3] = 44500000;

            Range rg = oSheet.get_Range("B3", "E6");
            //rg.Value2 = AddData(nRows, nColumns);
            rg.Value2 = oData;

            // Data points
            Range chartRange = oSheet.get_Range("C4", lowerRightCell);
            xlChart.SetSourceData(chartRange, Type.Missing);
            xlChart.ChartType = XlChartType.xlColumnClustered;

            // Xaxis data labels
            Series oS = (Series)xlChart.SeriesCollection(1);
            //Series oS = (Series)xlChart.Chart.SeriesCollection(1);
            oS.XValues = "='Sheet1'!$B$4:$" + "B$6";
            
            // Add title:
            xlChart.HasTitle = true;
            xlChart.ChartTitle.Text = "My Title";

            // Xaxis title
            Axis xAxis = (Axis)xlChart.Axes(XlAxisType.xlCategory,
                XlAxisGroup.xlPrimary);
            xAxis.HasTitle = true;
            xAxis.AxisTitle.Text = "Budget Years";
            xAxis.AxisTitle.Font.Size = 11;

            // legend:
            xlChart.HasLegend = true;
            int seriesNum = 1;
            string [] aStr = new string[] {"Construction", "Maintenance", "Rehab"};
            foreach (string str in aStr)
            {
                oS = (Series)xlChart.SeriesCollection(seriesNum++);
                //oS = (Series)xlChart.Chart.SeriesCollection(seriesNum++);
                oS.Name = str;
            }

            Axis yAxis = (Axis)xlChart.Axes(XlAxisType.xlValue,
                XlAxisGroup.xlPrimary);
            yAxis.HasTitle = true;
            yAxis.AxisTitle.Text = "Dollars";
            yAxis.AxisTitle.Font.Size = 11;

            // Change font size of X-Axis label
            //Axis xAxis = (Axis)xlChart.Chart.Axes(XlAxisType.xlCategory, XlAxisGroup.xlPrimary);
            //xAxis.AxisTitle.Font.Size = 11;
        }
    }   // end class
}   // end namespace
