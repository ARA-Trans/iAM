using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services.SummaryReport.Charts
{
    public class CashNeededByBPN
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public CashNeededByBPN(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        internal void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int simulationYearsCount)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.CashNeededByBPNUnconstrainedBudget;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, simulationYearsCount, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }
        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 1, Properties.Resources.BPN1, Color.Blue);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 2, Properties.Resources.BPN2, Color.IndianRed);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 3, Properties.Resources.BPN3, Color.Gray);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 4, Properties.Resources.BPN4, Color.Yellow);

            CreatMeanSpendingLine(bridgeWorkSummaryWorkSheet, totalCashNeededByBPNYearsRow, count, chart, totalCashNeededByBPNYearsRow + 5, Properties.Resources.MeanAnnualSpending, Color.Red);

        }

        private void CreatMeanSpendingLine(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 3, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalCashNeededByBPNYearsRow, 3, totalCashNeededByBPNYearsRow, count + 2];
            var lineGraph = chart.PlotArea.ChartTypes.Add(eChartType.Line);
            var excelChartSerie = lineGraph.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalCashNeededByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalCashNeededByBPNYearsRow, 2, totalCashNeededByBPNYearsRow, count + 2];
            var excelChartSerie = chart.Series.Add(serie, xSerie);
            excelChartSerie.Header = header;
            excelChartSerie.Fill.Color = color;
        }

        private void SetChartAxes(ExcelChart chart)
        {
            stackedColumnChartCommon.SetChartAxes(chart);
            var yAxis = chart.YAxis;
            yAxis.DisplayUnit = 1000000;
            yAxis.Format = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            yAxis.Title.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
            yAxis.Title.Font.Size = 10;
            yAxis.Title.Text = "Millions ($)";
        }
    }
}
