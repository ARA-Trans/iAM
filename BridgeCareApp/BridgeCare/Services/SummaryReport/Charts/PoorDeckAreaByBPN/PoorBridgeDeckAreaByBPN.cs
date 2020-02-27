using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;

namespace BridgeCare.Services.SummaryReport.PoorDeckAreaByBPN
{
    public class PoorBridgeDeckAreaByBPN
    {
        private readonly StackedColumnChartCommon stackedColumnChartCommon;

        public PoorBridgeDeckAreaByBPN(StackedColumnChartCommon stackedColumnChartCommon)
        {
            this.stackedColumnChartCommon = stackedColumnChartCommon;
        }

        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNYearsRow, int count)
        {
            stackedColumnChartCommon.SetWorksheetProperties(worksheet);
            var title = Properties.Resources.PoorDeckAreaByBPN;
            var chart = worksheet.Drawings.AddChart(title, eChartType.ColumnStacked);
            stackedColumnChartCommon.SetChartProperties(chart, title, 950, 700, 6, 7);

            SetChartAxes(chart);
            AddSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart);

            chart.AdjustPositionAndSize();
            chart.Locked = true;
        }

        private void AddSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNYearsRow, int count, ExcelChart chart)
        {
            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 1, Properties.Resources.BPN1, Color.Green);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 2, Properties.Resources.BPN2, Color.Yellow);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 3, Properties.Resources.BPN3, Color.Purple);

            CreateSeries(bridgeWorkSummaryWorkSheet, totalPoorDeckAreaByBPNYearsRow, count, chart, totalPoorDeckAreaByBPNYearsRow + 4, Properties.Resources.BPN4, Color.Violet);

        }

        private void CreateSeries(ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalPoorDeckAreaByBPNYearsRow, int count, ExcelChart chart, int fromRow, string header, Color color)
        {
            var serie = bridgeWorkSummaryWorkSheet.Cells[fromRow, 2, fromRow, count + 2];
            var xSerie = bridgeWorkSummaryWorkSheet.Cells[totalPoorDeckAreaByBPNYearsRow, 2, totalPoorDeckAreaByBPNYearsRow, count + 2];
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
