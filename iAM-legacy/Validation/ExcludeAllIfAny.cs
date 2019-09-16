using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class ExcludeAllIfAny : IValidator
	{
		bool correctErrors;
		public ExcludeAllIfAny( bool fixErrors )
		{
			correctErrors = fixErrors;
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

		public void Validate( List<Segment> segments )
		{
			for( int i = 0; i < segments.Count; ++i )
			{
				Segment segToValidate = segments[i];
				if( segToValidate.Exclude )
				{
					int j;
					for( j = i + 1; j < segments.Count; ++j )
					{
						Segment followUpSeg = segments[j];
						if( segToValidate.Equals(followUpSeg) )
						{
							if( correctErrors )
							{
								followUpSeg.Exclude = true;
							}
							followUpSeg.AddError("Another segment in the relevant area was excluded.");
						}
						else
						{
							break;
						}
					}
					i = j;
				}
			}
		}

		#endregion
	}
}
