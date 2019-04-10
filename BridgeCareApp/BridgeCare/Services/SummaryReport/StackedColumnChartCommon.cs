using OfficeOpenXml.Drawing.Chart;
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
        public void SetChartProperties(ExcelChart chart, string title)
        {
            chart.Title.Text = title;
            chart.SetPosition(8, 0, 8, 0);
            chart.SetSize(800, 600);
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
    }
}