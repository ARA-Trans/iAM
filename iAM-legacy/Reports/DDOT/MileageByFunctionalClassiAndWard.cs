using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace Reports.DDOT
{
	public class MileageByFunctionalClassiAndWard
	{
		int networkID;
		string userName;

		public MileageByFunctionalClassiAndWard( int netID, string user )
        {
			networkID = netID;
			userName = user;
        }

		public void CreateReport()
		{
			SetUpWorkBook("MileageByFunctionalClassiAndWard");
			CreateReportHeader("Mileage By Functional Classification & Ward");
			CreateReportRows();

			SaveReport();

			ReportGlobals.XLMgr.CloseWorkbook("MileageByFunctionalClassiAndWard");

		}

		private void CreateReportRows()
		{
			string wardFuncQuery = "SELECT CLASS.DATA_ AS 'FUNC. CLASS', WARD.DATA_ AS WARD, sum(LENGTH.DATA_) / 5280 AS LONGNESS FROM CLASS INNER JOIN LENGTH ON (CLASS.FACILITY = LENGTH.FACILITY AND CLASS.SECTION = LENGTH.SECTION) INNER JOIN WARD ON (CLASS.FACILITY = WARD.FACILITY AND CLASS.SECTION = WARD.SECTION) GROUP BY CLASS.DATA_, WARD.DATA_";
			DataSet wardFuncData = DBMgr.ExecuteQuery(wardFuncQuery);

			string wardsQuery = "SELECT DISTINCT WARD.DATA_ FROM WARD ORDER BY WARD.DATA_";
			DataSet distinctWards = DBMgr.ExecuteQuery(wardsQuery);

			string funcsQuery = "SELECT DISTINCT CLASS.DATA_ FROM CLASS ORDER BY CLASS.DATA_";
			DataSet distinctFuncs = DBMgr.ExecuteQuery(funcsQuery);

			string aggregateQuery = "SELECT WARD.DATA_ AS WARD, sum(LENGTH.DATA_) / 5280 AS LONGNESS FROM LENGTH INNER JOIN WARD ON (LENGTH.FACILITY = WARD.FACILITY AND LENGTH.SECTION = WARD.SECTION) GROUP BY WARD.DATA_";
			DataSet aggregateData = DBMgr.ExecuteQuery(aggregateQuery);

			List<string> wards = new List<string>();
			List<string> funcs = new List<string>();

			foreach( DataRow wardRow in distinctWards.Tables[0].Rows )
			{
				wards.Add(wardRow[0].ToString());
			}
			foreach( DataRow funcRow in distinctFuncs.Tables[0].Rows )
			{
				funcs.Add(funcRow[0].ToString());
			}

			if( wards[0] != "" )
			{
				wards.Insert(0, "");
			}
			if( funcs[0] != "" )
			{
				funcs.Insert(0, "");
			}

			//we need an extra column for the class names and two extra rows (1 for the column headers and one for the aggregate line)
			int height = funcs.Count + 2;
			int width = wards.Count + 1;
			object[,] writeToExcel = new object[height, width];

			for( int h = 0; h + 1 < height; ++h )
			{
				for( int w = 0; w + 1 < width; ++w )
				{
					writeToExcel[h + 1, w + 1] = 0.0;
				}
			}


			writeToExcel[0, 0] = "Func. Class";
			int columnIndex = 1;
			foreach( string ward in wards )
			{
				writeToExcel[0, columnIndex] = "W - " + ward;
				++columnIndex;
			}
			int rowIndex = 1;
			foreach( string func in funcs )
			{
				writeToExcel[rowIndex, 0] = func;
				++rowIndex;
			}

			writeToExcel[rowIndex, 0]= "Total";

			foreach( DataRow mileageRow in wardFuncData.Tables[0].Rows )
			{
				int columnToIncrement = wards.IndexOf(mileageRow["WARD"].ToString());
				int rowToIncrement = funcs.IndexOf(mileageRow["Func. Class"].ToString());

				writeToExcel[rowToIncrement + 1, columnToIncrement + 1] = ( (double) writeToExcel[rowToIncrement + 1, columnToIncrement + 1] ) + double.Parse(mileageRow["LONGNESS"].ToString());
			}

			columnIndex = 1;
			foreach( string ward in wards )
			{
				double sum = 0.0;
				for( int i = 0; i < funcs.Count; ++i )
				{
					sum += ( (double) writeToExcel[i + 1, columnIndex] );
				}
				writeToExcel[rowIndex, columnIndex] = sum;
				++columnIndex;
			}

			char finalLetter = Convert.ToChar(Convert.ToInt32('A') + width - 1);
			int finalRow = height + 6;

			string rangeSelector = "A7:" + finalLetter + finalRow.ToString();

			ReportGlobals.XLMgr.SetValues(rangeSelector, writeToExcel);

			ReportGlobals.XLMgr.SetNumberFormat(rangeSelector, "0.00");
			//ReportGlobals.XLMgr.AutoSizeColumns("A:" + finalLetter);

		}

		private void SetUpWorkBook( string reportName )
		{
			ReportGlobals.XLMgr.CreateNewWorkbook(reportName);
			ReportGlobals.defaultWorkbookName = reportName;
			ReportGlobals.defaultSheetIndex = 1;
		}

		private void CreateReportHeader( string reportName )
		{
			ReportGlobals.XLMgr.InsertImage(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RoadCare3 Documents\Washington DC\ddot logo.gif");
			object[,] headerData = new object[6, 1];
			headerData[0, 0] = "DDOT - " + reportName;
			headerData[4, 0] = "Prepared by: " + userName;
			headerData[5, 0] = DateTime.Now.ToShortDateString();
			ReportGlobals.XLMgr.SetValues("C1:C6", headerData);

			ReportGlobals.XLMgr.MergeCells("C1:I4", false);
			ReportGlobals.XLMgr.MergeCells("C5:I5", false);
			ReportGlobals.XLMgr.MergeCells("C6:I6", false);

			ReportGlobals.XLMgr.ChangeBackgroundColor("A1:I6", Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeForegroundColor("A1:I6", Color.FromArgb(255, 255, 255));

			ReportGlobals.XLMgr.ChangeHorizontalAlignment("A1:I6", XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment("A1:I6", XlVAlign.xlVAlignCenter);
			ReportGlobals.XLMgr.ChangeFontSize("C1:I4", 13);
			ReportGlobals.XLMgr.ChangeFontSize("C5:I6", 12);

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

				sfDlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				sfDlg.Filter = "Microsoft Office Excel Workbook (*.xls*)|*.xls*";

				sfDlg.RestoreDirectory = true;
				if( sfDlg.ShowDialog() == DialogResult.OK )
				{
					ReportGlobals.XLMgr.SaveWorkbookAs(ReportGlobals.defaultWorkbookName, sfDlg.FileName);
				}
			}
			catch( Exception exc )
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
		}


	}
}
