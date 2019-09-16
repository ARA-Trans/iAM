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
    public class LaneMilePerTreatmentReport
    {
        bool m_useLaneMiles;
        string m_strNetworkID, m_strSimulationID, m_strNetwork, m_strSimulation, m_strBudgetPer;

        public LaneMilePerTreatmentReport(string strNetworkID, string strSimulationID, string strNetwork, string strSimulation, string strBudgetPer,bool useLaneMiles)
        {
            m_strNetworkID = strNetworkID;
            m_strSimulationID = strSimulationID;
            m_strNetwork = strNetwork;
            m_strSimulation = strSimulation;
            strBudgetPer = strBudgetPer.ToLower();
            m_strBudgetPer = char.ToUpper(strBudgetPer[0]) + strBudgetPer.Substring(1);
            m_useLaneMiles = useLaneMiles;
        }

        public void CreateLaneMilesPerTreatmentReport()
        {
            Report.XL.Visible = false;
            Report.XL.UserControl = false;
            Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
            Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

            string tabName = "Lane-Miles Per";
            if(!m_useLaneMiles) tabName = "VLM Per";

            Report.SheetPageSetup(oSheet, tabName + m_strBudgetPer, 50d, 20d, 10d, m_strNetwork + " - " + m_strSimulation, DateTime.Now.ToLongDateString(), "Page &P", 1);
            Range oR = oSheet.get_Range("A1", "A1");

            object oEndCell = new object();
            string strFilter = "";
            string  strReportName = "Lane-Miles Per Treatment Report";
            string strRange;

            int sheetRow;
            int ndx, nCol;
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
            if (m_useLaneMiles)strReportName = "Vehicle Lane-Miles Per Treatment Report";
            Cursor c = Cursor.Current;
            Cursor.Current = new Cursor(Cursors.WaitCursor.Handle);

            DataRow drPage = dsPage.Tables[0].Rows[0];
            string strMajorTitle = drPage["phText"].ToString();
            if (!m_useLaneMiles) strMajorTitle = "Vehicle " + strMajorTitle;
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
            DataSet dsBudgetYears = DBOp.QueryBudgetYears(m_strSimulationID);
            int numYears = dsBudgetYears.Tables[0].Rows.Count;
            #endregion

            #region get Treatments
            List<string> treatmentList = DBOp.GetTreatments(m_strNetworkID, m_strSimulationID);
            treatmentList.Remove("No Treatment");
            int numTreatments = treatmentList.Count();
            #endregion

            aryCols = numYears + 1;
            aryRows = numTreatments + 3;
            Report.Resize2DArray(ref oData, aryRows, aryCols);
            Report.ClearDataArray(ref oData);

            #region build the Lane-Miles table
            Hashtable laneMilesPerTreatment;
            // get the number of districts
            List<string> strJuris = DBOp.GetJurisdiction(m_strNetworkID, "DISTRICT");
            strJuris.Sort();
            int numDistricts = strJuris.Count;
            sheetRow = 4;

            ndx = 1;
            if(m_useLaneMiles) oData[0, 0] = "Annual Total Lane-Miles";
            else oData[0, 0] = "Annual Total Vehicle Lane-Miles";
            oData[1, 0] = m_strBudgetPer;
            foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
            {
                oData[1, ndx++] = dr["Year_"];
            }
            sheetRow++;
            ndx = 2;
            nCol = 0;
            foreach (string s in treatmentList)
            {
                oData[ndx++, nCol] = s;
                sheetRow++;
            }
            oData[ndx, nCol] = "Total";
            sheetRow++;
            int totalRow = oData.GetUpperBound(0);
            double nTotal = 0d, nMiles;
            List<string> listAttributes;
            foreach (DataRow dr in dsBudgetYears.Tables[0].Rows)
            {
                ndx = 2;
                nCol++;
                nTotal = 0d;
                foreach (string s in treatmentList)
                {
                    strFilter = "TREATMENT = '" + s + "' AND YEARS = " + dr["Year_"].ToString();
                    laneMilesPerTreatment = DBOp.GetPercentagePerStringAttribute(m_strNetworkID, m_strSimulationID, "DISTRICT", "", "AREA", strFilter,m_useLaneMiles, out listAttributes);
                    nMiles = sumLaneMiles(laneMilesPerTreatment, numDistricts); // have to sum the districts to get the total
                    nTotal += nMiles;
                    oData[ndx++, nCol] = nMiles;
                    oData[totalRow, nCol] = nTotal;
                }
            }


            oEndCell = "A4";
            oEndCell = Report.WriteObjectArrayToExcel(oData, oSheet, oEndCell, false, false);
            #endregion

            #region format pageheader
            // PAGEHEADER
            strRange = "B1:" + Report.GetColumnLetter(aryCols) + "1";
            DataRow drPgHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            #endregion

            #region format groupheader
            strRange = "A4:" + Report.GetColumnLetter(aryCols) + "4";
            Report.FormatHeaders(oR, drPage, oSheet, "gh", strRange);
            #endregion

            #region format columnHeader
            strRange = "A5:" + Report.GetColumnLetter(aryCols) + "5";
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);
            #endregion

            #region format totals row
            strRange = "A" + (sheetRow).ToString() + ":" + Report.GetColumnLetter(aryCols) + (sheetRow).ToString();
            Report.FormatHeaders(oR, drPage, oSheet, "ch", strRange);
            #endregion

            #region format grid data
            strRange = "A6:" + Report.GetColumnLetter(aryCols) + (sheetRow).ToString();
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

            #region set column width
            strRange = "B6:" + Report.GetColumnLetter(aryCols) + (sheetRow).ToString();
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.NumberFormat = "#,##0.0";
            oR.ColumnWidth = 12;
            #endregion

            #region create column charts
            string strTitle = "";
            strRange = "B6:" + Report.GetColumnLetter(aryCols) + (sheetRow - 1).ToString();
            Range oSourceData = oSheet.get_Range(strRange, Missing.Value);
            int left = (int)Report.GetColumnWidthInPixels(oSourceData, oSheet);
            //strRange = "B6:F13";
            //oSourceData = oSheet.get_Range((sTmp + nTmp.ToString()), Missing.Value);

            strTitle = "Annual Lane-Miles Per " + m_strBudgetPer;
            if (!m_useLaneMiles) strTitle = "Annual Vehicle Lane-Miles Per " + m_strBudgetPer;
            
            string sheetName = "='Lane-Miles Per" + m_strBudgetPer + "'!$B$5:$" + Report.GetColumnLetter(aryCols) + "$5";
            if (!m_useLaneMiles) sheetName = "='VLM Per" + m_strBudgetPer + "'!$B$5:$" + Report.GetColumnLetter(aryCols) + "$5";

            Report.CreateColClusterBarGraph(left, 15, 425, 315, oSheet, oSourceData, sheetName,
                strTitle, 12, "Fiscal Year", 11, treatmentList, "", 11, XlRowCol.xlRows);
            
            #endregion

            Report.XL.Visible = true;
            Report.XL.UserControl = true;
        }   // end CreateBudgetPerTreatmentReport

        private double sumLaneMiles(Hashtable ht, int numberOfJurisdictions)
        {
            double retval = 0d, laneMiles;

            //for (int i = 1; i <= ht.Count; i++)
            for (int i = 1; i <= numberOfJurisdictions; i++)
                {
                object o = ht[i.ToString()];
                if (o != null)
                {
                    laneMiles = Convert.ToDouble(o);
                    retval += laneMiles;
                }
            }

            return retval;
        }
    }   // end class
}   // end namespace
