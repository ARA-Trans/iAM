using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing;
using OfficeOpenXml.Style;
using System;

namespace BridgeCare.Services
{
    public class PoorBridgeCount
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public PoorBridgeCount(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesCountSectionYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.PoorBridgeCompareBridgeCount;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 1200, 820, 2, 6);

            stackedColumnChartCommon.SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalPoorBridgesCountSectionYearsRow, count, chart);

            //chart.AdjustPositionAndSize();
            chart.Locked = true;
        }        

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesCountSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorBridgesCountSectionYearsRow, count, chart, totalPoorBridgesCountSectionYearsRow + 1, Properties.Resources.BridgeCare, Color.Blue);           
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesCountSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorBridgesCountSectionYearsRow, 2, totalPoorBridgesCountSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
    }
}