using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing;
using OfficeOpenXml.Style;

namespace BridgeCare.Services
{
    public class ConditionBridgeCount
    {        
        /// <summary>
        /// Fill Condition Bridge Cnt tab report.
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="bridgeWorkSummaryWorkSheet"></param>
        /// <param name="totalBridgeCountSectionYearsRow"></param>
        /// <param name="count"></param>
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count)
        {

            worksheet.Cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Protection.IsProtected = true;
            worksheet.View.ShowHeaders = false;

            var chart = worksheet.Drawings.AddChart(Properties.Resources.ConditionByBridgeCount, eChartType.ColumnStacked);
            SetChartProperties(chart);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void SetChartProperties(ExcelChart chart)
        {
            chart.Title.Text = Properties.Resources.ConditionByBridgeCount;
            chart.SetPosition(10, 0, 10, 0);
            chart.SetSize(800, 600);
            chart.RoundedCorners = false;
            chart.PlotArea.Border.Fill.Style = eFillStyle.NoFill;
            chart.Legend.Position = eLegendPosition.Bottom;
        }

        private static void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart, totalBridgeCountSectionYearsRow + 3, Properties.Resources.Poor, Color.Red);
            CreateSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart, totalBridgeCountSectionYearsRow + 2, Properties.Resources.Fair, Color.Yellow);
            CreateSeries(bridgeWorkSummaryWorkSheet, totalBridgeCountSectionYearsRow, count, chart, totalBridgeCountSectionYearsRow + 1, Properties.Resources.Good, Color.Green);
        }

        private static void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = chart.Series.Add(bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2], bridgeWorkSummaryWorkSheet.Cells[totalBridgeCountSectionYearsRow, 2, totalBridgeCountSectionYearsRow, count + 2]);
            serie.Header = header;
            serie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            chart.XAxis.MinorTickMark = eAxisTickMark.None;
            chart.XAxis.MajorTickMark = eAxisTickMark.None;
            chart.XAxis.Border.Fill.Style = eFillStyle.NoFill;

            chart.YAxis.MinorTickMark = eAxisTickMark.None;
            chart.YAxis.MajorTickMark = eAxisTickMark.None;
            chart.YAxis.MajorGridlines.Fill.Color = Color.LightGray;
            chart.YAxis.Border.Fill.Style = eFillStyle.NoFill;
        }
    }
}