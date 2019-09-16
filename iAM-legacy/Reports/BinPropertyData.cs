using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports
{
	public class BinPropertyData
	{
		public string m_propertyBinTitle;
		public Dictionary<string, string> m_conditionToValue = new Dictionary<string,string>();
		public string m_propertyBin;
		public double m_binTotal;

		public BinPropertyData()
		{

		}

		public void SetBinPropertyData(string propertyBinTitle, string condition, string value, string propertyBin)
		{
			m_propertyBin = propertyBin;
			m_conditionToValue.Add(condition, value);
			m_propertyBinTitle = propertyBinTitle;
		}
	}
}
