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
   public class SectionViewReport
   {
      private DataSet m_ds;
      private String m_strNetworkName;
      private int m_nMinYear, m_nMaxYear;

      public SectionViewReport(DataSet ds, String strNetworkName, int nMinYear, int nMaxYear)
      {
          m_ds = ds;
          m_strNetworkName = strNetworkName;
          m_nMaxYear = nMaxYear;
          m_nMinYear = nMinYear;
      }

      public static List<string> columnsFromArrayStartingWith(string[] strColumns, string strSearch)
      {
          List<string> strSubList = new List<string>();

          for (int i = 0; i <= strColumns.GetUpperBound(0); i++)
          {
              if (strColumns[i].StartsWith(strSearch))
              {
                  strSubList.Add(strColumns[i]);
              }
          }
          strSubList.Sort();
          return strSubList;
      }

      public void CreateSectionReport()
      {
        Report.XL.Visible = false;
        Report.XL.UserControl = false;
        Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
        Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;

        //string strTrace = string.Format("preamble {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        string strReportName = "Section Detail Report";
        Report.SheetPageSetup(oSheet, strReportName, 50d, 20d, 10d, "Network: " + m_strNetworkName, DateTime.Now.ToLongDateString(), "Page &P", 1);
        //oSheet.PageSetup.RightFooter = "Network: " + m_strNetworkName;
        //oSheet.PageSetup.LeftFooter = DateTime.Now.ToLongDateString();
        //oSheet.PageSetup.CenterFooter = "Page &P";
        //oSheet.PageSetup.FirstPageNumber = 1;
        //oSheet.PageSetup.LeftMargin = 50d;
        //oSheet.PageSetup.RightMargin = 20d;
        //oSheet.Columns.Font.Size = 10;

        object oEndCell = new object();
        DataSet dsPage = null;
        DataSet dsGroupNames = null;
        DataSet dsGroupDetail = null;

        try
        {
            dsPage = DBOp.QueryPageHeader(strReportName);
            dsGroupNames = DBOp.QueryAttributeGroupNames();
            dsGroupDetail = DBOp.QueryAttributeByGroup();
        }
        catch (Exception e)
        {
            throw e;
        }

        int nSize = m_nMaxYear-m_nMinYear+2;
        nSize = nSize < 1? 1: nSize;

        // Set up column header once, for multiple uses
        object[] oColumnHeader = new object[nSize];

        oColumnHeader[0] = "ATTRIBUTE_";
        int currentYear = m_nMinYear;
        for (int i = 1; i < nSize; i++, currentYear++)
        {
            oColumnHeader[i] = currentYear.ToString();
        }

        string[] strCols = new string[m_ds.Tables[0].Columns.Count];
        int idx = 0;
        foreach (DataColumn dataColumn in m_ds.Tables[0].Columns)
        {
          strCols[idx] = dataColumn.ColumnName.ToString();
          idx++;
        }
        List<string> subList = new List<string>();

        Cursor c = Cursor.Current;
        Cursor.Current = new Cursor(Cursors.WaitCursor.Handle);

        DataRow drPage = dsPage.Tables[0].Rows[0];
        string strMajorTitle = drPage["phText"].ToString();
        drPage = dsPage.Tables[0].Rows[2];
        string strMinorTitle = drPage["phText"].ToString();
        //if (strMinorTitle.IndexOf("@1") > 0) strMinorTitle = strMinorTitle.Replace("@1", m_strNetworkName);
        //if (strMinorTitle.IndexOf("@2") > 0) strMinorTitle = strMinorTitle.Replace("@2", m_ds.Tables[0].Rows[0]["FACILITY"].ToString() + ", Section: " + m_ds.Tables[0].Rows[0]["SECTION"].ToString());
        
        int sumCol = 0, numSpacers = dsGroupNames.Tables[0].Rows.Count, numColHeaders = 1;

        sumCol = dsGroupDetail.Tables[0].Rows.Count; // number of detail rows

        // calculate number of array rows for report
        int aryRows = ((dsPage.Tables[0].Rows.Count + 1 + dsGroupNames.Tables[0].Rows.Count + sumCol + (dsGroupNames.Tables[0].Rows.Count * numColHeaders) + numSpacers) * m_ds.Tables[0].Rows.Count);
        
        int aryCurrentRow = 0;
        object[,] oData1 = new object[aryRows, nSize];
        Report.ClearDataArray(ref oData1);

        //string strTrace = string.Format("start LOOP {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        string strFilter;
        #region Load data array
        //strTrace = string.Format("/tData start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        foreach (DataRow drdata in m_ds.Tables[0].Rows)
        {
            // Page Header
            strMinorTitle = drPage["phText"].ToString();
            if (strMinorTitle.IndexOf("@1") > 0) strMinorTitle = strMinorTitle.Replace("@1", m_strNetworkName);
            if (strMinorTitle.IndexOf("@2") > 0) strMinorTitle = strMinorTitle.Replace("@2", drdata["FACILITY"].ToString() + ", Section: " + drdata["SECTION"].ToString());
            oData1[aryCurrentRow, 0] = "";
            oData1[aryCurrentRow, 1] = strMajorTitle;
            aryCurrentRow++;
            oData1[aryCurrentRow, 0] = "";
            aryCurrentRow++;
            oData1[aryCurrentRow, 0] = "";
            oData1[aryCurrentRow, 1] = strMinorTitle;

            aryCurrentRow += 2;
            // Load Group Header
            foreach (DataRow dr1 in dsGroupNames.Tables[0].Rows)
            {
                oData1[aryCurrentRow, 0] = dr1["GROUPING"].ToString();
                aryCurrentRow++;
                // Load Column Header
                for (int i = 0; i < nSize; i++)
                {
                    oData1[aryCurrentRow, i] = oColumnHeader[i];
                }
                aryCurrentRow++;
                //Load Detail Data
				strFilter = "GROUPING = '" + dr1["GROUPING"].ToString() + "'";
				foreach (DataRow drDetail in dsGroupDetail.Tables[0].Select(strFilter))
				{
					// Set the Row Label text
					oData1[aryCurrentRow, 0] = drDetail["ATTRIBUTE_"].ToString();
					// Load the Data values
					subList = columnsFromArrayStartingWith(strCols, drDetail["ATTRIBUTE_"].ToString());
					PopulateGroupArrayRowAt(ref oData1, subList, drdata, aryCurrentRow);
					aryCurrentRow++;
				}
                aryCurrentRow++;
            }
        }
        //strTrace = string.Format("/tData stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        oEndCell = "A1";
        oEndCell = Report.WriteObjectArrayToExcel(oData1, oSheet, oEndCell, false);
        Range oR = oSheet.get_Range("A1", "A1");
        String strWork = "";
        List<string> rangeList = new List<string>();
        int sheetRow;

        #region set column widths
        //strTrace = string.Format("/tColWidth start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        sheetRow = 1;
        oR = oSheet.get_Range("A1:A1", Missing.Value);
        oR.ColumnWidth = 16;
        Report.BuildRange(ref strWork, ref rangeList, sheetRow, 'B', nSize - 1);
        Report.EndRangeList(ref strWork, ref rangeList);
        oR = oSheet.get_Range(rangeList.First(), Missing.Value);
        oR.ColumnWidth = 9;
        rangeList.Clear();
        strWork = "";
        //strTrace = string.Format("/tColWidth stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        #region ranges helper Array
        // Build a rows array used for building ranges, formatting report, etc.
        int [] clearRows = new int[dsGroupNames.Tables[0].Rows.Count + 1];
        aryCurrentRow = 1;
        clearRows[0] = dsPage.Tables[0].Rows.Count + 1;
        foreach(DataRow drGrouping in dsGroupNames.Tables[0].Rows) {
            strFilter = "grouping = '" + drGrouping["grouping"].ToString() + "'";

            int filterCount = 0;
            foreach (DataRow drDetail in dsGroupDetail.Tables[0].Select(strFilter))
            {
                filterCount++;
            }
            int cc = clearRows[aryCurrentRow - 1];
            clearRows[aryCurrentRow++] = cc + filterCount + 3;

        }
        #endregion

        #region clear gridlines
        //strTrace = string.Format("/tGridlines start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        sheetRow = 0;

        // Clear gridlines between data group, pages
        for (int i = 0; i < m_ds.Tables[0].Rows.Count; i++)
        {
            for (int clearCount = 0; clearCount <= clearRows.GetUpperBound(0); clearCount++)
            {
                if (clearCount == 0) 
                    sheetRow += clearRows[0];
                else
                    sheetRow += clearRows[clearCount] - clearRows[clearCount-1];

                if (clearCount == 0)
                {
                    strWork += "A" + (sheetRow - 3).ToString() + ":I" + sheetRow.ToString() + ",";
                }
                else
                {
                    Report.BuildRange(ref strWork, ref rangeList, sheetRow, 'A', nSize - 1);
                }
            }
        }
        // Finish the range List
        Report.EndRangeList(ref strWork, ref rangeList);
        foreach (string strRange in rangeList)
        {
            oR = oSheet.get_Range(strRange, Missing.Value);
            oR.ClearFormats();
        }
        rangeList.Clear();
        strWork = "";
        //strTrace = string.Format("/tGridline stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        #region pageBreaks
        // Add page breaks
        //strTrace = string.Format("/tPgBreak start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        sheetRow = clearRows[clearRows.GetUpperBound(0)];
        for (int i = 0; i < m_ds.Tables[0].Rows.Count; i++)
        {
            oR = oSheet.get_Range("A" + sheetRow.ToString(), Missing.Value);
            oR.PageBreak = (int)XlPageBreak.xlPageBreakManual;
            sheetRow += clearRows[clearRows.GetUpperBound(0)];
        }
        //strTrace = string.Format("/tPgBreak stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        #region pageHeader
        // Format page header for each page in report
        //strTrace = string.Format("/tPgHeader start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        sheetRow = 1;
        idx = 1;
        foreach (DataRow drPgHdr in dsPage.Tables[0].Rows)
        {
            for (int i = 0; i < m_ds.Tables[0].Rows.Count; i++)
            {
                //Report.BuildRange(ref strWork, ref rangeList, sheetRow, 'B', nSize - 2);
                Report.BuildRange(ref strWork, ref rangeList, sheetRow, 'B', 7);
                sheetRow += clearRows[clearRows.GetUpperBound(0)];
            }
            // Finish the range List
            Report.EndRangeList(ref strWork, ref rangeList);
            foreach (string strRange in rangeList)
            {
                Report.FormatHeaders(oR, drPgHdr, oSheet, "ph", strRange);
            }
            rangeList.Clear();
            sheetRow = ++idx;   // point to the next pageheader line
            strWork = "";
        }

        //strTrace = string.Format("/tPgHeader stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        #region pageImage
        //strTrace = string.Format("/tImage start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        //string strPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + drPage["reportGraphicFile"].ToString();
		string strPath = ".\\" + drPage["reportGraphicFile"].ToString();
		if (strPath != ".\\")
		{
			Image img = Image.FromFile(strPath);
			System.Windows.Forms.Clipboard.SetDataObject(img, true);
			sheetRow = 1;
			for (int i = 0; i < m_ds.Tables[0].Rows.Count; i++)
			{
				oR = oSheet.get_Range("A" + sheetRow.ToString(), Missing.Value);
				oSheet.Paste(oR, img);
				sheetRow += (clearRows[clearRows.GetUpperBound(0)]);
			}
		}
        //strTrace = string.Format("/tImage stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        #region groupHeaders
          // Format group headers for each page in report, programmed for single line group header with multiple group headers per page
        //strTrace = string.Format("/tGroupHeader start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        aryCurrentRow = 0;
        sheetRow = clearRows[0] + 1;
        for (int i = 0; i < m_ds.Tables[0].Rows.Count; i++)
        {
            aryCurrentRow = 0;
            foreach (DataRow drGroupName in dsGroupNames.Tables[0].Rows)
            {
                Report.BuildRange(ref strWork, ref rangeList, sheetRow, 'A', nSize - 1);
                aryCurrentRow++;
                sheetRow += clearRows[aryCurrentRow] - clearRows[aryCurrentRow - 1];
            }
            sheetRow += clearRows[0];
        }
        // Finish the range List
        Report.EndRangeList(ref strWork, ref rangeList);
        foreach (string strRange in rangeList)
        {
            DataRow drGroupHdr = dsPage.Tables[0].Rows[0];
            Report.FormatHeaders(oR, drGroupHdr, oSheet, "gh", strRange);
        }
        rangeList.Clear();
        strWork = "";
        //strTrace = string.Format("/tGroupHeader stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion

        #region columnHeaders
        // Build the range list for the column headers for each page in report
        //strTrace = string.Format("/tColHeader start {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        aryCurrentRow = 0;
        sheetRow = clearRows[0] + 2;
        for (int i = 0; i < m_ds.Tables[0].Rows.Count; i++)
        {
            aryCurrentRow = 0;
            foreach (DataRow dr1 in dsGroupNames.Tables[0].Rows) // column header is repeated for each group
            {
                Report.BuildRange(ref strWork, ref rangeList, sheetRow, 'A', nSize - 1);
                aryCurrentRow++;
                sheetRow += clearRows[aryCurrentRow] - clearRows[aryCurrentRow - 1];
            }
            sheetRow += clearRows[0];
        }
          // Finish the range List
        Report.EndRangeList(ref strWork, ref rangeList);

        DataRow drCol = dsPage.Tables[0].Rows[0];    // only one row 
        foreach (string strRange in rangeList)
        {
                Report.FormatHeaders(oR, drCol, oSheet, "ch", strRange);
        }
        //strTrace = string.Format("/tColHeader stop {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        #endregion
          
        //strTrace = string.Format("stop LOOP {0}", DateTime.Now.TimeOfDay);
        //System.Diagnostics.Trace.WriteLine(strTrace);
        Report.XL.Visible = true;
        Report.XL.UserControl = true;

      } // end createsectionreport

      private void PopulateGroupArrayRowAt(ref object[,] oData, List<string> subList, DataRow dr, int arrayRow)
      {
          string str;

          for (int j = 0; j < subList.Count; j++)
          {
              str = subList.ElementAt(j);
              int nCol = Convert.ToInt16(Report.Right(str, 4)) - m_nMinYear + 1;
              oData[arrayRow, nCol] = dr[str].ToString();
          }

      }
      

      //private void PopulateGroupArrayRow(ref object[,] oData, List<string> subList, DataRow dr, int arrayRow, int nSize)
      //{
      //      string str;
      //      str = subList.First();
      //      oData[arrayRow, nSize - 1] = dr[str].ToString();
      //}


   }
}
