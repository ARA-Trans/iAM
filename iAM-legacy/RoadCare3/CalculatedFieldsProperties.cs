using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PropertyGridEx;
using DatabaseManager;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.Collections;

namespace RoadCare3
{
	public class CalculatedFieldsProperties
	{
		string[] types = new string[] { "STRING", "NUMBER" };
		string[] trueFalse = new string[] { "True", "False" };
        NetworkDefinition _currNetDef;

		private String m_calculatedFieldName;

		public CalculatedFieldsProperties(NetworkDefinition currNetDef)
		{
            _currNetDef = currNetDef;
		}

		public String CalculatedFieldName
		{
			get { return m_calculatedFieldName; }
			set { m_calculatedFieldName = value; }
		}

		public void CreateCalculatedFieldPropertyGrid(PropertyGridEx.PropertyGridEx pg)
		{
			// Initialize the property grid for attributes.
			bool bReadOnly = true;
			if (pg.Dock == DockStyle.None)
			{
				bReadOnly = false;
			}
			pg.Item.Clear();
			pg.Item.Add("Field Name", "", bReadOnly, "Database Information", "Data Attribute.", true);

			pg.Item.Add("Grouping", "", false, "Database Information", "Used to group attributes of similar types.", true);
			pg.Item.Add("Ascending", true, false, "Database Information", "True if the number gets smaller with deterioration.", true);
			pg.Item[pg.Item.Count - 1].Choices = new CustomChoices(trueFalse, false);
			pg.Item.Add("Format", "", false, "Database Information", "Determines how numeric data appears in the attribute view. (e.g. f1 = 1.0 f2 = 1.00 etc.)", true);
			pg.Item.Add("Minimum", "", false, "Database Information", "Maximum allowed data value", true);
			pg.Item.Add("Maximum", "", false, "Database Information", "Minimum allowed data value", true);
			pg.Item.Add("Default_Value", "", false, "Database Information", "The default data value to use if no data is present.", true);
			pg.Item.Add("One", "", false, "Levels", "Defined level breaks and coloring schemes.", true);
			pg.Item.Add("Two", "", false, "Levels", "Defined level breaks and coloring schemes.", true);
			pg.Item.Add("Three", "", false, "Levels", "Defined level breaks and coloring schemes.", true);
			pg.Item.Add("Four", "", false, "Levels", "Defined level breaks and coloring schemes.", true);
			pg.Item.Add("Five", "", false, "Levels", "Defined level breaks and coloring schemes.", true);

            pg.Item.Add("Network Definition Name", _currNetDef.NetDefName, true, "Database Information", "Defines which network definition this calculated field belongs to.", true);
		}

		public String CreateCalculatedField(PropertyGridEx.PropertyGridEx pgCalcField)
		{
			m_calculatedFieldName = pgCalcField.Item[0].Value.ToString();
			String strInsert = "INSERT INTO ATTRIBUTES_ (ATTRIBUTE_, ";
			String strValues = " VALUES ('" + m_calculatedFieldName + "', '";

			for (int i = 1; i < pgCalcField.Item.Count; i++)
			{
				String strDatabaseAttribteName = (String)Global.m_htFieldMapping[pgCalcField.Item[i].Name];
				if (pgCalcField.Item[i].Value.ToString() != "")
				{
					strInsert += strDatabaseAttribteName + ", ";
					switch( DBMgr.NativeConnectionParameters.Provider )
					{
						case "ORACLE":
							switch( strDatabaseAttribteName.ToUpper() )
							{
								case "ASCENDING":
									if( pgCalcField.Item[i].Value.ToString().ToUpper().Trim() == "TRUE" )
									{
										strValues += "1', '";
									}
									else
									{
										strValues += "0', '";
									}
									break;
								default:
									strValues += pgCalcField.Item[i].Value + "', '";
									break;
							}
							break;
						default:
							strValues += pgCalcField.Item[i].Value + "', '";
							break;
					}
				}
			}
			strInsert += "CALCULATED, NATIVE_, TYPE_, ";
			strValues += "1', '1', 'NUMBER', '";
			strInsert = strInsert.Substring(0, strInsert.Length - 2) + ")";
			strValues = strValues.Substring(0, strValues.Length - 3);
			strInsert += strValues + ")";

			try { DBMgr.ExecuteNonQuery(strInsert); }
			catch (Exception sqlE)
			{
				Global.WriteOutput("Error: Insert new attribute into database failed with, " + sqlE.Message);
			}
			return m_calculatedFieldName;
		}

		private bool IsPropertyGridComplete(PropertyGridEx.PropertyGridEx pgCalcField)
		{
			// Check the property grid for valid entries...
			bool bIsComplete = true;
			if (pgCalcField.Item["Field Name"].Value.ToString() == ""
				|| pgCalcField.Item["Default_Value"].Value.ToString() == ""
				//|| pgCalcField.Item["Maximum"].Value.ToString() == ""
				//|| pgCalcField.Item["Minimum"].Value.ToString() == ""
				|| pgCalcField.Item["One"].Value.ToString() == ""
				|| pgCalcField.Item["Two"].Value.ToString() == ""
				|| pgCalcField.Item["Three"].Value.ToString() == ""
				|| pgCalcField.Item["Four"].Value.ToString() == ""
				|| pgCalcField.Item["Five"].Value.ToString() == "")
			{
				bIsComplete = false;
			}				
			else
			{
				bIsComplete = true;
			}
			return bIsComplete;
		}

		/// <summary>
		/// Updates the database each time a user completes a row in a property grid.
		/// </summary>
		public void UpdateProperty(String strProperty, String strValue)
		{
			String strUpdate;
			if (strValue.Trim() == "" || strValue.Trim().ToUpper() == "NULL")
			{
				strUpdate = "Update ATTRIBUTES_ Set " + Global.m_htFieldMapping[strProperty].ToString() + " = NULL Where ATTRIBUTE_ = '" + m_calculatedFieldName + "' AND NETWORK_DEFINITION_NAME = '" + _currNetDef.NetDefName + "'";
			}
			else
			{
				strUpdate = "Update ATTRIBUTES_ Set " + Global.m_htFieldMapping[strProperty].ToString() + " = '" + strValue + "' Where ATTRIBUTE_ = '" + m_calculatedFieldName + "' AND NETWORK_DEFINITION_NAME = '" + _currNetDef.NetDefName + "'";
			}
			try
			{
				DBMgr.ExecuteNonQuery(strUpdate);
			}
			catch (Exception sqlE)
			{
				Global.WriteOutput("Error: Unable to update ATTRIBUTES_ table with new properties for calculated field. " + sqlE.Message);
			}
		}

		public bool VerifyCalcFieldProperties(out string error, PropertyGridEx.PropertyGridEx pgCalcField)
		{
			error = "";
			Hashtable htProperties = new Hashtable();
			for (int i = 0; i < pgCalcField.Item.Count; i++)
			{
				htProperties.Add(pgCalcField.Item[i].Name, pgCalcField.Item[i].Value);
			}

			bool bHasError = false;
			if (pgCalcField.Item["Field Name"].Value == null || pgCalcField.Item["Field Name"].Value.ToString().Trim() == "")
			{
				bHasError = true;
				error += "Field Name, ";
			}
			
			if (htProperties["One"] == null || htProperties["One"].ToString().Trim() == "")
			{
				error += "One, ";
			}
			if (htProperties["Two"] == null || htProperties["Two"].ToString().Trim() == "")
			{
				error += "Two, ";
			}
			if (htProperties["Three"] == null || htProperties["Three"].ToString().Trim() == "")
			{
				error += "Three, ";
			}
			if (htProperties["Four"] == null || htProperties["Four"].ToString().Trim() == "")
			{
				error += "Four, ";
			}
			if (htProperties["Five"] == null || htProperties["Five"].ToString().Trim() == "")
			{
				error += "Five, ";
			}
			if (htProperties["Default_Value"] == null || htProperties["Default_Value"].ToString().Trim() == "")
			{
				error += "Default_Value, ";
			}
			try
			{
				// Error check for correct formatting.
				float f = 1f;
				String strFormatValue = htProperties["Format"].ToString();
				String str = f.ToString(strFormatValue);
				float.Parse(str);
			}
			catch
			{
				error += "Format, ";
			}
			
			if (error.Length == 0)
			{
				bHasError = true;
			}
			else
			{
				error = error.Substring(0, error.Length - 2);
				bHasError = false;
			}
			return bHasError;
		}
	}
}
