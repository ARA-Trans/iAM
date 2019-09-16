using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Chart
{
	/// <summary>
	/// Type of the chart
	/// </summary>
	public enum ChartType
	{
		/// <summary>
		/// Default
		/// </summary>
		Column,
		Bar,
		Line, 
		Pie,
		Scatter,
		Bubble, 
		Area,
		Doughnut,
		Stock
	}
	
	/// <summary>
	/// Subtype of the chart.
	/// To get string representation use ChartSubTypeUtils.ConvertToString() method.
	/// </summary>
	public enum ChartSubType
	{
		/// <summary>
		/// Default for Column
		/// </summary>
		Column_Plain, Column_Stacked, Column_PercentStacked,
		
		/// <summary>
		/// Default for Bar
		/// </summary>
		Bar_Plain, Bar_Stacked, Bar_PercentStacked,
		
		/// <summary>
		/// Default for Line
		/// </summary>
		Line_Plain, Line_Smooth,
		
		/// <summary>
		/// Default for Pie
		/// </summary>
		Pie_Plain, Pie_Exploded,
		
		/// <summary>
		/// Default for Scatter
		/// </summary>
		Scatter_Plain, Scatter_Line, Scatter_SmoothLine,
		
		/// <summary>
		/// Default for Bubble
		/// </summary>
		Bubble_Plain,
		
		/// <summary>
		/// Default for Area
		/// </summary>
		Area_Plain, Area_Stacked, Area_PercentStacked,
		
		/// <summary>
		/// Default for Doughnut
		/// </summary>
		Doughnut_Plain, Doughnut_Exploded,
		
		/// <summary>
		/// Default for Stock
		/// </summary>
		Stock_HighLowClose, Stock_OpenHighLowClose, Stock_Candlestick
	}
	
		
	public class ChartSubTypeUtils
	{
		public static string ConvertToString(ChartSubType chartSubType)
		{
			string chartSubTypeFull = chartSubType.ToString();
			string[] name_elements = chartSubTypeFull.Split( new char[]{ '_' });
			return  name_elements[1];
		}
	}
}
