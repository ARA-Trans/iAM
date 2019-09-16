using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

namespace Reports
{
    public static class ReportGlobals
    {
        public static int defaultSheetIndex = 1;
        public static string defaultWorkbookName = "";
        public static ExcelManager XLMgr = new ExcelManager();

        public static void CreateReportHeader(string title, string subTitle, string imagePath, string preparedBy)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                XLMgr.InsertImage(imagePath);
            }

            var headerData = new object[6, 1];
            headerData[0, 0] = title;
            headerData[4, 0] = "Prepared by: " + preparedBy;
            headerData[5, 0] = DateTime.Now.ToShortDateString();
            XLMgr.SetValues("C1:C6", headerData);

            XLMgr.MergeCells("C1:I4", false);
            XLMgr.MergeCells("C5:I5", false);
            XLMgr.MergeCells("C6:I6", false);

            XLMgr.ChangeBackgroundColor("A1:I6", Color.FromArgb(0, 0, 211));
            XLMgr.ChangeForegroundColor("A1:I6", Color.FromArgb(255, 255, 255));

            XLMgr.ChangeHorizontalAlignment("A1:I6", XlHAlign.xlHAlignCenter);
            XLMgr.ChangeVerticalAlignment("A1:I6", XlVAlign.xlVAlignCenter);
            XLMgr.ChangeFontSize("C1:I4", 20);
            XLMgr.ChangeFontSize("C5:I6", 16);

            XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
            XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeLeft, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
            XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeRight, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
            XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeTop, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
        }

        public static void SaveReport()
        {
            var sfDlg = new SaveFileDialog();
            try
            {
                sfDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sfDlg.Filter = "Microsoft Office Excel Workbook (*.xls*)|*.xls*";

                sfDlg.RestoreDirectory = true;
                if (sfDlg.ShowDialog() == DialogResult.OK)
                {
                    XLMgr.SaveWorkbookAs(defaultWorkbookName, sfDlg.FileName);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(
                        exc.Message,
                        "Aborted",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
            finally
            {
                sfDlg.Dispose();
            }
            XLMgr.CloseWorkbook(defaultWorkbookName);
        }
    }
}
