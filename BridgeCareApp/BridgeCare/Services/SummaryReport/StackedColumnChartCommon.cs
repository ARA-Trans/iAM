using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.Drawing;

namespace BridgeCare.Services
{
    public class StackedColumnChartCommon
    {
        /// <summary>
        /// Set common chart properties
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        public void SetChartProperties(ExcelChart chart, string title, int width, int height, int positionRow, int positionColumn)
        {
            chart.Title.Text = title;
            chart.SetPosition(positionRow, 0, positionColumn, 0);
            chart.SetSize(width, height);
            chart.RoundedCorners = false;
            chart.PlotArea.Border.Fill.Style = eFillStyle.NoFill;
            chart.Legend.Position = eLegendPosition.Bottom;
        }

        /// <summary>
        /// Set chart axes
        /// </summary>
        /// <param name="chart"></param>
        public void SetChartAxes(ExcelChart chart)
        {
            var xAxis = chart.XAxis;
            xAxis.MinorTickMark = eAxisTickMark.None;
            xAxis.MajorTickMark = eAxisTickMark.None;
            xAxis.Border.Fill.Style = eFillStyle.NoFill;

            var yAxis = chart.YAxis;
            yAxis.MinorTickMark = eAxisTickMark.None;
            yAxis.MajorTickMark = eAxisTickMark.None;
            yAxis.MajorGridlines.Fill.Color = Color.LightGray;
            yAxis.Border.Fill.Style = eFillStyle.NoFill;
        }

        /// <summary>
        /// Set worksheet properties, same in all stacked column chart reports
        /// </summary>
        /// <param name="worksheet"></param>
        public void SetWorksheetProperties(ExcelWorksheet worksheet)
        {
            var excelFill = worksheet.Cells.Style.Fill;
            excelFill.PatternType = ExcelFillStyle.Solid;
            excelFill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Protection.IsProtected = true;
            worksheet.View.ShowHeaders = false;
        }
    }
}
