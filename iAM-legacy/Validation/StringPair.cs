using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class StringPair
	{
		string independant;
		string dependant;

		public StringPair( string indep, string dep )
		{
			independant = indep;
			dependant = dep;
		}

		public string Independant
		{
			get
			{
				return independant;
			}
		}

		public string Dependant
		{
			get
			{
				return dependant;
			}
		}

		public override bool Equals( object obj )
		{
			bool equal = false;
			if( obj.GetType().Name == "StringPair" )
			{
				StringPair comparePair = (StringPair) obj;
				equal = ( comparePair.Dependant == this.Dependant && comparePair.Independant == this.Independant );
			}
			else
			{
				equal = base.Equals(obj);
			}
			return equal;
		}

		public override int GetHashCode()
		{
			return independant.GetHashCode() ^ dependant.GetHashCode();
		}

	}
}
