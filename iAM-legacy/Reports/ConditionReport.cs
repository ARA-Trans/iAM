using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Reflection;

namespace Reports
{
	public class ConditionReport
	{
		private string m_assetName;
		private string m_userName;
		private string m_imagePath;
		//private string m_propertyBin;	//m_propertyBin never assigned to, will always be null
		//private string m_conditionProperty;
		private string m_criteriaFilter;

		private List<IConditionReport> m_conditionReports = new List<IConditionReport>();

		public ConditionReport()
		{
 
		}

		public ConditionReport(string assetName, string userName, string imagePath, string criteriaFilter)
		{
			m_assetName = assetName;
			m_userName = userName;
			m_imagePath = imagePath;
			m_criteriaFilter = criteriaFilter;

			Type[] tarray = null;
			string test = @"C:\Documents and Settings\cbecker\Desktop\Reports.dll";
			Assembly a;
			
			//try
			//{
				a = Assembly.LoadFrom(test);
				tarray = a.GetExportedTypes();
			//}
			//catch (Exception ex)
			//{

			//}
			foreach (Type type in tarray)
			{
				if (type.BaseType != null)
				{
					if (type.BaseType.Name == "ConditionReport")
					{

					}
				}
			}
		}

		/// <summary>
		/// Creates a header at the specified start and end cells.
		/// </summary>
		/// <param name="startHeader">Header start cell</param>
		/// <param name="endHeader">Header end cell.</param>
		/// <param name="repeatRow">Number of rows till header is repeated.</param>
		/// <param name="numRepeats">Number of times to repeat the header.</param>
		public void CreateHeader(string startHeader, string endHeader, int repeatRow, int numRepeats)
		{
			string startRow = startHeader[1].ToString();
			string cellSelection = startHeader + ":" + endHeader;

			ReportGlobals.XLMgr.InsertImage(m_imagePath);
			object[,] titleData = new object[1, 1];
			titleData[0, 0] = m_assetName + " Condition Report";

			ReportGlobals.XLMgr.AdjustRowHeight(startRow, startRow, 36);

			ReportGlobals.XLMgr.SetValues(startHeader, titleData);
			ReportGlobals.XLMgr.MergeCells(cellSelection, false);

			ReportGlobals.XLMgr.ChangeBackgroundColor(cellSelection, Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeHorizontalAlignment(cellSelection, XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment(cellSelection, XlVAlign.xlVAlignCenter);
			ReportGlobals.XLMgr.ChangeFontSize(cellSelection, 16);
			ReportGlobals.XLMgr.ChangeForegroundColor(cellSelection, Color.White);
		}

		public void CreateSubHeader(string startSubHeader, string endSubHeader, int repeatRow, int numRepeats)
		{
			// Get the data for the sub header
			string cellSelection = startSubHeader + ":" + endSubHeader;
			object[,] subHeaderData = new object[1, 1];
			//subHeaderData[0, 0] = "Total Length (mi) of Condition per " + m_propertyBin + ": " + m_criteriaFilter; //m_propertyBin never assigned to, will always be null
			ReportGlobals.XLMgr.SetValues(startSubHeader, subHeaderData);
			ReportGlobals.XLMgr.MergeCells(cellSelection, false);

			ReportGlobals.XLMgr.ChangeBackgroundColor(cellSelection, Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeHorizontalAlignment(cellSelection, XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment(cellSelection, XlVAlign.xlVAlignCenter);
			ReportGlobals.XLMgr.ChangeFontSize(cellSelection, 14);
			ReportGlobals.XLMgr.ChangeForegroundColor(cellSelection, Color.White);
		}

		public object[,] LoadData(List<BinPropertyData> binnedPropertyData, List<string> propertyBins, List<string> conditionBins, Dictionary<string, double> conditionTotals)
		{
			object[,] conditionTable = new object[propertyBins.Count + 2, conditionBins.Count + 1];
			conditionTable[0, 0] = binnedPropertyData[0].m_propertyBinTitle;
			conditionTable[propertyBins.Count + 1,0] = "Totals: ";

			// Set up the condtion columns
			int i = 0;
			int j = 0;
			foreach (string condition in conditionBins)
			{
				i++;
				conditionTable[0, i] = condition;
			}
			i = 0;
			// Set up the propertyBin rows
			foreach (string propertyBin in propertyBins)
			{
				i++;
				conditionTable[i, 0] = propertyBin;
			}
			i = 0;
			// Fill in the data from the list of binned properties
			foreach (BinPropertyData bpd in binnedPropertyData)
			{
				i++;
				j = 0;
				foreach (string key in bpd.m_conditionToValue.Keys)
				{
					j++;
					conditionTable[i, j] = bpd.m_conditionToValue[key];
				}
			}
			i++;
			j = 0;
			foreach (string key in conditionTotals.Keys)
			{
				j++;
				conditionTable[i, j] = conditionTotals[key];
			}
			return conditionTable;
		}
	}
}
