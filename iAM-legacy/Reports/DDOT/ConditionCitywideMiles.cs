using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Drawing;

namespace Reports.DDOT
{
	public class ConditionCitywideMiles
	{
		private int m_networkID;
		private string m_userName;
		//private Dictionary<string, double> m_PCIBins;

		public ConditionCitywideMiles(int networkID, string userName)
		{
			m_networkID = networkID;
			m_userName = userName;
		}

		public void CreateConditionCitywidePercentReport()
		{
			List<BinAttribute> binAttributes = new List<BinAttribute>();
			Dictionary<string, double> binnedPCIData = new Dictionary<string, double>();

			binAttributes.Add(new BinAttribute(10, 0, "FAILED"));
			binAttributes.Add(new BinAttribute(25, 10, "SERIOUS"));
			binAttributes.Add(new BinAttribute(40, 25, "VERY POOR"));
			binAttributes.Add(new BinAttribute(55, 40, "POOR"));
			binAttributes.Add(new BinAttribute(70, 55, "FAIR"));
			binAttributes.Add(new BinAttribute(85, 70, "GOOD"));
			binAttributes.Add(new BinAttribute(100, 85, "EXCELLENT"));
			BinAttributeData PCIBins = new BinAttributeData("1", "PCI", "", "", "", "");
			binnedPCIData = PCIBins.CreateAttributeBins(binAttributes, Method.AREA);

			GenerateReport(binnedPCIData);
		}

		private void GenerateReport(Dictionary<string, double> binnedPCIData)
		{
			// Set up the XL workbook and worksheet defaults.
			ReportGlobals.defaultWorkbookName = "ConditionCitywide";
			ReportGlobals.defaultSheetIndex = 1;
			ReportGlobals.XLMgr.CreateNewWorkbook(ReportGlobals.defaultWorkbookName);

			// Create the header for the report.
			ReportGlobals.CreateReportHeader("Condition Citywide (Area)", "", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RoadCare3 Documents\Washington DC\ddot logo.gif", m_userName);

			// Create a two dimensional data object array to give to excel.
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
			ReportGlobals.XLMgr.SetBorders("A7:" + endColumnLetter + "7", XlBordersIndex.xlEdgeTop, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A7:" + endColumnLetter + "7", XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.SetBorders("A8:" + endColumnLetter + "8", XlBordersIndex.xlEdgeBottom, XlBorderWeight.xlMedium, XlLineStyle.xlContinuous);
			ReportGlobals.XLMgr.ChangeHorizontalAlignment("A7:" + endColumnLetter + "7", XlHAlign.xlHAlignCenter);

			// Set every other column to highlight starting with column A
			ReportGlobals.XLMgr.HighlightRanges('A', binnedPCIData.Count - 1, "7", "8", Color.FromArgb(197, 217, 241));
			// Add the data to the sheet
			ReportGlobals.XLMgr.SetValues("A7:" + endColumnLetter + "8", conditionColumns);
			ReportGlobals.XLMgr.AutoSizeColumns("A:" + endColumnLetter);
			
			// Now create the graph of the data
			string chartName = ReportGlobals.XLMgr.CreateVerticalBarChart("A7:" + endColumnLetter + "8", 'A', 200, 425, 315, XlRowCol.xlRows);
			// Fancy up the graph a bit...
			Chart pciCitywide = ReportGlobals.XLMgr.GetChart(chartName);
			pciCitywide.HasTitle = true;
			pciCitywide.ChartTitle.Text = "City PCI of Total Area";
			ReportGlobals.XLMgr.SetXYAxisTitles("Condition", "Area", pciCitywide);
			ReportGlobals.XLMgr.SetSeriesTitle("PCI %", chartName, "0");
			ReportGlobals.SaveReport();
		}
	}
}
