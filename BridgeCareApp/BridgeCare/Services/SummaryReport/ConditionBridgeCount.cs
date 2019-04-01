using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using System.Drawing;
using OfficeOpenXml.Drawing;

namespace BridgeCare.Services
{
    public class ConditionBridgeCount
    {
        enum HeaderLabel { Good, Fair, Poor };
        public void Fill(ExcelWorksheet worksheet, ExcelWorksheet bridgeWorkSummaryWorkSheet, int totalBridgeCountSectionYearsRow, int count)
        {
            var chart = worksheet.Drawings.AddChart("Condition By Bridge Count", eChartType.ColumnStacked);
            chart.Title.Text = "Condition By Bridge Count";
            chart.SetPosition(10, 0, 10, 0);
            chart.SetSize(800, 600);            
            chart.RoundedCorners = false;
            chart.PlotArea.Border.Fill.Style = eFillStyle.NoFill;
            chart.Legend.Position = eLegendPosition.Bottom;

            chart.XAxis.MinorTickMark = eAxisTickMark.None;
            chart.XAxis.MajorTickMark = eAxisTickMark.None;            
            chart.XAxis.Border.Fill.Style = eFillStyle.NoFill;

            chart.YAxis.MinorTickMark = eAxisTickMark.None;
            chart.YAxis.MajorTickMark = eAxisTickMark.None;
            chart.YAxis.MajorGridlines.Fill.Color = Color.LightGray;
            chart.YAxis.Border.Fill.Style = eFillStyle.NoFill;

            var poorBridgeCountRow = totalBridgeCountSectionYearsRow + 3;
            var poorSerie = chart.Series.Add(bridgeWorkSummaryWorkSheet.Cells[poorBridgeCountRow, 2, poorBridgeCountRow, count + 2], bridgeWorkSummaryWorkSheet.Cells[totalBridgeCountSectionYearsRow, 2, totalBridgeCountSectionYearsRow, count + 2]);
            poorSerie.Header = HeaderLabel.Poor.ToString();
            poorSerie.Fill.Color = Color.Red;

            var fairBridgeCountRow = totalBridgeCountSectionYearsRow + 2;
            var fairSerie = chart.Series.Add(bridgeWorkSummaryWorkSheet.Cells[fairBridgeCountRow, 2, fairBridgeCountRow, count + 2], bridgeWorkSummaryWorkSheet.Cells[totalBridgeCountSectionYearsRow, 2, totalBridgeCountSectionYearsRow, count + 2]);
            fairSerie.Header = HeaderLabel.Fair.ToString();
            fairSerie.Fill.Color = Color.Yellow;

            var goodBridgeCountRow = totalBridgeCountSectionYearsRow + 1;
            var goodSerie = chart.Series.Add(bridgeWorkSummaryWorkSheet.Cells[goodBridgeCountRow, 2, goodBridgeCountRow, count + 2], bridgeWorkSummaryWorkSheet.Cells[totalBridgeCountSectionYearsRow, 2, totalBridgeCountSectionYearsRow, count + 2]);
            goodSerie.Header = HeaderLabel.Good.ToString();
            goodSerie.Fill.Color = Color.Green;
        }
    }
}