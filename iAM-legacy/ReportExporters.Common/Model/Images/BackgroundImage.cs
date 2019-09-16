using System;
using System.Collections.Generic;
using System.Text;
using ReportExporters.Common.Model.Images;

namespace ReportExporters.Common.Model.Images
{
	public class BackgroundImage
	{
		public enum BackgroundRepeatType
		{
			/// <summary>
			/// Default
			/// </summary>
			Repeat,
			NoRepeat,
			RepeatX,
			RepeatY
		}

		private BaseImage _image;
		public BaseImage Image
		{
			get
			{
				return _image;
			}
		}

		public BackgroundImage(BaseImage baseImage)
		{
			this._image = baseImage;
		}

		private BackgroundRepeatType? _backgroundRepeat;
		/// <summary>
		/// Indicates how the background image should 
		/// repeat to fill the available space: vertically (y),
		/// horizontally (x), both or neither
		/// </summary>
		public BackgroundRepeatType? BackgroundRepeat
		{
			get { return _backgroundRepeat; }
			set { _backgroundRepeat = value; }
		}
	}
}
