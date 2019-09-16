using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model
{
	public class Action
	{
		//Drillthrough
		//BookmarkLink

		private string _hyperlink;
		public string Hyperlink
		{
			get
			{
				return _hyperlink;
			}
			set
			{
				_hyperlink = value;
			}
		}

		public Action(Uri hyperlink)
		{
			this._hyperlink = hyperlink.ToString();
		}

		public Action(string hyperlink)
		{
			this._hyperlink = hyperlink;
		}
	}
}
