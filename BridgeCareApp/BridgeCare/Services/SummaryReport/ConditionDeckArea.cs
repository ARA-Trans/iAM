using System;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

namespace BridgeCare.Services
{
    public class ConditionDeckArea
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public ConditionDeckArea(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon ?? throw new ArgumentNullException(nameof(stackedColumnChartCommon));
        }

        /// <summary>
        /// Fill Condition DA tab report
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="bridgeWorkSummaryWorkSheet"></param>
        /// <param name="totalDeckAreaSectionYearsRow"></param>
        /// <param name="count"></param>
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalDeckAreaSectionYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.ConditionByDeckArea;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalDeckAreaSectionYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }       

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalDeckAreaSectionYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalDeckAreaSectionYearsRow, count, chart, totalDeckAreaSectionYearsRow + 3, Properties.Resources.Poor, Color.Red);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalDeckAreaSectionYearsRow, count, chart, totalDeckAreaSectionYearsRow + 2, Properties.Resources.Fair, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalDeckAreaSectionYearsRow, count, chart, totalDeckAreaSectionYearsRow + 1, Properties.Resources.Good, Color.Green);
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalDeckAreaSectionYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalDeckAreaSectionYearsRow, 2, totalDeckAreaSectionYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }  

        private void SetChartAxes(ExcelChart chart)
        {
            stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;
            yAxis.DisplayUnit = 1000000;
            yAxis.Format = "_(* #,##0.00_);_(* (#,##0.00);_(* -??_);_(@_)";
            yAxis.Title.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
            yAxis.Title.Font.Size = 10;
            yAxis.Title.Text = "Millions sqft";
        }
    }    
}