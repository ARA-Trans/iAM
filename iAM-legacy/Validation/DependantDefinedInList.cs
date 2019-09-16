using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class DependantDefinedInList : IValidator
	{
		bool correctErrors;
		string indPropertyName;
		string depPropertyName;
		List<StringPair> acceptable;
		Dictionary<StringPair, string> correction;

		public DependantDefinedInList( string independantProperty, string dependantProperty, List<StringPair> allowableDependantValues, Dictionary<StringPair, string> correctionMapping, bool fix )
		{
			indPropertyName = independantProperty;
			depPropertyName = dependantProperty;

			acceptable = allowableDependantValues;
			correction = correctionMapping;

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

		public void Validate( List<Segment> segments )
		{
			foreach( Segment segToValidate in segments )
			{
				string error = "";
				string indValue = ( (StringProperty) segToValidate[indPropertyName] ).Value;
				string depValue = ( (StringProperty) segToValidate[depPropertyName] ).Value;
				if( !(acceptable.Contains( new StringPair(indValue,depValue))))
				{
					error = "Invalid value pair: (" + indPropertyName + "," + depPropertyName + ") = (" + indValue + "," + depValue + ").";
					if( correctErrors )
					{
						StringPair stringPairKey = null;
						foreach( StringPair potentialKey in correction.Keys )
						{
							if( potentialKey.Equals(new StringPair(indValue, depValue)) )
							{
								stringPairKey = potentialKey;
								break;
							}
						}
						if( stringPairKey != null )
						{
							( (StringProperty) segToValidate[depPropertyName] ).Value = correction[stringPairKey];
							error += "  Replaced dependent value with " + correction[stringPairKey] + ".";
						}
						else
						{
							error += "  Could not find replacement value.";
							segToValidate.Exclude = true;
						}
					}
					segToValidate.AddError(error);
				}
			}
		}
		#endregion
	}
}
