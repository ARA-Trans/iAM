using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
	public class PercentageOfAverageValidator : IValidator
	{
		Dictionary<string, double> m_PercentOfAverageMapping;

		#region IValidator Members
		public PercentageOfAverageValidator(Dictionary<string, double> decimalDeviation)
		{
			m_PercentOfAverageMapping = decimalDeviation;
		}

		public void Validate(List<RoadCareDataObject> toValidate, string type)
		{
			if (m_PercentOfAverageMapping.ContainsKey(type))
			{
				foreach (RoadCareDataObject objectToValidate in toValidate)
				{
					
				}
			}
		}

		#endregion


	}
}
