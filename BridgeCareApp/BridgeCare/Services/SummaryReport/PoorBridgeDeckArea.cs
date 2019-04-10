using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing;
using OfficeOpenXml.Style;
using System;

namespace BridgeCare.Services
{
    public class PoorBridgeDeckArea
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public PoorBridgeDeckArea(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesDeckAreaSectionYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.PoorBridgeCompareDeckArea;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 1200, 820, 2, 6);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalPoorBridgesDeckAreaSectionYearsRow, count, chart);

            //chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesDeckAreaSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorBridgesDeckAreaSectionYearsRow, count, chart, totalPoorBridgesDeckAreaSectionYearsRow + 1, Properties.Resources.BridgeCare, Color.Blue);
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorBridgesDeckAreaSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorBridgesDeckAreaSectionYearsRow, 2, totalPoorBridgesDeckAreaSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;            
            yAxis.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* -??_);_(@_)";            
        }
    }
}