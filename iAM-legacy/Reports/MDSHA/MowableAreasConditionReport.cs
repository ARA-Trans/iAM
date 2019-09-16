using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Drawing;

namespace Reports.MDSHA
{
	public class MowableAreasConditionReport : ConditionReport, IConditionReport
	{
		private string m_assetName;
		private string m_userName;
		private string m_conditionProperty;
		private string m_propertyBin;
		private string m_imagePath;
		private string m_criteriaFilter;
		private List<BinPropertyData> m_binnedProperties = new List<BinPropertyData>();

		public MowableAreasConditionReport(string assetName, string userName, string conditionProperty, string propertyBin, string imagePath, string criteriaFilter)
		{
			m_assetName = assetName;
			m_userName = userName;
			m_conditionProperty = conditionProperty;
			m_propertyBin = propertyBin;
			m_imagePath = imagePath;
			m_criteriaFilter = criteriaFilter;
		}

		#region IConditionReport Members

		public void CreateCondtionReport()
		{
			//string query = "";
			//string conditionQuery = "";
			//List<string> conditionBins = new List<string>();
			//List<string> propertyBins = new List<string>();
			//Dictionary<string, double> conditionTotals = new Dictionary<string, double>();

			//// Get the distinct propertyBin types
			//query = "SELECT DISTINCT " + m_propertyBin + " FROM MOW_AREAS";
			//DataSet distinctPropertyBins = DBMgr.ExecuteQuery(query);
			//foreach(DataRow propertyBin in distinctPropertyBins.Tables[0].Rows)
			//{
			//    propertyBins.Add(propertyBin[m_propertyBin].ToString());
			//}
			//int numDistinctPropertyBins = propertyBins.Count;

			//// Get the distinct condition levels
			//conditionQuery = "SELECT DISTINCT " + m_conditionProperty + " FROM MOW_AREAS";
			//DataSet distinctConditions = DBMgr.ExecuteQuery(conditionQuery);
			//foreach (DataRow distinctCondition in distinctConditions.Tables[0].Rows)
			//{
			//    conditionBins.Add(distinctCondition[m_conditionProperty].ToString());
			//    conditionTotals.Add(distinctCondition[m_conditionProperty].ToString(),0);
			//}
			//int numCondtionColumns = conditionBins.Count;

			//DataSet sumValues = null;
			
			//// Loop through each propertyBin and sum the area for each condition level.
			//BinPropertyData propertyBinData;
			//foreach (DataRow propertyBinRow in distinctPropertyBins.Tables[0].Rows)
			//{
			//    if (propertyBinRow[m_propertyBin] != null)
			//    {
			//        propertyBinData = new BinPropertyData();
			//        foreach(DataRow conditionRow in distinctConditions.Tables[0].Rows)
			//        {
			//            if (conditionRow[m_conditionProperty] != null)
			//            {
			//                if (m_criteriaFilter == "")
			//                {
			//                    conditionQuery = "SELECT SUM(CONVERT(float, AREA)) AS BIN_DATA FROM MOW_AREAS WHERE " + m_propertyBin + " = '" + propertyBinRow[m_propertyBin].ToString() + "' AND " + m_conditionProperty + " = '" + conditionRow[m_conditionProperty].ToString() + "'";
			//                }
			//                else
			//                {
			//                    conditionQuery = "SELECT SUM(CONVERT(float, AREA)) AS BIN_DATA FROM MOW_AREAS WHERE " + m_propertyBin + " = '" + propertyBinRow[m_propertyBin].ToString() + "' AND " + m_conditionProperty + " = '" + conditionRow[m_conditionProperty].ToString() + "' AND " + m_criteriaFilter;
			//                }
			//                sumValues = DBMgr.ExecuteQuery(conditionQuery);
			//                propertyBinData.SetBinPropertyData(m_propertyBin, conditionRow[m_conditionProperty].ToString(), sumValues.Tables[0].Rows[0]["BIN_DATA"].ToString(), propertyBinRow[m_propertyBin].ToString());
			//            }
			//            conditionTotals[conditionRow[m_conditionProperty].ToString()] += double.Parse(sumValues.Tables[0].Rows[0]["BIN_DATA"].ToString());
			//        }
			//        m_binnedProperties.Add(propertyBinData);
			//    }
			//}
			
			//int asciiA = Convert.ToInt32('A');
			//int endColumnNumber = asciiA + numCondtionColumns;
			//char endColumnLetter = Convert.ToChar(endColumnNumber);

			//// Create the default XL 
			//// Set up the XL workbook and worksheet defaults.
			//ReportGlobals.defaultWorkbookName = "ASSET_CONDTION_REPORT";
			//ReportGlobals.defaultSheetIndex = 1;
			//ReportGlobals.XLMgr.CreateNewWorkbook(ReportGlobals.defaultWorkbookName);

			//ConditionReport cr = new ConditionReport(m_assetName, m_userName, m_imagePath, m_propertyBin, m_conditionProperty, m_criteriaFilter);

			//cr.CreateHeader();
			//cr.CreateSubHeader();

			//ReportGlobals.XLMgr.AdjustColumnWidth("A", endColumnLetter.ToString(), 11);
			//object[,] conditionData = cr.LoadData(m_binnedProperties, propertyBins, conditionBins, conditionTotals);
			//ReportGlobals.XLMgr.SetValues("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), conditionData);
			//ReportGlobals.XLMgr.ChangeHorizontalAlignment("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter);
			//ReportGlobals.XLMgr.SetBorders("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideHorizontal, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
			//ReportGlobals.XLMgr.SetBorders("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlBordersIndex.xlInsideVertical, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);

			//ReportGlobals.XLMgr.SetBorders("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
			//ReportGlobals.XLMgr.SetBorders("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
			//ReportGlobals.XLMgr.SetBorders("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
			//ReportGlobals.XLMgr.SetBorders("A5:" + endColumnLetter + (numCondtionColumns + 2).ToString(), Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);

			//ReportGlobals.XLMgr.ChangeBackgroundColor("A5:" + endColumnLetter + numCondtionColumns.ToString(), Color.LightGray);
			//ReportGlobals.XLMgr.ChangeBackgroundColor("A" + (numDistinctPropertyBins + 6).ToString() + ":" + endColumnLetter + (numCondtionColumns + 2).ToString(), Color.LightGray);
			//CreatePieChart(endColumnLetter, numCondtionColumns);



			//ReportGlobals.SaveReport();
		}

		private void CreatePieChart(char endColumnLetter, int numConditionColumns)
		{
			//ReportGlobals.XLMgr.CreatePieChart("B6:" + endColumnLetter + (numConditionColumns + 1).ToString(), "A20", "Mowable Areas Condition per " + m_propertyBin);
		}

		#endregion
	}
}
