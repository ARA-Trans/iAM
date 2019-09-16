using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class DefinedInList : IValidator
	{
		List<string> acceptableValues;
		Dictionary<string, string> valueMapping;
		string valueName;
		bool correctErrors;

		public DefinedInList(string label, List<string> acceptable, bool fix)
		{
			correctErrors = fix;
			acceptableValues = acceptable;
			valueName = label;
		}

		public DefinedInList( string label, List<string> acceptable, Dictionary<string,string> valMap, bool fix )
		{
			correctErrors = fix;
			acceptableValues = acceptable;
			valueMapping = valMap;
			valueName = label;
		}

		#region IValidator Members

		public bool CorrectErrors
		{
			get
			{
				return correctErrors;
			}
			set
			{
				correctErrors = value;
			}
		}

		public void Validate(List<Segment> segments)
		{
			foreach (Segment segToCheck in segments)
			{
				StringProperty valueToCheck = (StringProperty)segToCheck[valueName];
				if (valueToCheck != null)
				{
					if (!acceptableValues.Contains(valueToCheck.Value.ToUpper()))
					{
						segToCheck.AddError("Contained invalid value for property (" + valueName + "): " + valueToCheck.Value);
						if( correctErrors )
						{
							if( valueMapping != null )
							{
								if( valueMapping.Keys.Contains(valueToCheck.Value.ToUpper()) )
								{
									segToCheck[valueName] = new StringProperty(valueName, valueMapping[valueToCheck.Value.ToUpper()]);
								}
								else if( valueMapping.Keys.Contains(valueToCheck.Value) )
								{
									segToCheck[valueName] = new StringProperty(valueName, valueMapping[valueToCheck.Value]);
								}
								else
								{
									segToCheck.Exclude = true;
								}
							}
							else
							{
								segToCheck.Exclude = true;
							}
						}
					}
				}
				else
				{
					if (correctErrors)
					{
						segToCheck.Exclude = true;
					}
					segToCheck.AddError("Missing property (" + valueName + ")");
				}
			}
		}

		#endregion
	}
}
