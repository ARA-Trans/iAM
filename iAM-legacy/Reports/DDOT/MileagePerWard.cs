﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ReportExporters.Common;
using ReportExporters.Common.Adapters;
using ReportExporters.WinForms.Adapters;
using ReportExporters.WinForms;
using System.IO;
using System.Data;
using DatabaseManager;
using ReportExporters.Common.Exporting;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Drawing;
//using System.Data;

namespace Reports.DDOT
{
	public class MileagePerWard
	{
		private int m_networkID;
		private string m_userName;

		public MileagePerWard(int networkID, string userName)
        {
			m_networkID = networkID;
			m_userName = userName;
        }

		public void CreateMileagePerWardReport()
		{
			string query = "SELECT WARD_NUM, SUM(WARD_LENGTH) / 5280 AS WARD_LENGTH FROM (SELECT WARD.DATA_ AS WARD_NUM, MAX(LENGTH.DATA_) AS WARD_LENGTH FROM LENGTH INNER JOIN WARD ON WARD.FACILITY = LENGTH.FACILITY AND WARD.SECTION = LENGTH.SECTION GROUP BY WARD.DATA_, LENGTH.FACILITY, LENGTH.SECTION) AS DERIVEDTABLE GROUP BY DERIVEDTABLE.WARD_NUM";
			DataSet distinctWards = DBMgr.ExecuteQuery(query);

			SetUpWorkBook("MileagePerWard");
			CreateReportHeader("Mileage Per Ward");

			CreateReportRows( distinctWards.Tables[0] );
			SaveReport();

			ReportGlobals.XLMgr.CloseWorkbook( "MileagePerWard" );
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
			reportData[0, 0] = "Ward";
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
			ReportGlobals.XLMgr.SetValues( "MileagePerWard", 1, "D7:E" + ( numRows + 7 ).ToString(), reportData );
			ReportGlobals.XLMgr.ChangeFontSize( "MileagePerWard", 1, "A7:I7", 16 );
			ReportGlobals.XLMgr.ChangeBackgroundColor( "MileagePerWard", 1, cellsToColor, Color.FromArgb( 197, 217, 241 ) );
			ReportGlobals.XLMgr.ChangeHorizontalAlignment( "MileagePerWard", 1, "A1:I" + ( numRows + 7 ).ToString(), XlHAlign.xlHAlignCenter );
			ReportGlobals.XLMgr.MergeCells( "MileagePerWard", 1, "A7:D" + ( numRows + 7 ).ToString(), true );
			ReportGlobals.XLMgr.MergeCells( "MileagePerWard", 1, "E7:I" + ( numRows + 7 ).ToString(), true );

			ReportGlobals.XLMgr.SetBorders( "MileagePerWard", 1, "A7:I7", XlBordersIndex.xlInsideHorizontal, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous );
			ReportGlobals.XLMgr.SetBorders( "MileagePerWard", 1, "A7:I7", XlBordersIndex.xlInsideVertical, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous );

			ReportGlobals.XLMgr.SetBorders( "MileagePerWard", 1, "A1:D" + ( numRows + 7 ).ToString(), XlBordersIndex.xlEdgeRight, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous );
	
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
