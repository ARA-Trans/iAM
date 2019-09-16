using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.AA_COUNTY
{
	public class TreatmentDollarYearReportItem
	{
		private string _year;
		private string _treatmentName;
		private string _dollarsSpent;

		public string Year
		{
			get { return _year; }
		}

		public string TreatmentName
		{
			get { return _treatmentName; }
		}

		public string Cost
		{
			get { return _dollarsSpent; }
		}

		public TreatmentDollarYearReportItem(string year, string treatmentName, string dollarsSpent)
		{
			_year = year;
			_treatmentName = treatmentName;
			_dollarsSpent = dollarsSpent;
		}
	}
}
