using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using RoadCareDatabaseOperations;
using DatabaseManager;

namespace RoadCare3
{
	public partial class FormAssetFilter : Form
	{
		private Hashtable m_attributeYears;

		private DataSet m_dsSectionIDs;

		private List<String> m_AssetTypes;
		private List<String> m_AttributeTypes;

		private String m_networkID;
		private String m_selectTop;
		private String m_whereClause;

		public FormAssetFilter(Hashtable attributeYears, String networkID)
		{
			InitializeComponent();
			m_attributeYears = attributeYears;
			m_networkID = networkID;
			m_selectTop = "SELECT DISTINCT SECTION_" + networkID + ".SECTIONID" +
				" FROM ASSET_SECTION_" + networkID +
				" INNER JOIN SECTION_" + networkID +
				" ON ASSET_SECTION_" + networkID + ".SECTIONID = SECTION_" + networkID + ".SECTIONID" +
				" INNER JOIN SEGMENT_" + networkID + "_NS0" +
				" ON ASSET_SECTION_" + networkID + ".SECTIONID = SEGMENT_" + networkID + "_NS0.SECTIONID";
		}

		private void FormAssetFilter_Load(object sender, EventArgs e)
		{
			m_AssetTypes = DBOp.GetRawAssetNames();
			m_AttributeTypes = DBOp.GetAttributeNames();
			List<String> years;
			foreach (String assetType in m_AssetTypes)
			{
				ConnectionParameters cp = DBMgr.GetAssetConnectionObject(assetType);
				TreeNode tnAssetType = tvFilterCriteria.Nodes.Add(assetType);
				tnAssetType.Tag = "ASSET";
				List<String> assetAttributes = DBOp.GetAssetAttributes(assetType, cp);
				foreach (String assetAttribute in assetAttributes)
				{
					tnAssetType.Nodes.Add(assetAttribute);
				}

			}
			foreach (String attributeType in m_AttributeTypes)
			{
				TreeNode tnRoadAttribute = tvFilterCriteria.Nodes.Add(attributeType);
				tnRoadAttribute.Tag = "ATTRIBUTE";
				try
				{
					years = Global.GetAttributeYears(attributeType, m_attributeYears);
					foreach (String year in years)
					{
						tnRoadAttribute.Nodes.Add(year);
					}
					tnRoadAttribute.Nodes.Add(attributeType);
				}
				catch// (Exception exc)
				{
					//Global.WriteOutput("Error: Problem encountered while filling road attribute node.  Check for valid attribute/asset rollup. " + exc.Message);
				}
			}
		}

		private void tvFilterCriteria_DoubleClick(object sender, EventArgs e)
		{
			// Figure out if they clicked on an asset or an attribute
			if (tvFilterCriteria.SelectedNode != null)
			{
				if (tvFilterCriteria.SelectedNode.Level == 1)
				{
					String strValue = tvFilterCriteria.SelectedNode.Parent.Text;
					if (tvFilterCriteria.SelectedNode.Parent.Tag.ToString() == "ATTRIBUTE")
					{
						if (tvFilterCriteria.SelectedNode.Text == tvFilterCriteria.SelectedNode.Parent.Text)
						{
							strValue = "";
						}
						else
						{
							strValue += "_";
						}
					}
					if (tvFilterCriteria.SelectedNode.Parent.Tag.ToString() == "ASSET")
					{
						strValue += ".";
					}
					strValue += tvFilterCriteria.SelectedNode.Text;
					int nPosition = textBoxSearch.SelectionStart;
					String strSelect = textBoxSearch.Text.ToString();
					strSelect = strSelect.Insert(nPosition, strValue);
					textBoxSearch.Text = strSelect;
					textBoxSearch.SelectionStart = nPosition + strValue.Length;
				}
			}
		}

		private String ParseUserSQL(List<String> assetTables)
		{
			String whereClause = textBoxSearch.Text;
			foreach (String assetType in m_AssetTypes)
			{
				int typeIndex = 0;
				for (typeIndex = whereClause.IndexOf(assetType + ".", typeIndex); typeIndex >= 0; typeIndex = whereClause.IndexOf(assetType + ".", typeIndex + 1))
				{
					int assetAttributeLength = whereClause.Substring(typeIndex).IndexOf(' ') - whereClause.Substring(typeIndex).IndexOf('.') - 1;
					String assetAttribute = whereClause.Substring(typeIndex + assetType.Length + 1, assetAttributeLength);
					if (IsSectionAttribute(assetAttribute))
					{
						whereClause = whereClause.Remove(typeIndex, assetType.Length + 1);
						int secondOperandEndIndex = GetSecondOperandIndex(whereClause, typeIndex);
						whereClause = whereClause.Insert(secondOperandEndIndex + 1, ")");
						String insertAdjust = "(ASSET_SECTION_" + m_networkID + ".ASSET_TYPE = '" + assetType + "' AND ASSET_SECTION_" + m_networkID + ".";
						whereClause = whereClause.Insert(typeIndex, insertAdjust);
						typeIndex += insertAdjust.Length;
					}
					else
					{
						if (!assetTables.Contains(assetType))
						{
							assetTables.Add(assetType);
						}
					}

				}
			}
			return whereClause;
		}

		private void buttonCheck_Click(object sender, EventArgs e)
		{
			GetSectionIDs();
		}

		private DataSet GetSectionIDs()
		{
			List<String> assetTables = new List<String>();
			String assetFilterStatement = m_selectTop;
			String whereClause = ParseUserSQL(assetTables);
			DataSet dsSectionIDs = null;
			foreach (String assetTableName in assetTables)
			{
				assetFilterStatement += " " + "FULL OUTER JOIN " + assetTableName +
					" ON " + assetTableName + ".GEO_ID = ASSET_SECTION_" + m_networkID + ".GEO_ID" +
					" AND ASSET_SECTION_" + m_networkID + ".ASSET_TYPE = '" + assetTableName + "'";
			}
			assetFilterStatement += " WHERE " + whereClause;
			try
			{
				dsSectionIDs = DBMgr.ExecuteQuery(assetFilterStatement);
				int iNumSections = dsSectionIDs.Tables[0].Rows.Count;
				labelResults.Text = iNumSections + " results match query";
				labelResults.Visible = true;
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error executing SQL statement: " + assetFilterStatement + ". " + exc.Message);
			}
			return dsSectionIDs;
		}

		private bool IsSectionAttribute(String assetAttribute)
		{
			bool SectionAttribute = false;
			switch (assetAttribute.ToUpper())
			{
				case "FACILITY":
				case "SECTION":
				case "DIRECTION":
					//dsmelser 2008.10.13
					//commented out some not-section-attribute "section attributes"
				//case "LONGITUDE":
				//case "LATITUDE":
				//case "LAST_MODIFIED":
				//case "GEOMETRY":
				case "BEGIN_STATION":
				case "END_STATION":
					SectionAttribute = true;
					break;
				default:
					SectionAttribute = false;
					break;
			}
			return SectionAttribute;
		}

		private int GetSecondOperandIndex(String clause, int startIndex)
		{
			int i;
			try
			{
				int opIndex = GetOperatorIndex(clause, startIndex);
				for (i = opIndex + 1; IsWhiteSpace(clause[i]); ++i) { }	//whitespace eater
				while (i < clause.Length )
				{
					if (i < clause.Length - 4)
					{
						if (clause.Substring(i, 4).ToUpper() == " AND" || clause.Substring(i, 3).ToUpper() == " OR")
						{
							break;
						}
					}
					else if (i < clause.Length - 3)
					{
						if (clause.Substring(i, 3).ToUpper() == " OR")
						{
							break;
						}
					}
					else
					{
						i = clause.Length;
						break;
					}
					++i;
				}
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Parsing error in GetSecondOperandIndex. " + exc.Message);
				return 0;
			}
			return i - 1;
		}

		private int GetOperatorIndex( String clause, int startIndex )
		{
			//List<int> sorter;

			int lt = clause.IndexOf("<",startIndex);
			int lte = clause.IndexOf("<=", startIndex);
			int e = clause.IndexOf("=", startIndex);
			int gre = clause.IndexOf(">=", startIndex);
			int gr = clause.IndexOf(">", startIndex);
			int ne = clause.IndexOf("<>", startIndex);

			if (lt == -1) lt = int.MaxValue - 1;
			if (lte == -1) lte = int.MaxValue - 1;
			if (e == -1) e = int.MaxValue - 1;
			if (gre == -1) gre = int.MaxValue - 1;
			if (gr == -1) gr = int.MaxValue - 1;
			if (ne == -1) ne = int.MaxValue - 1;

			lte += 1;
			gre += 1;
			ne += 1;

			return Math.Min(lt, Math.Min(lte, Math.Min(e, Math.Min(gre, Math.Min(gr, ne)))));
		}

		private bool IsWhiteSpace(char c)
		{
			return c == ' ' || c == '\t';
		}

		private void InsertOperator(String _operator)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, _operator);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + _operator.Length;
		}

		private void buttonEqual_Click(object sender, EventArgs e)
		{
			InsertOperator(" = ");
		}

		private void buttonLessEqual_Click(object sender, EventArgs e)
		{
			InsertOperator(" <= ");
		}

		private void buttonGreaterEqual_Click(object sender, EventArgs e)
		{
			InsertOperator(" >= ");
		}

		private void buttonAnd_Click(object sender, EventArgs e)
		{
			InsertOperator(" AND ");
		}

		private void buttonOR_Click(object sender, EventArgs e)
		{
			InsertOperator(" OR ");
		}

		private void buttonGreaterThan_Click(object sender, EventArgs e)
		{
			InsertOperator(" > ");
		}

		private void buttonLessThan_Click(object sender, EventArgs e)
		{
			InsertOperator(" < ");
		}

		private void buttonNotEqual_Click(object sender, EventArgs e)
		{
			InsertOperator(" <> ");
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			m_dsSectionIDs = GetSectionIDs();
			m_whereClause = textBoxSearch.Text;
			DialogResult = DialogResult.OK;
			this.Close();
		}

		public DataSet GetSectionIDSet()
		{
			return m_dsSectionIDs;
		}

		private void tvFilterCriteria_AfterSelect(object sender, TreeViewEventArgs e)
		{
			String selectedNodeType = NameSelectedNodeType();
			String columnName = BuildExampleSQLColumnName( selectedNodeType );
			String tableName = BuildExampleSQLTableName( selectedNodeType, columnName );
			String query = BuildExampleSQLStatement( selectedNodeType, columnName, tableName );
			UpdateExampleValues( query, columnName );
		}

		private string NameSelectedNodeType()
		{
			String selectedNodeType = "";
			if( tvFilterCriteria.SelectedNode != null )
			{
				if( tvFilterCriteria.SelectedNode.Level == 1 )
				{
					selectedNodeType = tvFilterCriteria.SelectedNode.Parent.Tag.ToString();
				}
			}

			return selectedNodeType;
		}

		private string BuildExampleSQLColumnName( String selectedNodeType )
		{
			String columnName = "";
			if( tvFilterCriteria.SelectedNode != null )
			{
				if( tvFilterCriteria.SelectedNode.Level == 1 )
				{
					if( selectedNodeType == "ATTRIBUTE" )
					{
						columnName = tvFilterCriteria.SelectedNode.Parent.Text;
						if( columnName != tvFilterCriteria.SelectedNode.Text )
						{
							columnName += "_" + tvFilterCriteria.SelectedNode.Text;
						}
					}
					if( selectedNodeType == "ASSET" )
					{
						columnName = tvFilterCriteria.SelectedNode.Text;
					}
				}
			}

			return columnName;
		}

		private string BuildExampleSQLTableName( String selectedNodeType, String columnName  )
		{
			String tableName = "";
			if( tvFilterCriteria.SelectedNode != null )
			{
				if( tvFilterCriteria.SelectedNode.Level == 1 )
				{
					if( selectedNodeType == "ATTRIBUTE" )
					{
						tableName = "SEGMENT_" + m_networkID + "_NS0";
					}
					if( selectedNodeType == "ASSET" )
					{
						tableName = tvFilterCriteria.SelectedNode.Parent.Text;
						if( IsSectionAttribute( columnName ) )
						{
							tableName = "SECTION_" + m_networkID;
						}
					}
				}
			}

			return tableName;
		}

		private string BuildExampleSQLStatement( String selectedNodeType, String columnName, String tableName )
		{
			String query = "";
			if( tvFilterCriteria.SelectedNode != null )
			{
				if( tvFilterCriteria.SelectedNode.Level == 1 )
				{
					if( checkBoxShowAll.Checked )
					{
						query = "SELECT DISTINCT " + columnName + " FROM " + tableName + " WHERE " + columnName + " IS NOT NULL";
					}
					else
					{
						query = "SELECT TOP(25) " + columnName + " FROM " + tableName + " WHERE " + columnName + " IS NOT NULL ORDER BY NEWID()";
					}
				}
			}

			return query;
		}

		private void listBoxValues_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listBoxValues.SelectedItem != null)
			{
				String strValue = "'" + listBoxValues.SelectedItem.ToString() + "'";
				int nPosition = textBoxSearch.SelectionStart;
				String strSelect = textBoxSearch.Text.ToString();
				strSelect = strSelect.Insert(nPosition, strValue);
				textBoxSearch.Text = strSelect;
				textBoxSearch.SelectionStart = nPosition + strValue.Length;
			}
		}

		public String GetWhereClause()
		{
			return m_whereClause;
		}

		private void UpdateExampleValues( String query, String columnName )
		{
			if( query != "" )
			{
				try
				{
					listBoxValues.Items.Clear();
					DataSet dsSampleValues = DBMgr.ExecuteQuery( query );
					foreach( DataRow dr in dsSampleValues.Tables[0].Rows )
					{
						if( !listBoxValues.Items.Contains( dr[columnName].ToString() ) )
						{
							listBoxValues.Items.Add( dr[columnName] );
						}
					}
				}
				catch( Exception exc )
				{
					Global.WriteOutput( "Error retrieving sample values. " + exc.Message );
				}
			}
		}

		private void checkBoxShowAll_CheckedChanged( object sender, EventArgs e )
		{
			String selectedNodeType = NameSelectedNodeType();
			String columnName = BuildExampleSQLColumnName( selectedNodeType );
			String tableName = BuildExampleSQLTableName( selectedNodeType, columnName );
			String query = BuildExampleSQLStatement( selectedNodeType, columnName, tableName );
			UpdateExampleValues( query, columnName );
		}

	}
}
