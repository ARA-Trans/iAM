using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services.SummaryReport.Charts
{
    public class CombinedPostedClosedByBPN
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public CombinedPostedClosedByBPN(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        internal void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int simulationYearsCount)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.CombinedPostedAndClosed;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 1200, 820, 2, 6);

            stackedColumnChartCommon.SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, simulationYearsCount, chart);

            //chart.AdjustPositionAndSize();
            chart.Locked = true;
        }
        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 1, Properties.Resources.Posted, Color.YellowGreen);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalPostedAndClosedByBPNYearsRow, count, chart, totalPostedAndClosedByBPNYearsRow + 2, Properties.Resources.Closed, Color.Red);
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPostedAndClosedByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPostedAndClosedByBPNYearsRow, 2, totalPostedAndClosedByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }
    }
}
