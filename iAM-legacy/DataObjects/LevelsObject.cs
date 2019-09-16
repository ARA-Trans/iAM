using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Data;

namespace DataObjects
{
	public class LevelsObject
	{
		private int m_levelID;
		private int m_propertyLevelID;
		private Color m_color;
		private string m_levelValue;

		public LevelsObject()
		{
 
		}

		public LevelsObject(DataRow levelObject)
		{
			m_levelID = (int)levelObject["LEVEL_ID"];
			m_propertyLevelID = (int)levelObject["PROPERTY_LEVEL_ID"];
			m_color = ColorTranslator.FromHtml(levelObject["COLOR"].ToString());
			m_levelValue = levelObject["LEVEL_VALUE"].ToString();
		}

		public int LevelID
		{
			get { return m_levelID; }
			set { m_levelID = value; }
		}

		public int PropertyLevelID
		{
			get { return m_propertyLevelID; }
			set { m_propertyLevelID = value; }
		}

		public Color Color
		{
			get { return m_color; }
			set { m_color = value; }
		}

		public string LevelValue
		{
			get { return m_levelValue; }
			set { m_levelValue = value; }
		}

	}
}
