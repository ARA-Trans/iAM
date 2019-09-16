using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;

namespace ReportExporters.Common.Model.Style
{
	public class Border
	{
		public Border()
		{
		}
		
		public Border(Border border)
		{
			this.Color = border.Color;
			this.Style = border.Style;
			this.Width = border.Width;
		}

		public Border(Color color, BorderStyle style, Unit width)
		{
			this.Color = color;
			this.Style = style;
			this.Width = width;
		}

		private Size? _width;
		public Size? Width
		{
			get { return _width; }
			set { _width = value; }
		}

		private Color? _color;
		public Color? Color
		{
			get { return _color; }
			set { _color = value; }
		}

		private BorderStyle? _style;
		public BorderStyle? Style
		{
			get { return _style; }
			set { _style = value; }
		}
	}
}
