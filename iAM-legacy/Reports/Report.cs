using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace Reports
{
    public enum Method
    {
        PERCENTAGE,
        AREA
    }

    public static class Report
    {
        /// <summary>
        ///     The Excel application object
        /// </summary>
        public static Microsoft.Office.Interop.Excel.Application m_oXL;

        /// <summary>
        ///     If the Excel object has not been instantiated, then we create a
        ///     new one here, if it already exists we get it.
        /// </summary>
        public static Microsoft.Office.Interop.Excel.Application XL
        {
            get
            {
                if (m_oXL == null)
                {
                    m_oXL = new Microsoft.Office.Interop.Excel.Application();
                    m_oXL.Caption = "RoadCare";
                }
                return m_oXL;
            }
        }

        public static void BuildRange(ref string strWork, ref List<string> rangeList, int sheetRow, char cStartCol, int numCols)
        {
            char asciiChar = cStartCol;
            int asciiValue = (int) asciiChar;
            int nLastComma, nMaxStringLength = 255;
            asciiValue += numCols;
            asciiChar = (char) asciiValue;
            string strTemp;

            strWork += cStartCol.ToString() + sheetRow.ToString() + ":" + asciiChar.ToString() + sheetRow.ToString();

            if (strWork.Length < nMaxStringLength) strWork += ",";
            if (strWork.Length == nMaxStringLength)
            {
                if (strWork.Substring(strWork.Length - 1) == ",")
                    strWork = strWork.Substring(0, strWork.Length - 1); // remove trailing comma
                rangeList.Add(strWork);
                strWork = "";
                return;
            }
            if (strWork.Length > nMaxStringLength)
            {
                strTemp = strWork.Substring(nMaxStringLength);  // store excess string chars
                strWork = strWork.Substring(0, nMaxStringLength);   // reduce length to max
                nLastComma = strWork.LastIndexOf(",");
                strTemp = strWork.Substring(nLastComma + 1) + strTemp;  // prepend excess after comma to temp
                rangeList.Add(strWork.Substring(0, nLastComma));
                strWork = strTemp;  // reset work to the excess and add a "," if needed
                if (strWork.Substring(strWork.Length - 1) != ",") strWork += ",";
            }
        }

        public static void ClearDataArray(ref object[,] oData)
        {
            for (int j = 0; j < oData.GetUpperBound(0) + 1; j++)
            {
                for (int i = 0; i < oData.GetUpperBound(1) + 1; i++)
                {
                    oData[j, i] = "";
                }
            }
            return;
        }

        public static void CloseExcel()
        {
            XL.Quit();
        }

        public static void CreateColClusterBarGraph(
            int left, int top, int width, int height,
            _Worksheet oSheet, Range dataRange,
            string strXAxis, string strChartTitle, double titleFontSize,
            string strXTitle, double xFontSize, List<string> listLegend,
            string strYTitle, double yFontSize, XlRowCol rowCol)
        {
            ChartObjects chartObjs = (ChartObjects) oSheet.ChartObjects(Type.Missing);
            ChartObject chartObj = chartObjs.Add(left + 100, top, 425, 315);
            Chart xlChart = chartObj.Chart;

            xlChart.SetSourceData(dataRange, Type.Missing);
            xlChart.ChartType = XlChartType.xlColumnClustered;

            xlChart.PlotBy = rowCol;

            // Xaxis data labels
            Series oS = (Series) xlChart.SeriesCollection(1);

            string strVersion = m_oXL.Application.Version;
            if (strVersion == "11.0")  // Office 2003 Excel
            {
                string strStartRow, strEndRow, strStartCol, strEndCol;
                int nPos = -1, nEnd = -1, nStartCol, nEndCol, nStartRow, nEndRow;

                // Sample incoming: "='Total Lane-Miles Per
                // Condition'!$B$5:$F$5"; or "='Total Lane-Miles Per
                // Condition'!$B$5:$B$10";
                nPos = strXAxis.IndexOf('$');
                if (nPos > -1)
                {
                    nEnd = strXAxis.IndexOf('$', nPos + 1);
                    strStartCol = strXAxis.Substring(nPos + 1, nEnd - nPos - 1);

                    nPos = strXAxis.IndexOf(':');
                    strStartRow = strXAxis.Substring(nEnd + 1, nPos - nEnd - 1);

                    nPos = strXAxis.IndexOf('$', nPos);
                    nEnd = strXAxis.IndexOf('$', nPos + 1);
                    strEndCol = strXAxis.Substring(nPos + 1, nEnd - nPos - 1);
                    strEndRow = strXAxis.Substring(nEnd + 1);

                    nStartRow = int.Parse(strStartRow);
                    nEndRow = int.Parse(strEndRow);

                    nStartCol = GetColumnNumber(strStartCol + strStartRow);
                    nEndCol = GetColumnNumber(strEndCol + strStartRow);

                    nPos = strXAxis.IndexOf('!');
                    strXAxis = strXAxis.Substring(0, nPos + 1);
                    strXAxis += "R" + nStartRow.ToString() + "C" + nStartCol.ToString() + ":R" + nEndRow.ToString() +
                       "C" + nEndCol.ToString();
                }
            }
            oS.XValues = strXAxis;

            // Add title:
            if (strChartTitle != "")
            {
                xlChart.HasTitle = true;
                xlChart.ChartTitle.Text = strChartTitle;
            }

            // Xaxis title
            if (strXTitle != "")
            {
                Axis xAxis = (Axis) xlChart.Axes(XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = strXTitle;
                xAxis.AxisTitle.Font.Size = xFontSize;
            }

            // legend:
            xlChart.HasLegend = false;
            if (listLegend != null)
            {
                if (listLegend.Count > 0)
                {
                    xlChart.HasLegend = true;

                    int seriesNum = 1;
                    foreach (string str in listLegend)
                    {
                        oS = (Series) xlChart.SeriesCollection(seriesNum);
                        oS.Name = str;
                        seriesNum++;
                    }
                }
            }

            if (strYTitle != "")
            {
                Axis yAxis = (Axis) xlChart.Axes(XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = strYTitle;
                yAxis.AxisTitle.Font.Size = yFontSize;
            }
        }

        /// <summary>
        ///     Creates a generic header to be placed on all sheets
        /// </summary>
        /// <param name="image">Agency/Client image</param>
        /// <param name="oSheet">Sheet to place header on</param>
        /// <param name="oStartCell">
        ///     Starting cell for header placement (probably "A1") defaults to
        ///     "A1"
        /// </param>
        /// <param name="strMajorTitle">
        ///     16pt title across the top of the sheet
        /// </param>
        /// <param name="strMinorTitle">14pt title below major title</param>
        /// <param name="strComment">
        ///     Comment line directly below header information, can be empty
        ///     string.
        /// </param>
        /// <returns>oEndCell - Lower right cell of header.</returns>
        public static object CreateHeader(String strImagePath, _Worksheet oSheet, object oStartCell, String strMajorTitle, String strMinorTitle, String strComment)
        {
            object oEndCell = GetCellAtOffset(oStartCell, 4, 14);
            object oHeaderTextStartCell = GetCellAtOffset(oStartCell, 0, 1);
            object oHeaderTextEndCell = GetCellAtOffset(oHeaderTextStartCell, 1, 9);

            Range cellRange = oSheet.get_Range(oStartCell, GetCellAtOffset(oStartCell, 2, 3));

            try
            {
                Image img = Image.FromFile(strImagePath);
                System.Windows.Forms.Clipboard.SetDataObject(img, true);
                oSheet.Paste(cellRange, img);
            }
            catch
            {
            }

            object[,] oHeaderData = new object[6, 11];
            oHeaderData[0, 0] = strMajorTitle;
            oHeaderData[2, 0] = strMinorTitle;
            oHeaderData[5, 0] = strComment;
            oHeaderData[1, 10] = "Prepared By: ";
            oHeaderData[2, 10] = DateTime.Now.ToLongDateString();
            oHeaderData[3, 10] = "Identifier: ";

            object oEndDataCell = GetCellAtOffset(oHeaderTextStartCell, 5, 10);
            Range oRngData = oSheet.get_Range(oHeaderTextStartCell, oEndDataCell);

            oRngData.Value2 = oHeaderData;

            Range oRngTitles = oSheet.get_Range(oHeaderTextStartCell, oHeaderTextEndCell);
            oRngTitles.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oRngTitles.VerticalAlignment = XlVAlign.xlVAlignCenter;
            oRngTitles.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
            oRngTitles.Font.Size = 20;
            oRngTitles.Font.Bold = true;
            oRngTitles.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.AntiqueWhite);
            oRngTitles.MergeCells = true;

            oHeaderTextStartCell = GetCellAtOffset(oHeaderTextStartCell, 2, 0);
            oHeaderTextEndCell = GetCellAtOffset(oHeaderTextStartCell, 1, 9);

            oRngTitles = oSheet.get_Range(oHeaderTextStartCell, oHeaderTextEndCell);
            oRngTitles.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            oRngTitles.VerticalAlignment = XlVAlign.xlVAlignCenter;
            oRngTitles.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
            oRngTitles.Font.Size = 14;
            oRngTitles.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.AntiqueWhite);
            oRngTitles.MergeCells = true;

            if (strComment != "")   // do not want the strComment and interior color for every report!
            {
                Range oRngComments = oSheet.get_Range(GetCellAtOffset(oStartCell, 5, 0), GetCellAtOffset(oStartCell, 5, 13));
                oRngComments.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oRngComments.VerticalAlignment = XlVAlign.xlVAlignCenter;
                oRngComments.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
                oRngComments.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.AntiqueWhite);
                oRngComments.MergeCells = true;
            }

            Range oRngIdentifier = oSheet.get_Range(GetCellAtOffset(oStartCell, 1, 13), GetCellAtOffset(oStartCell, 1, 16));
            oRngIdentifier.Font.Italic = true;
            oRngIdentifier.MergeCells = false;

            oRngIdentifier = oSheet.get_Range(GetCellAtOffset(oStartCell, 2, 13), GetCellAtOffset(oStartCell, 2, 16));
            oRngIdentifier.Font.Italic = true;
            oRngIdentifier.MergeCells = false;

            oRngIdentifier = oSheet.get_Range(GetCellAtOffset(oStartCell, 3, 13), GetCellAtOffset(oStartCell, 3, 16));
            oRngIdentifier.Font.Italic = true;
            oRngIdentifier.MergeCells = false;
            return oEndCell;
        }

        /// <summary>
        ///     Starts Excel if it is closed, and creates a workbook.
        /// </summary>
        /// <param name="strWorkBookName">
        ///     Name of the workbook to be created.
        /// </param>
        /// <returns>The created workbook</returns>
        public static Microsoft.Office.Interop.Excel._Workbook CreateWorkBook()
        {
            Microsoft.Office.Interop.Excel._Workbook oWB = (Microsoft.Office.Interop.Excel._Workbook) (XL.Workbooks.Add(Missing.Value));
            return oWB;
        }

        public static void EndRangeList(ref string strWork, ref List<string> rangeList)
        {
            // Finish the range List
            if (strWork != "")
            {
                if (strWork.Substring(strWork.Length - 1, 1) == ",")
                    strWork = strWork.Substring(0, strWork.Length - 1); // remove trailing comma
                rangeList.Add(strWork);
            }
        }

        public static void FormatHeaders(Range oRng, DataRow dr, _Worksheet oSheet, string strPrefix, string strRange)
        {
            string strFontProperty, strFieldName;

            oRng = oSheet.get_Range(strRange, Missing.Value);

            strFieldName = strPrefix + "MergeCells";
            strFontProperty = dr[strFieldName].ToString();
            if (strFontProperty == "True") oRng.MergeCells = true;

            strFieldName = strPrefix + "FontColor";
            strFontProperty = dr[strFieldName].ToString();
            if (strFontProperty != "")
                oRng.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.FromName(strFontProperty));

            strFieldName = strPrefix + "InteriorColor";
            strFontProperty = dr[strFieldName].ToString();
            if (strFontProperty != "")
                oRng.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromName(strFontProperty));

            strFieldName = strPrefix + "FontSize";
            strFontProperty = dr[strFieldName].ToString();
            if (strFontProperty == "") strFontProperty = "11";  //default font size
            oRng.Font.Size = Convert.ToInt32(strFontProperty);

            strFieldName = strPrefix + "FontBold";
            strFontProperty = dr[strFieldName].ToString();
            if (strFontProperty == "True") oRng.Font.Bold = true;

            strFieldName = strPrefix + "HAlign";
            strFontProperty = dr[strFieldName].ToString();
            if (strFontProperty == "C")
                oRng.HorizontalAlignment = XlHAlign.xlHAlignCenter;
            else if (strFontProperty == "R") oRng.HorizontalAlignment = XlHAlign.xlHAlignRight;
            else if (strFontProperty == "L") oRng.HorizontalAlignment = XlHAlign.xlHAlignLeft;
        }

        public static object GetCellAtOffset(object oStartCell, int iRow, int iCol)
        {
            int iNumEndRow = iRow + GetRowNumber(oStartCell.ToString());
            int iNumEndColumn = iCol + GetColumnNumber(oStartCell.ToString());

            return GetColumnLetter(iNumEndColumn) + iNumEndRow.ToString();
        }

        /// <summary>
        ///     Returns the character representation of the numbered column
        /// </summary>
        /// <param name="iColumnNumber">The number of the column</param>
        /// <returns>The letter representing the column</returns>
        public static string GetColumnLetter(int iColumnNumber)
        {
            int iMainLetterUnicode;
            char iMainLetterChar;

            // TODO: we need to cater for columns larger than ZZ
            if (iColumnNumber > 26)
            {
                int iFirstLetterUnicode = 0;  // default
                int iFirstLetter = Convert.ToInt32(iColumnNumber / 26);
                char iFirstLetterChar;
                if (Convert.ToDouble(iFirstLetter) == (Convert.ToDouble(iColumnNumber) / 26))
                {
                    iFirstLetterUnicode = iFirstLetter - 1 + 64;
                    iMainLetterChar = 'Z';
                }
                else
                {
                    iFirstLetterUnicode = iFirstLetter + 64;
                    iMainLetterUnicode = (iColumnNumber - (iFirstLetter * 26)) + 64;
                    iMainLetterChar = (char) iMainLetterUnicode;
                }
                iFirstLetterChar = (char) iFirstLetterUnicode;

                return (iFirstLetterChar.ToString() + iMainLetterChar.ToString());
            }
            // if we get here we only have a single letter to return
            iMainLetterUnicode = 64 + iColumnNumber;
            iMainLetterChar = (char) iMainLetterUnicode;
            return (iMainLetterChar.ToString());
        }

        /// <summary>
        ///     Returns the column number from the cellAddress e.g. D5 would
        ///     return 5
        /// </summary>
        /// <param name="cellAddress">
        ///     An Excel format cell addresss (e.g. D5)
        /// </param>
        /// <returns>The column number</returns>
        public static int GetColumnNumber(string cellAddress)
        {
            // find out position where characters stop and numbers begin
            int iColumnNumber = 0;
            int iPos = 0;
            bool found = false;
            foreach (char chr in cellAddress.ToCharArray())
            {
                iPos++;
                if (char.IsNumber(chr))
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                string AlphaPart = cellAddress.Substring(0, cellAddress.Length - (cellAddress.Length + 1 - iPos));

                int length = AlphaPart.Length;
                int count = 0;
                foreach (char chr in AlphaPart.ToCharArray())
                {
                    count++;
                    int chrValue = ((int) chr - 64);
                    switch (length)
                    {
                    case 1:
                        iColumnNumber = chrValue;
                        break;

                    case 2:
                        if (count == 1)
                            iColumnNumber += (chrValue * 26);
                        else
                            iColumnNumber += chrValue;
                        break;

                    case 3:
                        if (count == 1)
                            iColumnNumber += (chrValue * 26 * 26);
                        if (count == 2)
                            iColumnNumber += (chrValue * 26);
                        else
                            iColumnNumber += chrValue;
                        break;

                    case 4:
                        if (count == 1)
                            iColumnNumber += (chrValue * 26 * 26 * 26);
                        if (count == 2)
                            iColumnNumber += (chrValue * 26 * 26);
                        if (count == 3)
                            iColumnNumber += (chrValue * 26);
                        else
                            iColumnNumber += chrValue;
                        break;
                    }
                }
            }
            return (iColumnNumber);
        }

        // Sums the column width(s) in the supplied range (starts with column A,
        // ends with last column in the range) returns float
        public static float GetColumnWidthInPixels(Range oR, _Worksheet oSheet)
        {
            float sum = 0f;
            for (int index = 1; index <= oR.Columns.Count; index++)
            {
                sum += float.Parse(((Range) oSheet.Cells[1, index]).EntireColumn.Width.ToString());
            }
            return sum;
        }

        public static int GetNextRowNumber(object oStartCell)
        {
            return GetRowNumber(oStartCell.ToString()) + 1;
        }

        /// <summary>
        ///     Returns the row number from the cellAddress e.g. D5 would return
        ///     5
        /// </summary>
        /// <param name="cellAddress">
        ///     An Excel format cell addresss (e.g. D5)
        /// </param>
        /// <returns>The row number</returns>
        public static int GetRowNumber(string cellAddress)
        {
            // find out position where characters stop and numbers begin
            int iPos = 0;
            bool found = false;
            foreach (char chr in cellAddress.ToCharArray())
            {
                iPos++;
                if (char.IsNumber(chr))
                {
                    found = true;
                    break;
                }
            }
            if (found)
            {
                string NumberPart = cellAddress.Substring(iPos - 1, cellAddress.Length - (iPos - 1));
                if (IsNumericValue(NumberPart))
                    return (int.Parse(NumberPart));
            }
            return (0);
        }

        /// <summary>
        ///     Returns true if the string contains a numeric value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsNumericValue(string Value)
        {
            Regex objNotIntPattern = new Regex("[^0-9,.-]");
            Regex objIntPattern = new Regex("^-[0-9,.]+$|^[0-9,.]+$");

            return !objNotIntPattern.IsMatch(Value) && objIntPattern.IsMatch(Value);
        }

        // return left i characters of string s
        public static string Left(string s, int i)
        {
            if (s.Length < i)
                return s;
            else
                return s.Substring(0, i);
        }

        public static void PlaceReportGraphic(string strPath, Range oR, _Worksheet oSheet)
        {
            // Place report graphic
            if (!String.IsNullOrEmpty(strPath) && strPath != ".\\")
            {
                try
                {
                    Image img = Image.FromFile(strPath);
                    System.Windows.Forms.Clipboard.SetDataObject(img, true);
                    oR = oSheet.get_Range("A1", Missing.Value);
                    oSheet.Paste(oR, img);
                }
                catch { }
            }
        }

        public static void Resize2DArray(ref object[,] oData1, int rows, int cols)
        {
            object[,] newArray = new object[rows, cols];
            oData1 = newArray;
        }

        // return right i characters of string s
        public static string Right(string s, int i)
        {
            if (s.Length < i)
                return s;
            else
                return s.Substring(s.Length - i, i);
        }

        // oRngTitles = oSheet.get_Range(oHeaderTextStartCell,
        // oHeaderTextEndCell); oRngTitles.HorizontalAlignment =
        // XlHAlign.xlHAlignLeft; oRngTitles.VerticalAlignment =
        // XlVAlign.xlVAlignCenter; oRngTitles.Interior.Color =
        // System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);
        // oRngTitles.Font.Size = 13; oRngTitles.Font.Color =
        // System.Drawing.ColorTranslator.ToOle(Color.White);
        // oRngTitles.MergeCells = false;
        public static void SheetPageSetup(_Worksheet oSheet, string strSheetName, double leftMargin, double rightMargin, double colFontSize,
            string rightFooter, string leftFooter, string centerFooter, int firstPageNumber)
        {
            if (rightFooter != "") oSheet.PageSetup.RightFooter = rightFooter;
            if (leftFooter != "") oSheet.PageSetup.LeftFooter = leftFooter;
            if (centerFooter != "") oSheet.PageSetup.CenterFooter = centerFooter;
            if (firstPageNumber > 0) oSheet.PageSetup.FirstPageNumber = firstPageNumber;
            if (leftMargin > 0d) oSheet.PageSetup.LeftMargin = leftMargin;
            if (rightMargin > 0d) oSheet.PageSetup.RightMargin = rightMargin;
            if (colFontSize > 0d) oSheet.Columns.Font.Size = colFontSize;
            oSheet.Name = strSheetName;
        }

        /// <summary>
        ///     Writes out a C# DataGridView to an excel file
        /// </summary>
        /// <param name="dgv">The DataGridView to write</param>
        /// <param name="oSheet">An Excel worksheet</param>
        /// <param name="oStartingCell">
        ///     Excel cell at which to begin writing data. ie "C1"
        /// </param>
        /// <param name="bIncludeColumnHeaders">
        ///     Whether to include the dgv column headers in the Excel sheet
        ///     output.
        /// </param>
        /// <returns>The cell address ie "B7"</returns>
        public static object WriteDGVToExcel(DataGridView dgv, _Worksheet oSheet, object oStartingCell, bool bIncludeColumnHeaders)
        {
            // Getting the data from the dgv into the object array. NOTE: Excel,
            // being a wonderful program, starts its cell numbering at 1,1. Just
            // something to keep in mind.
            Range oRng;
            object[,] oDgvData;

            int iNumEndRow = dgv.Rows.Count + GetRowNumber(oStartingCell.ToString()) - 1;
            int iNumEndColumn = dgv.Columns.Count + GetColumnNumber(oStartingCell.ToString()) - 1;

            // Check for column headers, if they are included, add a row for
            // them and fill oDgvData oDgvData is a two dimensional object array
            // used to output the dgv data to the excel sheet. A single object
            // array can contain any type of data int, string, objects, etc.
            if (bIncludeColumnHeaders)
            {
                iNumEndRow++;
                oDgvData = new object[dgv.Rows.Count + 1, dgv.Columns.Count];
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    oDgvData[0, i] = dgv.Columns[i].HeaderText;
                }
            }
            else
            {
                oDgvData = new object[dgv.Rows.Count, dgv.Columns.Count];
            }

            // Here, we add one to the iterator to account for cell headers if
            // they are being included.
            // NOTE: The dgv[i,j] is COLUMN, ROW not row column. Fun.
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                for (int j = 0; j < dgv.Columns.Count; j++)
                {
                    if (dgv[j, i].Value != null)
                    {
                        if (bIncludeColumnHeaders)
                        {
                            oDgvData[i + 1, j] = dgv[j, i].Value;
                        }
                        else
                        {
                            oDgvData[i, j] = dgv[j, i].Value;
                        }
                    }
                }
            }

            // Sets the range in the excel spreadsheet, to insert oDgvData data.
            // Get column letter returns the letter associated with the Nth
            // column given it.
            object oEndCell = GetColumnLetter(iNumEndColumn) + iNumEndRow.ToString();
            oSheet.get_Range(oStartingCell, oEndCell).Value2 = oDgvData;

            // Autofits the inserted excel data.
            oRng = oSheet.get_Range(oStartingCell, oEndCell);
            oRng.EntireColumn.AutoFit();

            // Sets interior and then exterior borders and line styles.
            oRng.Borders.LineStyle = XlLineStyle.xlContinuous;
            oRng.Borders.Weight = XlBorderWeight.xlThin;
            oRng.BorderAround(XlLineStyle.xlDouble, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

            // This is the oRng for the header row only.
            if (bIncludeColumnHeaders)
            {
                object oLastHeaderCell = GetColumnLetter(iNumEndColumn) + GetRowNumber(oStartingCell.ToString()).ToString();

                oRng = oSheet.get_Range(oStartingCell, oLastHeaderCell);
                oRng.BorderAround(XlLineStyle.xlDouble, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
                oRng.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.AntiqueWhite);
                oRng.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Navy);

                // Bolding takes 5 seconds to complete avoid if possible.
                oRng.Font.Bold = true;
                oRng.RowHeight = 30;
                oRng.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                oRng.VerticalAlignment = XlVAlign.xlVAlignCenter;
            }
            return oEndCell;
        }

        public static object WriteObjectArrayToExcel(object[,] oData, _Worksheet oSheet, object oStartingCell, bool bHeaderRow)
        {
            int nRow = oData.GetLength(0);
            int nCol = oData.GetLength(1);

            Range oRange = oSheet.get_Range(oStartingCell, GetCellAtOffset(oStartingCell, nRow - 1, nCol - 1));
            oRange.Value2 = oData;

            // Sets interior and then exterior borders and line styles.
            oRange.Borders.LineStyle = XlLineStyle.xlContinuous;
            oRange.Borders.Weight = XlBorderWeight.xlThin;
            oRange.BorderAround(XlLineStyle.xlDouble, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);

            if (bHeaderRow)
            {
                //Get Range for head column.
                oRange = oSheet.get_Range(oStartingCell, GetCellAtOffset(oStartingCell, 0, nCol - 1));
                oRange.BorderAround(XlLineStyle.xlDouble, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
            }
            return GetCellAtOffset(oStartingCell, nRow - 1, nCol - 1);
        }

        public static object WriteObjectArrayToExcel(object[,] oData, _Worksheet oSheet, object oStartingCell, bool bHeaderRow, bool bBorder)
        {
            int nRow = oData.GetLength(0);
            int nCol = oData.GetLength(1);

            Range oRange = oSheet.get_Range(oStartingCell, GetCellAtOffset(oStartingCell, nRow - 1, nCol - 1));
            oRange.Value2 = oData;

            // Sets interior and then exterior borders and line styles.
            if (bBorder)
            {
                oRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                oRange.Borders.Weight = XlBorderWeight.xlThin;
                oRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
            }

            if (bHeaderRow)
            {
                //Get Range for head column.
                oRange = oSheet.get_Range(oStartingCell, GetCellAtOffset(oStartingCell, 0, nCol - 1));
                oRange.BorderAround(XlLineStyle.xlContinuous, XlBorderWeight.xlThin, XlColorIndex.xlColorIndexAutomatic, Missing.Value);
            }
            return GetCellAtOffset(oStartingCell, nRow - 1, nCol - 1);
        }
    }
}
