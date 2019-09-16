using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseManager;
using System.Data;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace Reports.DDOT
{
	public class MileageByFunctionalClass
	{
		private int m_networkID;
		private string m_userName;

		public MileageByFunctionalClass(int networkID, string userName)
		{
			m_networkID = networkID;
			m_userName = userName;
		}


		public void CreateMileageByFuncClass()
		{
			string query = "SELECT CLASS_NUM, SUM(ATTR_LENGTH) / 5280 AS CLASS_LENGTH FROM (SELECT CLASS.DATA_ AS CLASS_NUM, MAX(LENGTH.DATA_) AS ATTR_LENGTH FROM LENGTH INNER JOIN CLASS ON CLASS.FACILITY = LENGTH.FACILITY AND CLASS.SECTION = LENGTH.SECTION INNER JOIN DOMAIN ON DOMAIN.FACILITY = CLASS.FACILITY AND DOMAIN.SECTION = CLASS.SECTION WHERE DOMAIN.DATA_ = 'DDOT' GROUP BY CLASS.DATA_, LENGTH.FACILITY, LENGTH.SECTION) AS DERIVEDTABLE GROUP BY DERIVEDTABLE.CLASS_NUM";
			DataSet funcClassData = null;
			funcClassData = DBMgr.ExecuteQuery(query);


			SetUpWorkBook("MileageByFunctionalClass");
			CreateReportHeader("Mileage By Functional Class");
			CreateReportRows(funcClassData.Tables[0]);
			
			
			SaveReport();
			ReportGlobals.XLMgr.CloseWorkbook( ReportGlobals.defaultWorkbookName );
		}

		private void SetUpWorkBook( string reportName )
		{
			ReportGlobals.XLMgr.CreateNewWorkbook(reportName);
			ReportGlobals.defaultWorkbookName = reportName;
			ReportGlobals.defaultSheetIndex = 1;
		}


		private void CreateReportRows( System.Data.DataTable reportTable )
		{

			int numRows = reportTable.Rows.Count;
			
			object[,] reportData = new object[numRows + 1, 2];
			reportData[0, 0] = "Functional Class";
			reportData[0, 1] = "Length (Mi)";
			string cellsToColor = "";
			for( int i = 0; i < numRows; ++i )
			{
			    reportData[i+1, 0] = reportTable.Rows[i][0];
			    reportData[i+1, 1] = reportTable.Rows[i][1];
				if( i % 2 == 0 )
				{
					cellsToColor += "A" + (i+8).ToString() + ":I" + (i+8).ToString() + "," ;
				}
			}
			cellsToColor = cellsToColor.Substring(0,cellsToColor.Length-1);
			ReportGlobals.XLMgr.SetValues( "D7:E" + ( numRows + 7 ).ToString(), reportData );
			ReportGlobals.XLMgr.ChangeFontSize( "A7:I7", 16 );
			ReportGlobals.XLMgr.ChangeBackgroundColor(cellsToColor, Color.FromArgb( 197, 217, 241 ) );
			ReportGlobals.XLMgr.ChangeHorizontalAlignment( "A1:I" + ( numRows + 7 ).ToString(), XlHAlign.xlHAlignCenter );
			ReportGlobals.XLMgr.MergeCells( "A7:D" + ( numRows + 7 ).ToString(), true );
			ReportGlobals.XLMgr.MergeCells( "E7:I" + ( numRows + 7 ).ToString(), true );

			ReportGlobals.XLMgr.SetBorders( "A7:I7", XlBordersIndex.xlInsideHorizontal, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous );
			ReportGlobals.XLMgr.SetBorders( "A7:I7", XlBordersIndex.xlInsideVertical, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous );

			ReportGlobals.XLMgr.SetBorders( "A1:D" + ( numRows + 7 ).ToString(), XlBordersIndex.xlEdgeRight, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous );
	
		}

		private void CreateReportHeader( string reportName )
		{
			ReportGlobals.XLMgr.InsertImage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RoadCare3 Documents\Washington DC\ddot logo.gif");
			object[,] headerData = new object[6, 1];
			headerData[0, 0] = "DDOT - " + reportName;
			headerData[4, 0] = "Prepared by: " + m_userName;
			headerData[5, 0] = DateTime.Now.ToShortDateString();
			ReportGlobals.XLMgr.SetValues("C1:C6", headerData);

			ReportGlobals.XLMgr.MergeCells("C1:I4", false);
			ReportGlobals.XLMgr.MergeCells("C5:I5", false);
			ReportGlobals.XLMgr.MergeCells("C6:I6", false);

			ReportGlobals.XLMgr.ChangeBackgroundColor("A1:I6", Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeForegroundColor("A1:I6", Color.FromArgb(255, 255, 255));

			ReportGlobals.XLMgr.ChangeHorizontalAlignment("A1:I6", XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment("A1:I6", XlVAlign.xlVAlignCenter);
			ReportGlobals.XLMgr.ChangeFontSize("C1:I4", 20);
			ReportGlobals.XLMgr.ChangeFontSize("C5:I6", 16);

			ReportGlobals.XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeLeft, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeRight, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A1:I6", XlBordersIndex.xlEdgeTop, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);

		}


		private void SaveReport()
		{
			SaveFileDialog sfDlg = new SaveFileDialog();

			try
			{

				sfDlg.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
				sfDlg.Filter = "Microsoft Office Excel Workbook (*.xls*)|*.xls*";

				sfDlg.RestoreDirectory = true;
				if( sfDlg.ShowDialog() == DialogResult.OK )
				{
					ReportGlobals.XLMgr.SaveWorkbookAs( ReportGlobals.defaultWorkbookName, sfDlg.FileName );
				}
			}
			catch( Exception exc )
			{
				MessageBox.Show(
						exc.Message,
						"Aborted",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error );
			}
			finally
			{
				sfDlg.Dispose();
			}
		}
	}
}
