using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reports.DDOT
{
	public static class DDOTGlobals
	{
		static Dictionary<string, ThresholdDescriptor> propertyDescriptors = new Dictionary<string, ThresholdDescriptor>();

		static DDOTGlobals()
		{
			ThresholdDescriptor pciLevels = new ThresholdDescriptor("ERROR");
			pciLevels.AddDescriptor("Poor", 0.0);
			pciLevels.AddDescriptor("Fair", 33.0);
			pciLevels.AddDescriptor("Good", 66.0);

			propertyDescriptors.Add( "IRI", pciLevels );
		}

		public static string Description( string propertyName, double propertyValue )
		{
			return propertyDescriptors[propertyName].GetDescriptor(propertyValue);
		}
		public static double Threshold( string propertyName, string thresholdName )
		{
			return propertyDescriptors[propertyName].GetThreshold(thresholdName);
		}
	}
}
