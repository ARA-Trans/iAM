using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Validation_;
using DatabaseManager;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace Reports.DDOT
{
	public class PageViewForSegments
	{
		Segment reportData;
		string userName;
		int networkID;

		public PageViewForSegments( int netID, string usrName, Segment reportBasis )
		{
			networkID = netID;
			userName = usrName;
			reportData = reportBasis;
		}

		public void CreateReport()
		{
			//general case properties
			string fromStreet = GetProperty("SECTION_FROM");
			string toStreet = GetProperty("SECTION_TO");
			string owner = GetProperty("DOMAIN");
			string functionalClass = GetProperty("DOMAIN");
			double length = double.Parse(GetProperty("LENGTH"));
			double lanes = double.Parse(GetProperty("LANES"));


			//specific properties
			string areaQuery = "SELECT AREA FROM PCI WHERE FACILITY = '" + reportData.Facility + "' AND SECTION = '" + reportData.Section + "'";
			double area = double.Parse(DBMgr.ExecuteQuery(areaQuery).Tables[0].Rows[0][0].ToString());
			double width = area / length;

			string distressQuery = "SELECT DISTRESS, SEVERITY, EXTENT, PCI FROM PCI_DETAIL INNER JOIN PCI ON PCI_DETAIL.PCI_ID = PCI.ID_ WHERE FACILITY = '" + reportData.Facility + "' AND SECTION = '" + reportData.Section + "' AND DATE_ IN (SELECT MAX(DATE_) FROM PCI_DETAIL INNER JOIN PCI ON PCI_DETAIL.PCI_ID = PCI.ID_ WHERE FACILITY = '" + reportData.Facility + "' AND SECTION = '" + reportData.Section + "')";

			//string simulationQuery = "";

			SetUpWorkBook("PageViewForSegments");
			CreateReportHeader("Page View For Segments");
		}

		private string GetProperty( string property )
		{
			return DBMgr.ExecuteQuery(GeneratePropertyQuery(property)).Tables[0].Rows[0][0].ToString();
		}

		private string GeneratePropertyQuery( string property )
		{
			return "SELECT DATA AS "
				+ property
				+ " FROM "
				+ property
				+ " WHERE "
				+ property
				+ ".FACILITY = '"
				+ reportData.Facility
				+ "' AND "
				+ property
				+ ".SECTION = '"
				+ reportData.Section
				+ "'";
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
