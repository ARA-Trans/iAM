using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services.SummaryReport.Charts
{
    public class ClosedBridgeCountByBPN
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public ClosedBridgeCountByBPN(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalClosedBridgeCountByBPNYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.ClosedBridgeCountByBPN;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            stackedColumnChartCommon.SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalClosedBridgeCountByBPNYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalClosedBridgeCountByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalClosedBridgeCountByBPNYearsRow, count, chart, totalClosedBridgeCountByBPNYearsRow + 1, Properties.Resources.BPN1, Color.Purple);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalClosedBridgeCountByBPNYearsRow, count, chart, totalClosedBridgeCountByBPNYearsRow + 2, Properties.Resources.BPN2, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalClosedBridgeCountByBPNYearsRow, count, chart, totalClosedBridgeCountByBPNYearsRow + 3, Properties.Resources.BPN3, Color.Gray);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalClosedBridgeCountByBPNYearsRow, count, chart, totalClosedBridgeCountByBPNYearsRow + 4, Properties.Resources.BPN4, Color.Yellow);

        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorDeckAreaByBPNYearsRow, 2, totalPoorDeckAreaByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
    }
}
