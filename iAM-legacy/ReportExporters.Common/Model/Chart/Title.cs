using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Style;

namespace ReportExporters.Common.Model.Chart
{
	/// <summary>
	/// The Title element defines a title for the chart or for an axis
	/// </summary>
	public class Title
	{
		#region Enums
		
		public enum TitlePosition
		{
			/// <summary>
			/// Default mode
			/// </summary>
			Center, Near, Far
		}

		#endregion

		#region Properties

		private string _caption;
		/// <summary>
		/// Caption of the title
		/// </summary>
		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption = value;
			}
		}

		private BaseStyle _style;
		/// <summary>
		/// Defines text, border and background style properties for the title.
		/// All Textbox properties apply.
		/// </summary>
		public BaseStyle Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		private TitlePosition? _position;
		/// <summary>
		/// The position of the title.
		/// Not used for chart title
		/// </summary>
		public TitlePosition? Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		#endregion
		
		public Title()
		{
			_style = new BaseStyle();
		}
	}
}
