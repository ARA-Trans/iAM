using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using DatabaseManager;
using Microsoft.Office.Interop.Excel;
using RoadCareDatabaseOperations;

namespace Reports.PennDotBridge
{
    internal class PennDotBaseReport
    {
        public _Worksheet Sheet { get; }
        public string NetworkId { get; }

        public string SimulationId { get; }
        public BridgeAnalysis Analysis { get; }

        public DataSet Network { get; set; }
        public DataSet Simulation { get; set; }

        public DataSet Investment { get; set; }
        public int FirstYear { get; set; }
        public int NumberYears { get; set; }
        public PennDotBaseReport(string networkId, string simulationId, _Worksheet oSheet, BridgeAnalysis analysis)
        {
            Sheet = oSheet;
            NetworkId = networkId;
            SimulationId = simulationId;
            Analysis = analysis;
            Network = DBOp.GetNetworkDesc(NetworkId);
            Simulation = DBOp.QuerySimulations(SimulationId);
            Investment = DBOp.QueryInvestments(SimulationId);
            var drInvestment = Investment.Tables[0].Rows[0];
            FirstYear = Convert.ToInt32(drInvestment["FIRSTYEAR"]);
            NumberYears = Convert.ToInt32(drInvestment["NUMBERYEARS"]);


        }

        public void WriteHeader(string title)
        {
            // calculate the size for the array
            var columns = 1;
            var  rows = 1;
            var oData = new object[rows, columns];


            oData[0, 0] = title;
            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A1", false, false);

            var oR = Sheet.get_Range("A1", "A1");
            oR.ColumnWidth = 25;

            oR = Sheet.get_Range("A1", Report.GetCellAtOffset("A1",0, NumberYears + 1));
            oR.Font.Bold = true;
            oR.Font.Size = 16;
            oR.Font.Name = "Calabri";
            oR.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
            oR.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.RoyalBlue);
            oR.MergeCells = true;
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;

        }

        public void WriteAnalysisInformationFooter(string startCell)
        {
            // calculate the size for the array
            var columns = 2;
            var rows = 7;
            var oData = new object[rows, columns];

            var drNetwork = Network.Tables[0].Rows[0];
            var drSimulations = Simulation.Tables[0].Rows[0];

            oData[0, 0] = "Analysis Information";
            oData[1, 0] = "Database:";
            oData[1, 1] = DBMgr.NativeConnectionParameters.Database;
            oData[2, 0] = "Network Name:";
            oData[2, 1] = drNetwork["NETWORK_NAME"];
            oData[3, 0] = "Network Description:";
            oData[3, 1] = drNetwork["Description"].ToString();
            oData[4, 0] = "Simulation Name:";
            oData[4, 1] = drSimulations["Simulation"].ToString();
            oData[5, 0] = "Simulation Description:";
            oData[5, 1] = drSimulations["Comments"].ToString();
            oData[6, 0] = "Created On:";
            oData[6, 1] = Report.Left(drSimulations["DATE_CREATED"].ToString(), 9);

            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, startCell, false, false);

            var oR = Sheet.get_Range(startCell, Report.GetCellAtOffset(startCell, 0, NumberYears + 1));
            oR.Font.Bold = true;
            oR.Font.Size = 12;
            oR.Font.Name = "Calabri";
            oR.Font.Color = ColorTranslator.ToOle(System.Drawing.Color.White);
            oR.Interior.Color = ColorTranslator.ToOle(System.Drawing.Color.RoyalBlue);
            oR.MergeCells = true;
            oR.HorizontalAlignment = XlHAlign.xlHAlignCenter;

        }

        public void WriteAnalysisYears(string columnHeader)
        {
            var oData = new object[1, NumberYears+1];
            oData[0, 0] = columnHeader;
            for (int i = 0; i < NumberYears; i++)
            {
                oData[0, i+1] = FirstYear + i;
            }
            var endCell = Report.WriteObjectArrayToExcel(oData, Sheet, "A3", false, true);

            var oR = Sheet.get_Range("A3", Report.GetCellAtOffset("A3", 0, NumberYears + 1));
            oR.Font.Bold = true;
            oR.Font.Size = 11;
            oR.Font.Name = "Calabri";


        }


        public void CreateScatter(string chartTitle,string xTitle, string yTitle, int numberSeries)
        {
            var range = Sheet.get_Range(Report.GetCellAtOffset("B5", numberSeries, 0), Report.GetCellAtOffset("A3", 20+numberSeries, NumberYears));
            //Worksheet operationalWorksheet = (Worksheet)workbookIndex[workbook].Sheets[sheetIndex];
            //Range chartWidthAndHeight = ParseRange(operationalWorksheet, chartLocation);
            //double width = double.Parse(chartWidthAndHeight.Cells.ColumnWidth.ToString());
            //double height = double.Parse(chartWidthAndHeight.Cells.RowHeight.ToString());

            // Column A width, plus half again column A
            //Range rngA = ParseRange(operationalWorksheet, "A");
            //double aPlusHalfA = double.Parse(rngA.Cells.ColumnWidth.ToString());

            //// The top of the chart will start below the bottom of the data?
            ////Range rngDataBottom = GetHeaderPlusSubHeaderHeight();

            ChartObjects chartObjs = (ChartObjects)Sheet.ChartObjects(Type.Missing);
            ChartObject chartObj = chartObjs.Add(range.Left, range.Top, range.Width, range.Height);
            Chart xlChart = chartObj.Chart;
            
            xlChart.Location(XlChartLocation.xlLocationAutomatic, Sheet.Name);

            Range sourceData = Sheet.get_Range("A3", Report.GetCellAtOffset("A3", numberSeries, NumberYears));
            xlChart.ChartType = XlChartType.xlXYScatterLines;
            xlChart.SetSourceData(sourceData, XlRowCol.xlRows);
            xlChart.HasTitle = true;
            xlChart.ChartTitle.Text = chartTitle;

            xlChart.HasLegend = true;
            xlChart.Legend.Position = XlLegendPosition.xlLegendPositionBottom;
            xlChart.Legend.Font.Size = 14;
            for (int i = 1; i <= numberSeries; i++)
            {
                Series oSeries = (Series) xlChart.SeriesCollection(i);
            }

            SetXyAxis(xTitle,yTitle,xlChart);          
    
        }

        private void SetXyAxis(string xAxisTitle, string yAxisTitle, Chart axesToTitle)
        {
            Axis xAxis = (Axis)axesToTitle.Axes(Microsoft.Office.Interop.Excel.XlAxisType.xlCategory,
                XlAxisGroup.xlPrimary);
            if (xAxisTitle.Length > 0)
            {
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = xAxisTitle;
            }
            else
            {
                xAxis.HasTitle = false;
            }

            xAxis.MaximumScale = FirstYear + NumberYears;
            xAxis.MinimumScale = FirstYear;
            xAxis.MajorUnit = 1;

            Axis yAxis = (Axis)axesToTitle.Axes(Microsoft.Office.Interop.Excel.XlAxisType.xlValue, Microsoft.Office.Interop.Excel.XlAxisGroup.xlPrimary);
            yAxis.HasTitle = true;
            yAxis.AxisTitle.Orientation = Microsoft.Office.Interop.Excel.XlOrientation.xlHorizontal;
            yAxis.AxisTitle.Text = yAxisTitle;
        }



        public void CreateStackedColumnBarGraph( int numberSeries)
        {
            ChartObjects chartObjs = (ChartObjects)Sheet.ChartObjects(Type.Missing);
            var range = Sheet.get_Range(Report.GetCellAtOffset("B6", numberSeries, 0), Report.GetCellAtOffset("A3", 20 + numberSeries + 1, NumberYears));
            ChartObject chartObj = chartObjs.Add(range.Left, range.Top, range.Width, range.Height);
            Chart xlChart = chartObj.Chart;

            var sourceData = Sheet.get_Range("B4", Report.GetCellAtOffset("B4", numberSeries-1, NumberYears-1));
            xlChart.SetSourceData(sourceData, Type.Missing);
            xlChart.ChartType = XlChartType.xlColumnStacked;


            for(int i = 1; i <= numberSeries; i++)
            {
                var series = (Series)xlChart.SeriesCollection(i);
                series.Name = Sheet.get_Range(Report.GetCellAtOffset("A3",i,0), Report.GetCellAtOffset("A3", i, 0)).Value.ToString();
            }


            // Xaxis data labels
            var oS = (Series)xlChart.SeriesCollection(1);

            oS.XValues = Sheet.get_Range("B3", Report.GetCellAtOffset("B3", 0, NumberYears - 1));
            /*
            // Add title:
            if (strChartTitle != "")
            {
                xlChart.HasTitle = true;
                xlChart.ChartTitle.Text = strChartTitle;
            }

            // Xaxis title
            if (strXTitle != "")
            {
                Axis xAxis = (Axis)xlChart.Axes(XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary);
                xAxis.HasTitle = true;
                xAxis.AxisTitle.Text = strXTitle;
                xAxis.AxisTitle.Font.Size = xFontSize;
            }

            // legend:
            xlChart.HasLegend = false;
            if (listLegend != null)
            {
                if (listLegend.Count > 0)
                {
                    xlChart.HasLegend = true;

                    int seriesNum = 1;
                    foreach (string str in listLegend)
                    {
                        oS = (Series)xlChart.SeriesCollection(seriesNum);
                        oS.Name = str;
                        seriesNum++;
                    }
                }
            }

            if (strYTitle != "")
            {
                Axis yAxis = (Axis)xlChart.Axes(XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary);
                yAxis.HasTitle = true;
                yAxis.AxisTitle.Text = strYTitle;
                yAxis.AxisTitle.Font.Size = yFontSize;
            }
            */
        }


    }
}
