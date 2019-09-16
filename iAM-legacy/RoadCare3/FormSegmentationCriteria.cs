using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;
using System.Collections;
using System.Data.SqlClient;

namespace RoadCare3
{
    public partial class FormSegmentationCriteria : Form
    {
        public String m_strName;
        public String m_strCriteria;
        private String m_strAttribute;
        Hashtable hashDataSet = new Hashtable(); // key field - object dataset


        public FormSegmentationCriteria(String strName, String strCriteria, String strAttribute)
        {
            InitializeComponent();
            m_strName = strName;
            m_strCriteria = strCriteria;
            m_strAttribute = strAttribute;
        }

        private void FormSegmentationCriteria_Load(object sender, EventArgs e)
        {
            String strQuery;
            DataSet ds; 


            if (m_strName != "")
            {
                textBoxSubsetName.Text = m_strName;
                textBoxSubsetName.ReadOnly = true;
                textBoxSearch.Text = m_strCriteria;
            }

            if (m_strAttribute != "")
            {
                comboBoxAttribute.Items.Add(m_strAttribute);
                comboBoxAttribute.Text = m_strAttribute;
            }
            else
            {
                strQuery = "SELECT DISTINCT ATTRIBUTE_ FROM ATTRIBUTES_ WHERE CALCULATED IS NULL OR CALCULATED = 0";
                ds = DBMgr.ExecuteQuery(strQuery);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    comboBoxAttribute.Items.Add(row.ItemArray[0].ToString());
                }
                if (comboBoxAttribute.Items.Count > 0) comboBoxAttribute.SelectedIndex = 0;
            }


            String strAttribute = comboBoxAttribute.SelectedItem.ToString();

            FillListBoxes(strAttribute);

        }

        private void FillListBoxes(string strAttribute)
        {
            hashDataSet.Clear();
            listBoxField.Items.Clear();
            listBoxValue.Items.Clear();

            listBoxField.Items.Add("ROUTES");
            listBoxField.Items.Add("BEGIN_STATION");
            listBoxField.Items.Add("END_STATION");
            listBoxField.Items.Add("DIRECTION");
            listBoxField.Items.Add("DATE_");
            listBoxField.Items.Add("YEARS");
            listBoxField.Items.Add("[" + strAttribute + "]");

			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(strAttribute);
			String strQuery;
			switch( cp.Provider )
			{
				case "MSSQL":
					strQuery = "SELECT COUNT(*) FROM " + strAttribute + " WHERE ROUTES<> ''";
					break;
				case "ORACLE":
					strQuery = "SELECT COUNT(*) FROM " + strAttribute + " WHERE ROUTES LIKE '_%'";
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
					//break;
			}
			try
			{
				DataSet ds = DBMgr.ExecuteQuery(strQuery, cp);

				int nCount = 0;
				if (ds.Tables[0].Rows.Count > 0)
				{
					nCount = int.Parse(ds.Tables[0].Rows[0].ItemArray[0].ToString());
				}

				int nMode = 1;
				while (nCount / nMode > 100)
				{
					nMode = nMode * 2;
				}
				switch( cp.Provider )
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT DATA_ FROM " + strAttribute + " WHERE ID_%" + nMode.ToString() + "='0' AND ROUTES<>'' ORDER BY DATA_";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT DATA_ FROM " + strAttribute + " WHERE MOD(ID_," + nMode.ToString() + ")='0' AND ROUTES LIKE '_%' ORDER BY DATA_";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for FillListBoxes()" );
						//break;
				}
				ds = DBMgr.ExecuteQuery(strQuery, cp);
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					listBoxValue.Items.Add(row.ItemArray[0].ToString());
				}

				hashDataSet.Add(strAttribute, ds);

				switch( cp.Provider )
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT year(DATE_) AS YEARS FROM " + strAttribute + " WHERE ROUTES<>'' ORDER BY YEARS";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT TO_CHAR(DATE_,'YYYY') AS YEARS FROM " + strAttribute + " WHERE ROUTES LIKE '_%' ORDER BY YEARS";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for FillListBoxes()" );
						//break;
				}
				ds = DBMgr.ExecuteQuery(strQuery, cp);
				hashDataSet.Add("YEARS", ds);
				hashDataSet.Add("DATE_", new DataSet());
				switch (cp.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT ROUTES FROM " + strAttribute + " WHERE ROUTES<>'' ORDER BY ROUTES";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT ROUTES FROM " + strAttribute + " WHERE ROUTES LIKE '_%' ORDER BY ROUTES";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
				ds = DBMgr.ExecuteQuery(strQuery, cp);
				hashDataSet.Add("ROUTES", ds);

				switch( cp.Provider )
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT DIRECTION FROM " + strAttribute + " WHERE ROUTES<>'' ORDER BY DIRECTION";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT DIRECTION FROM " + strAttribute + " WHERE ROUTES LIKE '_%' ORDER BY DIRECTION";
						break;
					default:
						throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
						//break;
				}
				ds = DBMgr.ExecuteQuery(strQuery, cp);
				hashDataSet.Add("DIRECTION", ds);
				hashDataSet.Add("BEGIN_STATION", new DataSet());
				hashDataSet.Add("END_STATION", new DataSet());
			}
			catch (Exception exc)
			{
				Global.WriteOutput("Error: A problem occured while trying to fill the attribute list box. " + exc.Message);
			}
        
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!CheckQuery())
            {
                return;
            }
            String strSubset = textBoxSubsetName.Text.Trim();
            if (strSubset == "")
            {
                MessageBox.Show("A value must be entered for subset name.");
                Global.WriteOutput("Error: Subset name must be entered.");
                return;
            }

            String strCriteria = textBoxSearch.Text.Replace("'", "|");
            if (textBoxSubsetName.ReadOnly)//Edit
            {
                String strUpdate = "UPDATE CRITERIA_SEGMENT SET FAMILY_EXPRESSION ='" + strCriteria + "' WHERE FAMILY_NAME ='" + strSubset + "'";
                try
                {
                    DatabaseManager.DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch(Exception exception)
                {
                    Global.WriteOutput("Error:" + exception.ToString());
                    MessageBox.Show("Error: " + exception.ToString());
                    return;
                }
            }
            else// Insert
            {
                String strInsert = "INSERT INTO CRITERIA_SEGMENT (FAMILY_NAME,FAMILY_EXPRESSION) VALUES ('" + strSubset + "','" + strCriteria + "')";
                try
                {
                    DatabaseManager.DBMgr.ExecuteNonQuery(strInsert);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error:" + exception.ToString());
                    MessageBox.Show("Error: " + exception.ToString());
                    return;
                }

            
            }

            this.m_strCriteria = textBoxSearch.Text;
            this.m_strName = strSubset;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            CheckQuery();
        }

        private void listBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (listBoxField.SelectedItem != null)
			{
				String strKey = listBoxField.SelectedItem.ToString();
				listBoxValue.Items.Clear();

				int nBegin = strKey.IndexOf("[");
				int nEnd = strKey.IndexOf("]");
				if (nEnd >= 0 && nBegin >= 0)
				{
					strKey = strKey.Substring(nBegin + 1, nEnd - nBegin - 1);
				}

				if (hashDataSet.Contains(strKey))
				{
					listBoxValue.Items.Clear();
					DataSet ds = (DataSet)hashDataSet[strKey];
					if (ds.Tables.Count > 0)
					{
						foreach (DataRow row in ds.Tables[0].Rows)
						{
							listBoxValue.Items.Add(row.ItemArray[0]);
						}
					}
				}
			}
 
        }

        private void comboBoxAttribute_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillListBoxes(comboBoxAttribute.Text.ToString());
        }

        
        private bool CheckQuery()
        {
			String strWhere = textBoxSearch.Text.ToString();
			String strAny = strWhere.ToUpper();
			String strTable = "";
			String strQuery;

			bool bGoodQuery = false;
			bool bAny = false;
			if (strAny.Contains("ANYRECORD") || strAny.Contains("ANYCHANGE") || strAny.Contains("ANYYEAR"))
				bAny = true;

			m_strAttribute = "[" + comboBoxAttribute.Text + "]";

			switch (DBMgr.NativeConnectionParameters.Provider)
			{
				case "MSSQL":
					strWhere = strWhere.Replace(m_strAttribute, " DATA_ ");
					strTable = m_strAttribute.Replace("[", " ");
					strTable = strTable.Replace("]", " ");
					break;
				case "ORACLE":
					strWhere = strWhere.Replace(m_strAttribute, " \"DATA_\" ");
					strTable = m_strAttribute.Replace("[", "");
					strTable = strTable.Replace("]", "");
					break;
				default:
					throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
					//break;
			}

			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(strTable);

			if (bAny)
			{
				switch (cp.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE ROUTES<>'' OR ROUTES IS NOT NULL";
						break;
					case "ORACLE":
						//if our goal is to select all non-null, non-empty strings in the routes column
						//we should be using AND and not OR here (and in the other cases too...)
						//the empty string ('') fails "LIKE '_%' (and "<>''") but passes "IS NOT NULL"
						//that means that empty strings pass with an OR

						//I'm leaving it as is to keep consistent functionality for the time being, but when
						//these debugging checks are removed (and the contents moved to functions),
						//this should be examined.
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE ROUTES LIKE '_%' OR ROUTES IS NOT NULL";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
			}
			else
			{

				switch (cp.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE (ROUTES<>'' OR ROUTES IS NOT NULL) AND (" + strWhere + ")";
						break;
					case "ORACLE":
						//if our goal is to select all non-null, non-empty strings in the routes column
						//we should be using AND and not OR here (and in the other cases too...)
						//the empty string ('') fails "LIKE '_%' (and "<>''") but passes "IS NOT NULL"
						//that means that empty strings pass with an OR

						//I'm leaving it as is to keep consistent functionality for the time being, but when
						//these debugging checks are removed (and the contents moved to functions),
						//this should be examined.
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE (ROUTES LIKE '_%' OR ROUTES IS NOT NULL) AND (" + strWhere + ")";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
				
			}
			try
			{
				strTable = strTable.Trim();
				switch (cp.Provider)
				{
					case "MSSQL":
						strWhere = strWhere.Replace("YEARS", " year(DATE_) ");
						break;
					case "ORACLE":
						strWhere = strWhere.Replace("YEARS", " TO_CHAR(\"DATE_\",'YYYY') ");
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for FillListBoxes()");
						//break;
				}
				DataSet ds = DBMgr.ExecuteQuery(strQuery, cp);
				if (ds.Tables.Count == 1)
				{
					String strOut = ds.Tables[0].Rows[0].ItemArray[0].ToString();
					strOut += " entries returned.";
					labelResult.Text = strOut;
					labelResult.Visible = true;
					bGoodQuery = true;
				}
			}
			catch (Exception sqlE)
			{
				string[] str = sqlE.ToString().Split('\n');
				String strError = str[0].Replace("DATA", m_strAttribute);

				MessageBox.Show(strError);

			}
			//catch (Exception ex)
			//{
			//    Global.WriteOutput("Error attempting to check query: " + ex.Message);
			//}
			return bGoodQuery;
        }

        private void buttonAnychange_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "Anychange";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;

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

        private void listBoxField_MouseDoubleClick(object sender, MouseEventArgs e)
        {
			if (listBoxField.SelectedItem != null)
			{
				int nPosition = textBoxSearch.SelectionStart;
				String strField = " " + listBoxField.SelectedItem.ToString() + " ";
				String strSelect = textBoxSearch.Text.ToString();
				strSelect = strSelect.Insert(nPosition, strField);
				textBoxSearch.Text = strSelect;
				textBoxSearch.SelectionStart = nPosition + strField.Length;
			}
        }

        private void listBoxValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
			if (listBoxValue.SelectedItem != null)
			{
				int nPosition = textBoxSearch.SelectionStart;
				String strValue = " '" + listBoxValue.SelectedItem.ToString() + "' ";
				String strSelect = textBoxSearch.Text.ToString();
				strSelect = strSelect.Insert(nPosition, strValue);
				textBoxSearch.Text = strSelect;
				textBoxSearch.SelectionStart = nPosition + strValue.Length;
			}

        }

        private void buttonAnyRecord_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "Anyrecord";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

		private void btnAnyYear_Click(object sender, EventArgs e)
		{
			int nPosition = textBoxSearch.SelectionStart;
			String strValue = "AnyYear";
			String strSelect = textBoxSearch.Text.ToString();
			strSelect = strSelect.Insert(nPosition, strValue);
			textBoxSearch.Text = strSelect;
			textBoxSearch.SelectionStart = nPosition + strValue.Length;
		}

    }
}