using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services
{
    public class NHSConditionBridgeCount
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public NHSConditionBridgeCount(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        /// <summary>
        /// Fill NHS Condition Bridge Cnt tab report.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="bridgeWorkSummaryWorkSheet"></param>
        /// <param name="nhsBridgeCountPercentSectionYearsRow"></param>
        /// <param name="count"></param>
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int nhsBridgeCountPercentSectionYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.NHSConditionByBridgeCountLLCC;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 1000, 700, 6, 6);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, nhsBridgeCountPercentSectionYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int nhsBridgeCountPercentSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, nhsBridgeCountPercentSectionYearsRow, count, chart, nhsBridgeCountPercentSectionYearsRow + 3, Properties.Resources.Poor, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, nhsBridgeCountPercentSectionYearsRow, count, chart, nhsBridgeCountPercentSectionYearsRow + 2, Properties.Resources.Fair, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, nhsBridgeCountPercentSectionYearsRow, count, chart, nhsBridgeCountPercentSectionYearsRow + 1, Properties.Resources.Good, Color.Green);
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int nhsBridgeCountPercentSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[nhsBridgeCountPercentSectionYearsRow, 2, nhsBridgeCountPercentSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            stackedColumnChartCommon.SetChartAxes(chart);
            chart.YAxis.Format = "#0%";
        }
    }
}