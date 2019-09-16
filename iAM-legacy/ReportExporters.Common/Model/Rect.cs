using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model
{
	public partial class Rect
	{
		private Size? _left;
		public Size? Left
		{
			get { return _left; }
			set { _left = value; }
		}

		private Size? _top;
		public Size? Top
		{
			get { return _top; }
			set { _top = value; }
		}

		private Size? _right;
		public Size? Right
		{
			get { return _right; }
			set { _right = value; }
		}

		private Size? _bottom;
		public Size? Bottom
		{
			get { return _bottom; }
			set { _bottom = value; }
		}

		public Size All
		{
			set
			{
				Bottom = value;
				Right = value;
				Top = value;
				Left = value;
			}
		}

		internal Rect()
		{
		}

		public Rect(Size _all)
		{
			All = _all;
		}

		public Rect(Size? _left, Size? _top, Size? _right, Size? _bottom)
		{
			Left = _left;
			Top = _top;
			Right = _right;
			Bottom = _bottom;
		}
	}
}
