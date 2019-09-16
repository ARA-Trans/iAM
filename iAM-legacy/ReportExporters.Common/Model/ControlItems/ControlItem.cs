using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Style;
using System.Web.UI.WebControls;

namespace ReportExporters.Common.Model.ControlItems
{
	public abstract class ControlItem
	{
		#region Properties

		private Size? _top;
		public Size? Top
		{
			get { return _top; }
			set { _top = value; }
		}

		private Size? _left;
		public Size? Left
		{
			get { return _left; }
			set { _left = value; }
		}

		private Size? _width;
		public Size? Width
		{
			get { return _width; }
			set { _width = value; }
		}

		private Size? _height;
		public Size? Height
		{
			get { return _height; }
			set { _height = value; }
		}

		private BaseStyle _style;
		public BaseStyle Style
		{
			get { return _style; }
			set { _style = value; }
		}

		#endregion

		protected ControlItem()
		{
			//Width = new Unit(1.0, UnitType.Inch);
			//Top = new Unit(0.0, UnitType.Inch);
			//Left = new Unit(0.0, UnitType.Inch);
			//Height = new Unit(0.25, UnitType.Inch);
			//Style = new BaseStyle();
		}

		public void Initialize(ReportStyle reportStyle)
		{
			this.Height = reportStyle.Height;
			this.Width = reportStyle.Width;
			this.Style = reportStyle;
		}
	}

	internal class DataRegion : ControlItem
	{
	}

	internal class Line : ControlItem
	{
	}

	internal class List : DataRegion
	{
	}
}
