using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.DDOT
{
	public class ThresholdDescriptor
	{
		string lowestDescription;
		Dictionary<double, string> thresholdNames;
		Dictionary<string, double> thresholdValues;
		List<double> sortableValues;

		public ThresholdDescriptor(string lowest)
		{
			thresholdNames = new Dictionary<double, string>();
			thresholdValues = new Dictionary<string,double>();
			sortableValues = new List<double>();
			lowestDescription = lowest;
		}

		public void AddDescriptor( string threshName, double threshVal )
		{
			thresholdNames.Add(threshVal, threshName);
			thresholdValues.Add(threshName, threshVal);
			sortableValues.Add(threshVal);
		}

		public string GetDescriptor( double propertyValue )
		{
			string lastGood = lowestDescription;
			//thresholdValues.Sort(delegate( double a, double b )
			//{
			//    return a.CompareTo(b)
			//});		//should sort ascending
			sortableValues.Sort();
			foreach( double threshold in sortableValues )
			{
				if( propertyValue >= threshold )
				{
					lastGood = thresholdNames[threshold];
				}
				else
				{
					break;
				}
			}

			return lastGood;
		}

		public double GetThreshold( string descriptor )
		{
			double threshold = double.MinValue;
			if( thresholdValues.Keys.Contains(descriptor) )
			{
				threshold = thresholdValues[descriptor];
			}
			return threshold;
		}
	}
}
