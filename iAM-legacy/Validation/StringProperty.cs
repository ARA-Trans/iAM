using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public class StringProperty : Property
	{
		public StringProperty(string desc, string val)
		{
			descriptor = desc;
			numeric = false;
			data = val;
		}

		public string Value
		{
			get
			{
				return (string) data;
			}
			set
			{
				data = value;
			}
		}

		public override bool Equals( object obj )
		{
			bool equal = false;
			if( obj != null )
			{
				if( obj.GetType().Name == "StringProperty" )
				{
					StringProperty other = (StringProperty) obj;
					equal = ( other.Label == this.Label ) && ( other.Value == this.Value );
				}
				else
				{
					equal = base.Equals(obj);
				}
			}
			return equal;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
