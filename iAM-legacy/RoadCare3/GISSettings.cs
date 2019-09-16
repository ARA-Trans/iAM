using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

namespace RoadCare3
{
	public class GISSettings : RoadCareSettings
	{
		public GISSettings()
		{
 
		}

		public void AddGISNetworkSettings(String network, GISNetworkSettings gisNetworkSettings)
		{
			AddSetting(network, gisNetworkSettings);
		}
	}
}
