using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using Microsoft.Office.Interop.Excel;
using System.Drawing;

namespace Reports.MDSHA
{
	public class AssetInventory
	{
		private string m_assetName;
		private string m_filters;
		private string m_userName;
		private List<string> m_assetColumns;

		public AssetInventory(string assetName, string userName, string filters, List<string> columns)
		{			
			m_assetName = assetName;
			m_userName = userName;
			m_filters = filters;
			m_assetColumns = columns;
		}

		public void CreateAssetInventoryReport()
		{
			string query = "";
			if(m_filters != "")
			{
				query = "SELECT OBJECTID, ROUTEID, MUN_SORT, ID_PREFIX, MP_SUFFIX, MAIN_LINE, MP_DIRECTI, ASSOC_ID_P, EXIT_NUMBE, RAMP_NUMBE, DISTRICT, MAINT_SHOP, ID_RTE_NO, COUNTY_NO, GPS_Date, Northing, Easting, POLE__, Pole_Heigh, BASE_TYPE, ARM_LENGTH, Num_Lights, BULB_TYPE, WATTAGE, COMMENT_, ON_OFF FROM " + m_assetName + " WHERE " + m_filters;
				//query = "SELECT ";
				//foreach(string columnName in m_assetColumns)
				//{
				//    query += columnName + ", ";
				//}
				//query = query.Substring(0, query.Length - 2);
				//query += " WHERE " + m_filters;
			}
			else
			{
				query = "SELECT OBJECTID, ROUTEID, MUN_SORT, ID_PREFIX, MP_SUFFIX, MAIN_LINE, MP_DIRECTI, ASSOC_ID_P, EXIT_NUMBE, RAMP_NUMBE, DISTRICT, MAINT_SHOP, ID_RTE_NO, COUNTY_NO, GPS_Date, Northing, Easting, POLE__, Pole_Heigh, BASE_TYPE, ARM_LENGTH, Num_Lights, BULB_TYPE, WATTAGE, COMMENT_, ON_OFF FROM " + m_assetName;
				//query = "SELECT ";
				//foreach(string columnName in m_assetColumns)
				//{
				//    query += columnName + ", ";
				//}
				//query = query.Substring(0, query.Length - 2);
				//query += " WHERE " + m_filters;
			}
			DataSet assetInfo = DBMgr.ExecuteQuery(query);

			int numAssetColumns = assetInfo.Tables[0].Columns.Count;
			int asciiA = Convert.ToInt32('A');
			int endColumnNumber = asciiA + numAssetColumns - 1;
			char endColumnLetter = Convert.ToChar(endColumnNumber);

			// Set up the XL workbook and worksheet defaults.
			ReportGlobals.defaultWorkbookName = "ASSET_REPORT";
			ReportGlobals.defaultSheetIndex = 1;
			ReportGlobals.XLMgr.CreateNewWorkbook(ReportGlobals.defaultWorkbookName);
			ReportGlobals.XLMgr.FormatAllCellsAsText();

			CreateAssetInventoryHeader(@".\SHA.png", "" , m_assetName + " Inventory Report", endColumnLetter);
			CreateAssetInventorySubHeader(endColumnLetter);
			PopulateAssetInventoryData(assetInfo, endColumnLetter);

			//CreateAssetInventoryFooter();
			ReportGlobals.SaveReport();
		}

		private void PopulateAssetInventoryData(DataSet assetInfo, char endColumnLetter)
		{
			object[,] headingData = new object[1, 27];
			headingData[0, 0] = "OBJECTID";
			headingData[0,1] = "ROUTEID";
			headingData[0,2] = "MUN_SORT";
			headingData[0,3] = "ID_PREFIX";
			headingData[0,4] = "MP_SUFFIX";
			headingData[0,5] = "MAIN_LINE";
			headingData[0,6] = "MP_DIRECTI";
			headingData[0,7] = "ASSOC_ID_P";
			headingData[0,8] = "EXIT_NUMBE";
			headingData[0,9] = "RAMP_NUMBE";
			headingData[0,10] = "DISTRICT";
			headingData[0,11] = "MAINT_SHOP";
			headingData[0,12] = "ID_RTE_NO";
			headingData[0,13] = "COUNTY_NO";
			headingData[0,14] = "GPS_Date";
			headingData[0,15] = "Northing";
			headingData[0,16] = "Easting";
			headingData[0,17] = "POLE__";
			headingData[0,18] = "Pole_Heigh";
			headingData[0,19] = "BASE_TYPE";
			headingData[0,20] = "ARM_LENGTH";
			headingData[0,21] = "Num_Lights";
			headingData[0,22] = "BULB_TYPE";
			headingData[0,23] = "WATTAGE";
			headingData[0,24] = "COMMENT";
			headingData[0, 25] = "ON_OFF";

			ReportGlobals.XLMgr.SetValues("A5:" + endColumnLetter + "5", headingData);
			ReportGlobals.XLMgr.ChangeBackgroundColor("A5:" + endColumnLetter + "5", Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeForegroundColor("A5:" + endColumnLetter + "5", Color.White);

			object[,] objectData = new object[assetInfo.Tables[0].Rows.Count, assetInfo.Tables[0].Columns.Count];
			for(int i = 0; i < assetInfo.Tables[0].Rows.Count; i++)
			{
				for (int j = 0; j < assetInfo.Tables[0].Rows[i].ItemArray.Length; j++)
				{
					objectData[i,j] = assetInfo.Tables[0].Rows[i][j];
				}
			}

			ReportGlobals.XLMgr.SetValues("A6:" + endColumnLetter + assetInfo.Tables[0].Rows.Count, objectData);
		}

		private void CreateAssetInventorySubHeader(char endColumn)
		{
			// Get the data for the sub header
			object[,] subHeaderData = new object[1, 1];
			subHeaderData[0, 0] = "RoadCare Filter Summary: " + m_filters;
			ReportGlobals.XLMgr.SetValues("A3", subHeaderData);
			ReportGlobals.XLMgr.MergeCells("A3:" + endColumn + "3", false);

			ReportGlobals.XLMgr.ChangeBackgroundColor("A3:" + endColumn + "3", Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeHorizontalAlignment("A3:" + endColumn + "3", XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment("A3:" + endColumn + "3", XlVAlign.xlVAlignCenter);
			//ReportGlobals.XLMgr.ChangeFontSize("A3:" + endColumn + "3", 12);
			ReportGlobals.XLMgr.ChangeForegroundColor("A3:" + endColumn + "3", Color.White);

			
		}

		private void CreateAssetInventoryFooter()
		{

		}

		private void CreateAssetInventoryHeader(string imagePath, string preparedBy, string title, char endColumn)
		{
			ReportGlobals.XLMgr.InsertImage(imagePath);
			object[,] titleData = new object[1, 1];
			titleData[0, 0] = title;

			ReportGlobals.XLMgr.AdjustRowHeight("1", "1", 36);

			ReportGlobals.XLMgr.SetValues("A1", titleData);
			ReportGlobals.XLMgr.MergeCells("A1:" + endColumn + "1", false);

			ReportGlobals.XLMgr.ChangeBackgroundColor("A1:" + endColumn + "1", Color.FromArgb(0, 0, 211));
			ReportGlobals.XLMgr.ChangeHorizontalAlignment("A1:" + endColumn + "1", XlHAlign.xlHAlignCenter);
			ReportGlobals.XLMgr.ChangeVerticalAlignment("A1:" + endColumn + "1", XlVAlign.xlVAlignCenter);
			ReportGlobals.XLMgr.ChangeFontSize("A1:" + endColumn + "1", 16);
			ReportGlobals.XLMgr.ChangeForegroundColor("A1:" + endColumn + "1", Color.White);
		}
	}
}
