using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadCareDatabaseOperations;
using System.Collections;

namespace Reports
{
	public class BinAttributeData
	{
		private string m_networkID;
		private string m_userName;
		private string m_simulationID;
		private string m_year;
		private string m_attribute;
		private string m_criteria;

		public BinAttributeData(string networkID, string attribute, string userName, string year, string criteria, string simulationID)
		{
			m_networkID = networkID;
			m_userName = userName;
			m_criteria = criteria;
			m_simulationID = simulationID;
			m_attribute = attribute;
			m_year = year;
		}

		public Dictionary<string, double> CreateAttributeBins(List<BinAttribute> binAttributes, Method method)
		{
			Dictionary<string, double> runningAttributeTotals = new Dictionary<string,double>();
			List<string> attributeValues;
			double attributeNumValue;
			Hashtable attributeFrequency = DBOp.GetPercentagePerStringAttribute(m_networkID, m_simulationID, m_attribute, m_year, method.ToString(), m_criteria,false, out attributeValues);

			foreach(BinAttribute binAttribute in binAttributes)
			{
				runningAttributeTotals.Add(binAttribute.Name, 0);
			}
			runningAttributeTotals.Add("Other", 0);

			foreach (string attributeValue in attributeValues)
			{
				foreach (BinAttribute binAttribute in binAttributes)
				{
					if (attributeValue != null && attributeValue != "" && attributeValue != "NULL")
					{
						attributeNumValue = double.Parse(attributeValue);
						if (attributeNumValue > binAttribute.Min && attributeNumValue <= binAttribute.Max)
						{
							runningAttributeTotals[binAttribute.Name] += double.Parse(attributeFrequency[attributeValue].ToString());
						}
					}
					else
					{
						runningAttributeTotals["Other"] += double.Parse(attributeFrequency[attributeValue].ToString());
					}
				}
			}
			return runningAttributeTotals;
		}
	}

	public class BinAttribute
	{
		private double m_max;
		private double m_min;

		private string m_name;

		public BinAttribute(double max, double min, string name)
		{
			m_max = max;
			m_min = min;
			m_name = name;
		}

		public double Max
		{
			get
			{
				return m_max;
			}
		}

		public double Min
		{
			get
			{
				return m_min;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
		}
	}
}
