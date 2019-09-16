using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

using DatabaseManager;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Office.Interop.Excel;


using System.Data.SqlClient;

namespace Reports
{
    public class AttributeViewReport
    {
        private DataGridView m_dgv;
        private String m_strNetworkName;

        public AttributeViewReport(DataGridView dgv, String strNetworkName)
        {
            m_dgv = dgv;
            m_strNetworkName = strNetworkName;
        }

        public void CreateAttributeReport()
        {
            try
            {
                Report.XL.Visible = false;
                Report.XL.UserControl = false;
                Microsoft.Office.Interop.Excel._Workbook oWB = Report.CreateWorkBook();
                Microsoft.Office.Interop.Excel._Worksheet oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
    			//I don't have aralogo.jpg...
				object oEndHeaderCell = Report.CreateHeader( ".\\sha.png", oSheet, "A1", "Attribute View Report", "Network: " + m_strNetworkName, "Pavement attribute data for network - " + m_strNetworkName );

                int iEndHeaderRow = Report.GetNextRowNumber(oEndHeaderCell.ToString());
                iEndHeaderRow++; // One row of white space between header and data.
                object oStartData = "A" + iEndHeaderRow.ToString();
				//Report.PlaceReportGraphic(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoadCare Projects\\" + drPage["reportGraphicFile"].ToString(), "A1", oSheet);
                Report.WriteDGVToExcel(m_dgv, oSheet, oStartData, true);
                Report.XL.Visible = true;
                Report.XL.UserControl = true;
            }
            catch (Exception exc)
            {
                throw exc;
            }

            //oWB.SaveAs("test", XlFileFormat.xlHtml, Missing.Value, Missing.Value, Missing.Value, Missing.Value, XlSaveAsAccessMode.xlExclusive, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            
            //xlChart.ChartType = XlChartType.xlXYScatter;
            
            //xlChart.HasLegend = false;
            //xlChart.SetSourceData(oRng, Missing.Value);

            //oChart.ChartWizard(oRng, Microsoft.Office.Interop.Excel.XlChartType.xlXYScatter, Missing.Value,
            //Microsoft.Office.Interop.Excel.XlRowCol.xlColumns, Missing.Value, Missing.Value, Missing.Value,
            //Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            //oChart.SetSourceData(oRng, Missing.Value);
        } 
    }
}
