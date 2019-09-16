using System;
using System.Collections.Generic;
using System.Text;

namespace ReportExporters.Common.Model.ControlItems
{
	public class CISubreport : ControlItem
	{
		private string _reportName;
		public string ReportName
		{
			get
			{
				return _reportName;
			}
		}

		public CISubreport(string reportName) : base()
		{
			_reportName = reportName;
		}
	}

}
