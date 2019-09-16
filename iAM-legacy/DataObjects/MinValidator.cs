using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjects
{
	public class MinValidator : IValidator
	{
		Dictionary<string, double> m_MinMapping;
		public MinValidator(Dictionary<string, double> mapping)
		{
			m_MinMapping = mapping;
		}

		#region IValidator Members
		public void Validate(List<RoadCareDataObject> toValidate, string type)
		{
			if (m_MinMapping.ContainsKey(type))
			{
				foreach (RoadCareDataObject objectToValidate in toValidate)
				{
					if (((INumericObject)objectToValidate).Data < m_MinMapping[type])
					{
						objectToValidate.Errors.Add(type + " had value less than " + m_MinMapping[type]);
					}
				}
			}
		}
		#endregion
	}
}
