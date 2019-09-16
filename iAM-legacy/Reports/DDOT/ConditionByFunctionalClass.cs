using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using DatabaseManager;
using System.Data;

namespace Reports.DDOT
{
	public class ConditionByFunctionalClass
	{
		private int m_networkID;
		private string m_userName;

		public ConditionByFunctionalClass(int networkID, string userName)
		{
			m_networkID = networkID;
			m_userName = userName;
		}

		public void CreateConditionByFunctionalClassReport()
		{
			List<BinAttribute> binAttributes = new List<BinAttribute>();
			Dictionary<string, double> binnedPCIData;
			List<BinnedData> pciBins = new List<BinnedData>();

			binAttributes.Add(new BinAttribute(10, 0, "FAILED"));
			binAttributes.Add(new BinAttribute(25, 10, "SERIOUS"));
			binAttributes.Add(new BinAttribute(40, 25, "VERY POOR"));
			binAttributes.Add(new BinAttribute(55, 40, "POOR"));
			binAttributes.Add(new BinAttribute(70, 55, "FAIR"));
			binAttributes.Add(new BinAttribute(85, 70, "GOOD"));
			binAttributes.Add(new BinAttribute(100, 85, "EXCELLENT"));

			BinAttributeData PCIBinsPerFunctionalClass;
			string functionalClass = "";
			
			string query = "SELECT DISTINCT CLASS FROM SEGMENT_" + m_networkID + "_NS0";
			DataSet distinctClasses = DBMgr.ExecuteQuery(query);
			foreach (DataRow distinctClass in distinctClasses.Tables[0].Rows)
			{
				BinnedData bin = new BinnedData();
				if(distinctClass[0] != null)
				{
					functionalClass = distinctClass[0].ToString();
					PCIBinsPerFunctionalClass = new BinAttributeData(m_networkID.ToString(), "PCI", m_userName, "", "CLASS = '" + functionalClass + "'", "");
					binnedPCIData = PCIBinsPerFunctionalClass.CreateAttributeBins(binAttributes, Method.PERCENTAGE);
					
					bin.m_binnedPCIData = binnedPCIData;
					bin.m_functionalClass = functionalClass;
				}
				pciBins.Add(bin);
			}
			GenerateReport(pciBins);
		}

		private void GenerateReport(List<BinnedData> pciBins)
		{
			// Set up the XL workbook and worksheet defaults.
			ReportGlobals.defaultWorkbookName = "ConditionCitywide";
			ReportGlobals.defaultSheetIndex = 1;
			ReportGlobals.XLMgr.CreateNewWorkbook(ReportGlobals.defaultWorkbookName);

			// Create the header for the report.
			//ReportGlobals.CreateReportHeader("Condition Citywide (Area)", "", @"C:\Documents and Settings\cbecker\My Documents\RoadCare3 Documents\Washington DC\ddot logo.gif", m_userName);
			ReportGlobals.CreateReportHeader( "Condition Citywide (Area)", "", "", m_userName );
			int startRow = 6;
			for (int i = 0; i < pciBins.Count; i++)
			{
				CreateFunctionalClassSection(pciBins[i].m_binnedPCIData, startRow.ToString(), pciBins[i].m_functionalClass);
				startRow += 3;
			}
			ReportGlobals.SaveReport();
		}

		private void CreateFunctionalClassSection(Dictionary<string, double> binnedPCIData, string startRow, string functionalClass)
		{
			// Create the functional class header
			CreateSubHeader("Functional Class " + functionalClass);

			// Create a two dimensional data object array to give to excel.
			string currentRow = Report.GetNextRowNumber(startRow).ToString();
			string nextRow = Report.GetNextRowNumber(currentRow).ToString();
			object[,] conditionColumns = new object[2, binnedPCIData.Count];
			int i = 0;
			// Loop through the binned PCI data and set the first row to be headers for the data row
			foreach (string key in binnedPCIData.Keys)
			{
				conditionColumns[0, i] = key;
				i++;
			}
			// Now loop through and set the data row of the object array
			i = 0;
			foreach (string key in binnedPCIData.Keys)
			{
				conditionColumns[1, i] = binnedPCIData[key];
				i++;
			}
			// Get out to which column letter we are pasting data.
			int asciiA = Convert.ToInt32('A');
			int endColumnNumber = asciiA + binnedPCIData.Count - 1;
			char endColumnLetter = Convert.ToChar(endColumnNumber);

			// Format data cells
			ReportGlobals.XLMgr.SetBorders("A" + currentRow + ":" + endColumnLetter + currentRow, XlBordersIndex.xlEdgeTop, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A" + currentRow + ":" + endColumnLetter + currentRow, XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A" + nextRow + ":" + endColumnLetter + nextRow, XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.ChangeHorizontalAlignment("A" + currentRow + ":" + endColumnLetter + currentRow, XlHAlign.xlHAlignCenter);

			// Set every other column to highlight starting with column A
			ReportGlobals.XLMgr.HighlightRanges('A', binnedPCIData.Count - 1, currentRow, nextRow, Color.FromArgb(197, 217, 241));

			// Add the data to the sheet
			ReportGlobals.XLMgr.SetValues("A" + currentRow + ":" + endColumnLetter + nextRow, conditionColumns);
			ReportGlobals.XLMgr.AutoSizeColumns("A:" + endColumnLetter);

			// Now create the graph of the data
			string chartName = ReportGlobals.XLMgr.CreateVerticalBarChart("A" + currentRow + ":" + endColumnLetter + nextRow, 'A', 200, 425, 315, XlRowCol.xlRows);
			// Fancy up the graph a bit...
			Chart pciCitywide = ReportGlobals.XLMgr.GetChart(chartName);
			pciCitywide.HasTitle = true;
			pciCitywide.ChartTitle.Text = "Condition By Functional Class " + functionalClass;
			ReportGlobals.XLMgr.SetXYAxisTitles("Condition", "Percent Area", pciCitywide);
			ReportGlobals.XLMgr.SetSeriesTitle("PCI %", chartName, "0");
		}

		private string CreateSubHeader(string title)
		{
			string lastRow = "";


			return lastRow;
		}

		private string CreateBarGraph(string title)
		{
			string lastRow = "";
			return lastRow;
		}

	}

	public class BinnedData
	{
		public Dictionary<string, double> m_binnedPCIData;
		public string m_functionalClass;
	}
}
