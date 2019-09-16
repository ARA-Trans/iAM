using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RoadCare3
{
	[Serializable]
    public class GISNetworkSettings : RoadCareSettings
    {
        public GISNetworkSettings()
        {

        }

		public void AddAttributeColorList(String attributeName, object colors)
		{
			AddSetting(attributeName, colors);
		}
	}
}
