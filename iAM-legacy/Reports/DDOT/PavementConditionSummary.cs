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
	public class PavementConditionSummary
	{
		string userName;
		int networkID;

		public PavementConditionSummary( int netID, string usrName )
		{
			networkID = netID;
			userName = usrName;
		}

		public void CreateReport()
		{
			SetUpWorkBook("PavementConditionSummary");
			CreateReportHeader("Pavement Condition Summary");

			DataSet wards = DBMgr.ExecuteQuery( "SELECT DISTINCT DATA_ FROM WARDS" );
			foreach( DataRow wardRow in wards.Tables[0].Rows )
			{
				AddWardSectionToReport(wardRow[0].ToString());
			}
		}

		private void SetUpWorkBook( string reportName )
		{
			ReportGlobals.XLMgr.CreateNewWorkbook(reportName);
			ReportGlobals.defaultWorkbookName = reportName;
			ReportGlobals.defaultSheetIndex = 1;
		}


		private void AddWardSectionToReport( string wardName )
		{
			string areaAveragePCIQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(PCI.PCI * PCI.AREA)/SUM(PCI.AREA) AS AVG_PCI FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION WHERE WARD.DATA_ = '" + wardName + "' GROUP BY CLASS.DATA_";

			string totalMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM WARD INNER JOIN CLASS ON WARD.FACILITY = CLASS.FACILITY AND WARD.SECTION = CLASS.SECTION INNER JOIN LENGTH ON WARD.FACILITY = LENGTH.FACILITY AND WARD.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' GROUP BY CLASS.DATA_";

			string lowestPCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI < '25' GROUP BY CLASS.DATA_";
			string lowPCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '25' AND PCI < '50' GROUP BY CLASS.DATA_";
			string middlePCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '50' AND PCI < '75' GROUP BY CLASS.DATA_";
			string highPCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '75' GROUP BY CLASS.DATA_";

			double goodThreshold = DDOTGlobals.Threshold("IRI", "Good");
			double fairThreshold = DDOTGlobals.Threshold("IRI", "Fair");
			double poorThreshold = DDOTGlobals.Threshold("IRI", "Poor");

			string goodPCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '" + goodThreshold + "' GROUP BY CLASS.DATA_";
			string fairPCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '" + fairThreshold + "' AND PCI.PCI < '" + goodThreshold + "' GROUP BY CLASS.DATA_";
			string poorPCIMilesQuery = "SELECT CLASS.DATA_ AS FUNCTIONAL_CLASS, SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN CLASS ON PCI.FACILITY = CLASS.FACILITY AND PCI.SECTION = CLASS.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '" + poorThreshold + "' AND PCI.PCI < '" + fairThreshold + "' GROUP BY CLASS.DATA_";

			string aggregateAreaAveragePCIQuery = "SELECT SUM(PCI.PCI * PCI.AREA)/SUM(PCI.AREA) AS AVG_PCI FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION WHERE WARD.DATA_ = '" + wardName + "'";
			string aggregateTotalMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM WARD INNER JOIN LENGTH ON WARD.FACILITY = LENGTH.FACILITY AND WARD.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "'";

			string aggregateLowestPCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI < '25'";
			string aggregateLowPCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '25' AND PCI.PCI < '50'";
			string aggregateMiddlePCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '50' AND PCI.PCI < '75'";
			string aggregateHighPCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '75'";

			string aggregateGoodPCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '" + goodThreshold + "'";
			string aggregateFairPCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '" + fairThreshold + "' AND PCI.PCI < '" + goodThreshold + "'";
			string aggregatePoorPCIMilesQuery = "SELECT SUM(LENGTH.DATA_) / 5280 AS DIST_LENGTH FROM PCI INNER JOIN WARD ON PCI.FACILITY = WARD.FACILITY AND PCI.SECTION = WARD.SECTION INNER JOIN LENGTH ON PCI.FACILITY = LENGTH.FACILITY AND PCI.SECTION = LENGTH.SECTION WHERE WARD.DATA_ = '" + wardName + "' AND PCI.PCI >= '" + poorThreshold + "' AND PCI.PCI < '" + fairThreshold + "'";
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
