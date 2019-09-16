using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class PropertyNameMapping : IValidator
	{
		List<string> acceptableNames;
		Dictionary<string, string> nameMapping;

		bool correctErrors;

		public PropertyNameMapping( List<string> canUse )
		{
			acceptableNames = canUse;
			correctErrors = false;
		}

		public PropertyNameMapping( List<string> canUse, Dictionary<string, string> mapping, bool correct )
		{
			acceptableNames = canUse;
			nameMapping = mapping;
			correctErrors = correct;
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

		public void Validate( List<Segment> segmentsToValidate )
		{
			foreach( Segment segToValidate in segmentsToValidate )
			{
				foreach( Property segProperty in segToValidate.Properties )
				{
					if( !acceptableNames.Contains(segProperty.Label) )
					{
						if( correctErrors )
						{
							if( nameMapping != null && nameMapping.Keys.Contains(segProperty.Label) )
							{
								segProperty.Label = nameMapping[segProperty.Label];
							}
							else
							{
								segToValidate.Exclude = true;
								segToValidate.AddError("Found invalid property name (" + segProperty.Label + ") with no mapping.");
							}
						}
						else
						{
							segToValidate.AddError("Found invalid property name (" + segProperty.Label + ") with no mapping.");
						}
					}
				}
			}
		}

		#endregion
	}
}
