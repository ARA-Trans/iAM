using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class NumericProperty : Property
	{
		public NumericProperty(string desc, double val)
		{
			descriptor = desc;
			numeric = true;
			data = val;
		}

		public double Value
		{
			get
			{
				return (double)data;
			}
		}

		public override bool Equals( object obj )
		{
			bool equal = false;
			if( obj.GetType().Name == "NumericProperty" )
			{
				NumericProperty other = (NumericProperty) obj;
				equal = ( other.Label == this.Label ) && ( other.Value == this.Value );
			}
			else
			{
				equal = base.Equals(obj);
			}
			return equal;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
