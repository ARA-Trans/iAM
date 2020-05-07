using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing;
using OfficeOpenXml.Style;
using System;

namespace BridgeCare.Services
{
    public class ConditionBridgeCount
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public ConditionBridgeCount(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon ?? throw new ArgumentNullException(nameof(stackedColumnChartCommon));
        }

        /// <summary>
        /// Fill Condition Bridge Cnt tab report.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="bridgeWorkSummaryWorkSheet"></param>
        /// <param name="totalBridgeCountSectionYearsRow"></param>
        /// <param name="count"></param>
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.CombineNHSNonNHSConditionByBridgeCount;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }        

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart, totalBridgeCountSectionYearsRow + 3, Properties.Resources.Poor, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart, totalBridgeCountSectionYearsRow + 2, Properties.Resources.Fair, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart, totalBridgeCountSectionYearsRow + 1, Properties.Resources.Good, Color.FromArgb(0, 176, 80));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalBridgeCountSectionYearsRow, 2, totalBridgeCountSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            stackedColumnChartCommon.SetChartAxes(chart);
            chart.YAxis.Format = "#0%";
            chart.YAxis.MaxValue = 1;
        }
    }
}
