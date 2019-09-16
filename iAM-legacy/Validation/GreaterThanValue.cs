using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class GreaterThanValue : IValidator
	{
		string valueName;
		double threshold;
		bool correctErrors;

		public GreaterThanValue(string label, double minimum, bool fix)
		{
			valueName = label;
			threshold = minimum;
			correctErrors = fix;
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
				NumericProperty valToCheck = (NumericProperty)segToCheck[valueName];
				if (valToCheck != null)
				{
					if (valToCheck.Value <= threshold)
					{
						if (correctErrors)
						{
							segToCheck.Exclude = true;
						}
						segToCheck.AddError(valueName + "(" + valToCheck.Value + ") was not greater than " + threshold.ToString() + ".");
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
