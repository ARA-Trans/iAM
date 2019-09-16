using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.Images
{
	public enum ImageSource
	{
		External,
		Embedded,
		Database
	}

	public enum ImageMIMEType
	{
		Bmp, Jpeg, Gif, Png, XPng
	}

	/// <summary>
	/// Defines the behavior if the image does not fit within the specified size.
	/// </summary>
	public enum ImageSizing
	{
		/// <summary>
		/// Default
		/// </summary>
		AutoSize,
		Fit,
		FitProportional,
		Clip
	}
}
