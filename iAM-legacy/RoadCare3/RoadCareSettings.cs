using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Schema;

namespace RoadCare3
{
	[XmlInclude(typeof(GISNetworkSettings))]
	[XmlInclude(typeof(LevelColors))]
	[XmlInclude(typeof(RoadCareSettings))]
	//[XmlInclude(typeof(RCSetting))]
	[XmlInclude(typeof(List<LevelColors>))]
    public abstract class RoadCareSettings
    {
		protected List<RCSetting> storedRCSettings;

        public RoadCareSettings()
        {
            storedRCSettings = new List<RCSetting>();
        }

		public List<RCSetting> StoredSettings
		{
			get { return storedRCSettings; }
			set { storedRCSettings = value; }
		}
		
        public void AddSetting(String settingName, object o)
        {
            storedRCSettings.Add(new RCSetting(settingName, o));
        }

        public object GetSetting(String settingName)
        {
            return storedRCSettings.Find(delegate(RCSetting rc) { return rc.Name == settingName; }).Data;
        }

		//public void SerializeObject(StreamWriter sw)
		//{
		//    foreach (RCSetting setting in storedRCSettings)
		//    {
		//        setting.SerializeObject(sw);
		//    }
		//}

		[Serializable]
		public class RCSetting
		{
			String m_Name = "";
			object m_Data = null;

			public RCSetting()
			{

			}

			public RCSetting(String strName, object objData)
			{
				m_Name = strName;
				m_Data = objData;
			}

			public String Name
			{
				get { return m_Name; }
				set { m_Name = value; }
			}

			public object Data
			{
				get { return m_Data; }
				set { m_Data = value; }
			}
		}
	}
}
