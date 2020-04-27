using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services.SummaryReport.Charts
{
    public class NonNHSConditionBridgeCount
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public NonNHSConditionBridgeCount(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon ?? throw new ArgumentNullException(nameof(stackedColumnChartCommon));
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int nonNHSBridgeCountPercentRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.ConditionByBridgeCount;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, nonNHSBridgeCountPercentRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int nonNHSBridgeCountPercentRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, nonNHSBridgeCountPercentRow, count, chart, nonNHSBridgeCountPercentRow + 3, Properties.Resources.Poor, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, nonNHSBridgeCountPercentRow, count, chart, nonNHSBridgeCountPercentRow + 2, Properties.Resources.Fair, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, nonNHSBridgeCountPercentRow, count, chart, nonNHSBridgeCountPercentRow + 1, Properties.Resources.Good, Color.FromArgb(0, 176, 80));
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int nonNHSBridgeCountPercentRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[nonNHSBridgeCountPercentRow, 2, nonNHSBridgeCountPercentRow, count + 2];
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
