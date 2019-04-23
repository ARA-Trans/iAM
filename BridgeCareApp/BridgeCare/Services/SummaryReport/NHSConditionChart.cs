using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services
{
    public class NHSConditionChart
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public NHSConditionChart(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        /// <summary>
        /// Fill NHS Condition Bridge Cnt and DA tab reports.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="bridgeWorkSummaryWorkSheet"></param>
        /// <param name="nhsPercentSectionYearsRow"></param>
        /// <param name="count"></param>
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int nhsPercentSectionYearsRow, string title, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);            
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 1050, 700, 6, 6);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, nhsPercentSectionYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int nhsPercentSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, nhsPercentSectionYearsRow, count, chart, nhsPercentSectionYearsRow + 3, Properties.Resources.Poor, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, nhsPercentSectionYearsRow, count, chart, nhsPercentSectionYearsRow + 2, Properties.Resources.Fair, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, nhsPercentSectionYearsRow, count, chart, nhsPercentSectionYearsRow + 1, Properties.Resources.Good, Color.Green);
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int nhsPercentSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[nhsPercentSectionYearsRow, 2, nhsPercentSectionYearsRow, count + 2];
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