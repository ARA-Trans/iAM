using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;

namespace ReportExporters.Common.ValueTypeConverters.BooleanTypeConverters
{
	public class CustomBooleanConverter : BooleanConverter
	{
		public enum BooleanToStringValues
		{
			YesNo,
			PlusMinus
		}

		protected Dictionary<bool, object> Values;
		protected object valueAsUndefined;
		
		public CustomBooleanConverter()
		{
		}

		public CustomBooleanConverter(object valueAsTrue, object valueAsFalse, object valueAsUndefined)
		{
			Initialize(valueAsTrue, valueAsFalse, valueAsUndefined);
		}

		private void Initialize(object valueAsTrue, object valueAsFalse, object valueAsUndefined)
		{
			Values = new Dictionary<bool, object>();
			Values.Add(true, valueAsTrue);
			Values.Add(false, valueAsFalse);
			this.valueAsUndefined = valueAsUndefined;
		}

		public CustomBooleanConverter(BooleanToStringValues convertTemplate)
		{
			switch (convertTemplate)
			{
				case BooleanToStringValues.YesNo:
					{
						Initialize("Yes", "No", null);
						break;
					}
				case BooleanToStringValues.PlusMinus:
					{
						Initialize("+", "-", null);
						break;
					}
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if ((destinationType == typeof(string)) && (value is bool))
			{
				bool bValue = (bool)value;
				if ((Values != null) && (Values.Count == 2))
				{
					return Values[bValue];
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
