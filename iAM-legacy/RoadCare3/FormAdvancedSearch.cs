using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;
using DatabaseManager;
using RoadCareDatabaseOperations;
using CalculateEvaluate;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.IO;

namespace RoadCare3
{
    public partial class FormAdvancedSearch : Form
    {
        Hashtable m_hashAttributeYear; //Table containing years for which attribute data is available.
        String m_strNetwork;//Network name.
        String m_strNetworkID;
        private String m_strQuery = "";
        bool m_bSimulation;//True if this is an advanced search for a simulation.
        bool m_bResults = false; //Simulation Results
        String m_strSimulationID;
        public CalculateEvaluate.CalculateEvaluate m_evaluate;
        bool m_bCalculatedFields = false;
        bool m_bAsset = false;
        String m_strAsset = "";

        /// <summary>
        /// Calculated field Advanced search
        /// </summary>
		public FormAdvancedSearch(String strCriteria)
		{
            InitializeComponent();
            m_bCalculatedFields = true;
            m_strQuery = strCriteria;
            Global.Attributes.Clear();
            Global.LoadAttributes();
		}
		
        public FormAdvancedSearch(String strNetwork,Hashtable hashAttributeYear,String strQuery,bool bSimulation)
        {
            InitializeComponent();
            m_hashAttributeYear = hashAttributeYear;
            m_strNetwork = strNetwork;
            m_bSimulation = bSimulation;
            m_strQuery = strQuery;
            m_strSimulationID = "";

            String strSelect = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + m_strNetwork + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception e)
            {
                String strError = "Error in getting NETWORKID for NETWORK_NAME = " + m_strNetwork + ". SQL error = " + e.Message;
                Global.WriteOutput(strError);
            }
			//if (m_bResults)
			//{
			//    strSelect = "SELECT COUNT(*) FROM SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID;
			//}
			//else
			//{
			//    strSelect = "SELECT COUNT(*) FROM SECTION_" + m_strNetworkID;
			//}
			//try
			//{
			//    DataSet ds = DBMgr.ExecuteQuery(strSelect);
			//    int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(),out m_nSection);

			//}
			//catch (Exception e)
			//{
			//    String strError = "Error in getting Section count for Advanced Search, SQL error = " + e.Message;
			//    Global.WriteOutput(strError);
			//}

			//while (m_nSection / m_nMode > 100)
			//{
			//    m_nMode = m_nMode * 2;
			//}

        }

        public FormAdvancedSearch(String strNetwork, String strSimulationID,Hashtable hashAttributeYear, String strQuery)
        {
            InitializeComponent();
            m_hashAttributeYear = hashAttributeYear;
            m_strNetwork = strNetwork;
            m_bSimulation = false;
            m_bResults = true;
            m_strQuery = strQuery;
            m_strSimulationID = strSimulationID;

            String strSelect = "SELECT NETWORKID FROM NETWORKS WHERE NETWORK_NAME='" + m_strNetwork + "'";
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                m_strNetworkID = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception e)
            {
                String strError = "Error in getting NETWORKID for NETWORK_NAME = " + m_strNetwork + ". SQL error = " + e.Message;
                Global.WriteOutput(strError);
            }
			//if (m_bResults)
			//{
			//    strSelect = "SELECT COUNT(*) FROM SIMULATION_" + m_strNetworkID + "_" + m_strSimulationID;
			//}
			//else
			//{
			//    strSelect = "SELECT COUNT(*) FROM SECTION_" + m_strNetworkID;
			//}
			//try
			//{
			//    DataSet ds = DBMgr.ExecuteQuery(strSelect);
			//    int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(), out m_nSection);

			//}
			//catch (Exception e)
			//{
			//    String strError = "Error in getting Section count for Advanced Search, SQL error = " + e.Message;
			//    Global.WriteOutput(strError);
			//}

			//while (m_nSection / m_nMode > 100)
			//{
			//    m_nMode = m_nMode * 2;
			//}

        }
		
        public FormAdvancedSearch(String strNetworkID, String strSimulationID, String strQuery)
        {
            InitializeComponent();
            m_hashAttributeYear = Global.GetAttributeYear(strNetworkID,strSimulationID);
            m_strQuery = strQuery;
            m_strSimulationID = strSimulationID;
            m_strNetworkID = strNetworkID;
            String strSelect;
            int m_nSection = 0;
            
            try
            {
                strSelect = "SELECT COUNT(*) FROM SECTION_" + m_strNetworkID;
                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(), out m_nSection);

            }
            catch (Exception e)
            {
                String strError = "Error in getting Section count for Advanced Search, SQL error = " + e.Message;
                Global.WriteOutput(strError);
            }

			//while (m_nSection / m_nMode > 100)
			//{
			//    m_nMode = m_nMode * 2;
			//}
        }
		
        public FormAdvancedSearch(String strCriteria, String strAssetTable)
        {
            InitializeComponent();
            m_strQuery = strCriteria;
            m_strAsset = strAssetTable;
            m_bAsset = true;


        }

		private void FormAdvancedSearch_Load(object sender, EventArgs e)
        {
            if (!m_bCalculatedFields && !m_bAsset)
            {
                LoadAdvancedSearch();
            }
            else if(m_bCalculatedFields)
            {
                LoadSearchCalculatedFields();
            }
            else if (m_bAsset)
            {
                LoadAssetSearchFieldS();

            }
        }

        private void LoadAssetSearchFieldS()
        {
            this.Text = "Calculated Asset Fields Criteria";
            textBoxSearch.Text = m_strQuery;
            DataSet ds = DBMgr.GetTableColumnsWithTypes(m_strAsset);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strAttribute = row["column_name"].ToString();
                TreeNode tn = treeViewAttribute.Nodes.Add(strAttribute);
            }
        }

        private void LoadSearchCalculatedFields()
        {
            this.Text = "Calculated Fields Criteria";
            textBoxSearch.Text = m_strQuery;
            String strSelect = "SELECT DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE CALCULATED=0 OR CALCULATED IS NULL";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strAttribute = row["ATTRIBUTE_"].ToString();
                TreeNode tn = treeViewAttribute.Nodes.Add(strAttribute);
            }
        }

        private void LoadAdvancedSearch()
        {
            this.Text = "Attribute Advanced Search";
            textBoxSearch.Text = m_strQuery;
            if (!m_bSimulation)
            {
                treeViewAttribute.Nodes.Add("FACILITY");
                treeViewAttribute.Nodes.Add("SECTION");
                treeViewAttribute.Nodes.Add("BEGIN_STATION");
                treeViewAttribute.Nodes.Add("END_STATION");
                treeViewAttribute.Nodes.Add("DIRECTION");
            }
            String strSelect = "SELECT DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_ ORDER BY ATTRIBUTE_";
            DataSet ds = DBMgr.ExecuteQuery(strSelect);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                String strAttribute = row[0].ToString();
                if (m_hashAttributeYear.Contains(strAttribute.ToUpper()))
                {
                    TreeNode tn = treeViewAttribute.Nodes.Add(row[0].ToString());
                    if (!m_bSimulation)
                    {
                        if (m_hashAttributeYear.Contains(row[0].ToString()))
                        {
                            List<String> listYear = (List<String>)m_hashAttributeYear[row[0].ToString()];

                            foreach (String strYear in listYear)
                            {
                                tn.Nodes.Add(strYear);
                            }
                            if (!m_bResults)
                            {
                                tn.Nodes.Add(row[0].ToString());
                            }
                        }
                    }
                }
            }
            if (m_bSimulation)
            {
                this.Text = "Edit Criteria";
            }

        }

        private void checkBoxShowAll_CheckedChanged(object sender, EventArgs e)
        {
            UpdateValueList(checkBoxShowAll.Checked);
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (m_bCalculatedFields)
            {
                CheckCalculatedField();
            }
            else if(!m_bAsset && !m_bCalculatedFields)
            {
                CheckQuery();
            }
            else if (m_bAsset)
            {
                CheckAssetField();
            }
        }

        private bool CheckAssetField()
        {
            String strWhere = textBoxSearch.Text;
            if (strWhere.Trim() == "")
            {
                labelResults.Text = "Blank criteia match all assets.";
                return true;
            }

            String strSelect = "SELECT COUNT(*) FROM " + m_strAsset + " WHERE " + strWhere;

            try
            {
                int nValue = DBMgr.ExecuteScalar(strSelect);
                labelResults.Text = nValue.ToString() + " sections meet criteria.";
                labelResults.Visible = true;
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error:" + exception.Message);

            }
            return true;
        }

        private bool CheckCalculatedField()
        {
            String strWhere = textBoxSearch.Text;
            if (strWhere.Trim() != "")
            {
                String strCriteria = strWhere.Trim().ToUpper();
                List<String> listAttribute = Global.ParseAttribute(strCriteria);
                foreach (String str in listAttribute)
                {
                    String strType = Global.GetAttributeType(str);
                    if (strType == "STRING")
                    {
                        String strOldValue = "[" + str + "]";
                        String strNewValue = "[@" + str + "]";
                        strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                    }
                }

                m_evaluate = new CalculateEvaluate.CalculateEvaluate();
                m_evaluate.BuildTemporaryClass(strCriteria, false);
				CompilerResults m_crCriteria = null;
				try
				{
					m_crCriteria = m_evaluate.CompileAssembly();
				}
				catch( Exception ex )
				{
					Global.WriteOutput( "Error: Compiling CRITERIA statement: " + ex.Message );
				}
                if (m_evaluate.m_listError.Count > 0 || m_crCriteria == null)
                {
                    labelResults.Visible = true;
                    labelResults.Text = "Error: Compiling CRITERIA statement.";
                    return false;
                }
                else
                {
                    labelResults.Visible = true;
                    labelResults.Text = "Calculated Field Criteria statement compiled correctly.";
                }
            }
            return true;
        }

        private bool CheckQuery()
        {
            // Build select String
            String strWhere = textBoxSearch.Text;
            String strFrom = DBOp.BuildFromStatement(m_strNetworkID, m_strSimulationID, true);
            if (strWhere.Trim() != "")
            {
                if (m_bSimulation)
                {
                    String strCriteria = strWhere.Trim();
                    List<String> listAttribute = Global.ParseAttribute(strCriteria);
                    foreach (String str in listAttribute)
                    {
                        String strType = Global.GetAttributeType(str);
                        if (strType == "STRING")
                        {
                            String strOldValue = "[" + str + "]";
                            String strNewValue = "[@" + str + "]";
                            strCriteria = strCriteria.Replace(strOldValue, strNewValue);
                        }
                    }

                    m_evaluate = new CalculateEvaluate.CalculateEvaluate();
                    m_evaluate.BuildTemporaryClass(strCriteria, false);
					try
					{
						CompilerResults m_crCriteria = m_evaluate.CompileAssembly();
                        if (m_evaluate.m_listError.Count > 0 || m_crCriteria == null)
                        {
                            Global.WriteOutput("Error: Compiling CRITERIA statement.");
                            return false;
                        }
					}
					catch( Exception ex )
					{
						Global.WriteOutput( "Error: Compiling CRITERIA statement." + ex.Message );
						return false;		
					}
                }
            }


          
            if (strWhere.Trim() == "")
            {
                return true;
            }

            if (m_bSimulation)
            {
                int nIndex = 0;
                int nBeginIndex = 0;
                int nEndIndex = 0;

                while (nBeginIndex > -1)
                {
                    nBeginIndex = strWhere.IndexOf("[", nIndex);
                    if (nBeginIndex < 0) continue;
                    
                    nEndIndex = strWhere.IndexOf("]", nBeginIndex);

                    if (nEndIndex > -1 && nBeginIndex > -1)
                    {
                        String strAttribute = strWhere.Substring(nBeginIndex+1, nEndIndex - nBeginIndex-1);

                        if (!m_hashAttributeYear.Contains(strAttribute.ToUpper()))
                        {
                            Global.WriteOutput("Attribute " + strAttribute + " not included in Network.");
                            return false;
                        }
                        String str = "[" + strAttribute + "]";
                        strWhere = strWhere.Replace(str, strAttribute);
                        nBeginIndex = 0;
                    }
                }
            }

			
			//oracle chokes on non-space whitespace
			Regex whiteSpaceMechanic = new Regex( @"\s+" );
			strWhere = whiteSpaceMechanic.Replace( strWhere, " " );

            String strSelect = "SELECT COUNT(*)" + strFrom;
            strSelect += " WHERE ";
            strSelect += strWhere;


            DataSet ds;
            try
            {
                ds = DBMgr.ExecuteQuery(strSelect);
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Check query with SQL message " + exception.Message);
                return false;
            }

            int nCount = 0;

            int.TryParse(ds.Tables[0].Rows[0].ItemArray[0].ToString(), out nCount);

            labelResults.Visible = true;
            labelResults.Text = nCount.ToString() + " results match query.";
            return true;


        }

        private void UpdateValueList(bool bShowAll)
        {
            if (!m_bCalculatedFields && !m_bAsset)
            {
                UpdateValues(bShowAll);
            }
            else if(m_bCalculatedFields)
            {
                UpdateCalculatedFieldsValues();
            }
            else if (m_bAsset)
            {
                UpdateAssetFieldValues();
            }
        }

        private void UpdateAssetFieldValues()
        {
            listBoxValues.Items.Clear();
            String attribute = treeViewAttribute.SelectedNode.Text;
            String strSelect = "SELECT DISTINCT " + attribute + " FROM " + m_strAsset;
            ConnectionParameters cp = DBMgr.GetAssetConnectionObject(m_strAsset);
            try
            {
                DataReader dr = new DataReader(strSelect,cp);
                while (dr.Read())
                {
                    listBoxValues.Items.Add(dr["attribute"].ToString());
                }
                dr.Close();
            }
            catch (Exception exception)
            {
                Global.WriteOutput("Error: Connecting to ASSET = " + attribute + "." + exception.Message);
            }
        }

        private void UpdateCalculatedFieldsValues()
        {
            listBoxValues.Items.Clear();
            String attribute = treeViewAttribute.SelectedNode.Text;
            String strSelect = "SELECT DISTINCT DATA_ FROM " + attribute;
            ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(attribute);
            try
            {
                DataReader dr = new DataReader(strSelect, cp);
                while (dr.Read())
                {
                    listBoxValues.Items.Add(dr["DATA_"].ToString());
                }
                dr.Close();
            }
            catch(Exception exception)
            {
                Global.WriteOutput("Error: Connecting to RAW ATTRIBUTE = " + attribute + "." + exception.Message);
            }
        }

        private void UpdateValues(bool bShowAll)
        {

            String strSelect = "";
            String strFrom = DBOp.BuildFromStatement(m_strNetworkID, m_strSimulationID, bShowAll);
            Global.LoadAttributes();

            if(treeViewAttribute.SelectedNode == null) return;

            if (treeViewAttribute.SelectedNode.Level == 0)
            {
                String strAttribute = treeViewAttribute.SelectedNode.Text;
                String sSection = "SECTION_" + m_strNetworkID;
                
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
						if (!bShowAll)
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom + " WHERE (ABS(CAST((BINARY_CHECKSUM(" + sSection + ".SECTIONID, NEWID())) as int))% 100) < 1";
						}
						else
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom;
						}
                        break;
                    case "ORACLE":
						if (!bShowAll)
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom;
						}
						else
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom;
						}
                        break;
                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for UpdateValueList()");
                }


            }
            else // A year has been selected
            {
                String strYear = treeViewAttribute.SelectedNode.Text;
                String strAttribute = treeViewAttribute.SelectedNode.Parent.Text;
                String strAttributeYear;
                String sSection = "SECTION_" + m_strNetworkID;
                
                if (strYear == strAttribute)
                {
                    strAttributeYear = strAttribute;
                }
                else
                {
                    strAttributeYear = strAttribute + "_" + strYear;
                }
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						if (!bShowAll)
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom + " WHERE (ABS(CAST((BINARY_CHECKSUM(" + sSection + ".SECTIONID, NEWID())) as int))% 100) < 1";
						}
						else
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom;
						}
						break;
					case "ORACLE":
						if (!bShowAll)
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom;
						}
						else
						{
							strSelect = "SELECT DISTINCT " + strAttribute + strFrom;
						}
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for UpdateValueList()");
						//break;
				}
            }
            listBoxValues.Items.Clear();//Clear existing.
            try
            {

                DataSet ds = DBMgr.ExecuteQuery(strSelect);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    listBoxValues.Items.Add(row[0].ToString());

                }
            }
            catch (Exception exc)
            {
                listBoxValues.Items.Clear();
                Global.WriteOutput("Error filling value list. " + exc.Message);
				throw exc;

            }
        }

        public void buttonOK_Click(object sender, EventArgs e)
        {
			
            if (!m_bCalculatedFields && !m_bAsset)
            {
                if (!CheckQuery()) return;
            }
            else if(m_bCalculatedFields)
            {
                if (!CheckCalculatedField()) return;
            }
            else if (m_bAsset)
            {
                if (!CheckAssetField()) return;

            }
            m_strQuery = textBoxSearch.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void treeViewAttribute_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateValueList(checkBoxShowAll.Checked);
        }

        private void buttonEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "=";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonNotEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "<>";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonLessEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = ">=";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonLessThan_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = ">";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonGreaterEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "<=";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;

        }

        private void buttonGreaterThan_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "<";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonAnd_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " AND ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonOR_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " OR ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void listBoxValues_DoubleClick(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "'" + listBoxValues.SelectedItem.ToString() + "'";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void treeViewAttribute_MouseDoubleClick(object sender, MouseEventArgs e)
        {
			if( treeViewAttribute.SelectedNode != null )
			{
				if( m_bCalculatedFields )
				{
					String strValue = "[" + treeViewAttribute.SelectedNode.Text + "]";
					int nPosition = textBoxSearch.SelectionStart;
					String strSelect = textBoxSearch.Text.ToString();
					strSelect = strSelect.Insert( nPosition, strValue );
					textBoxSearch.Text = strSelect;
					textBoxSearch.SelectionStart = nPosition + strValue.Length;
				}
				else
				{
					String strValue = "";
					if( treeViewAttribute.SelectedNode == null )
						return;
					if( treeViewAttribute.SelectedNode.Level == 0 )
					{
						if( Global.Attributes.Contains( treeViewAttribute.SelectedNode.Text ) )
						{
							if( !m_bSimulation )
							{
								return;
							}
							else
							{
								strValue = "[" + treeViewAttribute.SelectedNode.Text + "]";
							}
						}
						else
						{
							strValue = treeViewAttribute.SelectedNode.Text;
						}

					}
					else // A year has been selected
					{
						String strYear = treeViewAttribute.SelectedNode.Text;
						String strAttribute = treeViewAttribute.SelectedNode.Parent.Text;
						if( strYear == strAttribute )
						{
							strValue = strAttribute;
						}
						else
						{
							strValue = strAttribute + "_" + strYear;
						}

					}
					int nPosition = textBoxSearch.SelectionStart;
					String strSelect = textBoxSearch.Text.ToString();
					strSelect = strSelect.Insert( nPosition, strValue );
					textBoxSearch.Text = strSelect;
					textBoxSearch.SelectionStart = nPosition + strValue.Length;
				}
			}
        }

        public String GetWhereClause()
        {
            return m_strQuery;
        }


        public String Query
        {
            get{return m_strQuery;}
            set{m_strQuery = value;}
        }
    }
}
