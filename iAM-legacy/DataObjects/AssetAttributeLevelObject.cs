using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DatabaseManager;
using System.Drawing;

namespace DataObjects
{
	public class AssetAttributeLevelObject
	{
		private int m_assetPropertyID;
		private string m_attribute;
		private string m_asset;
		private string m_assetProperty;
		private string m_userName;
		private string m_assetPropertyType;
		private ConnectionParameters m_cp;
		private List<LevelsObject> m_levelObjects;

		public AssetAttributeLevelObject()
		{
			m_levelObjects = new List<LevelsObject>();
		}


		public AssetAttributeLevelObject(DataRow assetAttributeLevel, ConnectionParameters cp)
		{
			if (assetAttributeLevel["ASSET"] != null)
			{
				m_asset = assetAttributeLevel["ASSET"].ToString();
			}
			if (assetAttributeLevel["ATTRIBUTE_"] != null)
			{
				m_attribute = assetAttributeLevel["ATTRIBUTE_"].ToString();
			}
			if (assetAttributeLevel["ASSET_PROPERTY"] != null)
			{
				m_assetProperty = assetAttributeLevel["ASSET_PROPERTY"].ToString();
			}
			m_userName = assetAttributeLevel["USER_NAME"].ToString();
			m_assetPropertyID = (int)assetAttributeLevel["ASSET_PROPERTY_ID"];
			m_cp = cp;

			m_levelObjects = GetLevelsObject(m_assetPropertyID);
			LoadPropertyType();
		}

		private void LoadPropertyType()
		{
			if (String.IsNullOrEmpty(m_attribute))
			{
				m_assetPropertyType = DBMgr.IsColumnTypeString(m_asset, m_assetProperty, m_cp);
			}
			else
			{
				m_assetPropertyType = DBMgr.IsColumnTypeString(m_attribute, "DATA_", m_cp);
			}
		}

		public int AssetPropertyID
		{
			get { return m_assetPropertyID; }
			set 
			{
				m_assetPropertyID = value;
				m_levelObjects = GetLevelsObject(m_assetPropertyID);
			}
		}

		public string Attribute
		{
			get {return m_attribute; }
			set { m_attribute = value; }
		}

		public string Asset
		{
			get { return m_asset; }
			set { m_asset = value; }
		}

		public string AssetProperty
		{
			get {return m_assetProperty;}
			set { m_assetProperty = value; }
		}

		public string AssetPropertyType
		{
			set { m_assetPropertyType = value; }
			get { return m_assetPropertyType; }
		}

		public string UserName
		{
			get {return m_userName;}
			set { m_userName = value; }
		}

		public List<LevelsObject> Levels
		{
			get { return m_levelObjects; }
		}

		private List<LevelsObject> GetLevelsObject(int assetPropertyID)
		{
			List<LevelsObject> levelObjects = new List<LevelsObject>();
			String strSelect = "SELECT * FROM LEVELS WHERE PROPERTY_LEVEL_ID = " + assetPropertyID + " ORDER BY LEVEL_VALUE";
			DataSet ds = DBMgr.ExecuteQuery(strSelect);
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				levelObjects.Add(new LevelsObject(row));
			}
			return levelObjects;
		}

		public Color GetColor(string levelValue)
		{
			Color levelToFind;
			if (String.IsNullOrEmpty(levelValue))
			{
				levelToFind = Color.Black;
			}
			else
			{
				if (m_assetPropertyType == "String" || m_assetPropertyType == "Boolean")
				{
					levelToFind = m_levelObjects.Find((delegate(LevelsObject lo) { return lo.LevelValue == levelValue; })).Color;
				}
				else
				{
					levelToFind = GetColor(double.Parse(levelValue));
				}
			}
			return levelToFind;
		}

		private Color GetColor(double levelValue)
		{
			// Ascending attribute
			Color returnColor = Color.Empty;
			foreach (LevelsObject lowerValue in m_levelObjects)
			{
				if (levelValue < double.Parse(lowerValue.LevelValue) && returnColor == Color.Empty)
				{
					returnColor = lowerValue.Color;
				}
			}
			if (returnColor == Color.Empty)
			{
				returnColor = Color.Black;
			}
			return returnColor;

		}
		
	}
}
