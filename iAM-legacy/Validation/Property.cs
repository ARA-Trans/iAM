using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validation_
{
	public abstract class Property
	{
		protected string descriptor;
		protected object data;
		protected bool numeric;

		public bool IsNumeric
		{
			get
			{
				return numeric;
			}
		}

		public string Label
		{
			get
			{
				return descriptor;
			}
			set
			{
				descriptor = value;
			}
		}
	}
}
