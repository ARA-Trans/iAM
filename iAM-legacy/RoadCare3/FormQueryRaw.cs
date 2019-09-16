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
namespace RoadCare3
{
    public partial class FormQueryRaw : Form
    {
        public String m_strAttribute = "";
        public bool m_bLinear;
        Hashtable hashDataSet = new Hashtable(); // key field - object dataset
        public String m_strQuery;

        private String m_strAsset = "";

        /// <summary>
        /// Constructor for raw data searches.
        /// </summary>
        /// <param name="strAttribute">The attribute being searched on.</param>
        /// <param name="bLinear">Is the search being performed on linear or section reference data.</param>
        /// <param name="strQuery">Query taken from user input.</param>
        public FormQueryRaw(String strAttribute, bool bLinear,String strQuery)
        {
            InitializeComponent();
            //m_strAttribute = "[" + strAttribute + "]";
			m_strAttribute = strAttribute;
            m_bLinear = bLinear;
            m_strQuery = strQuery;
        }

        public FormQueryRaw(String strAsset, String strQuery)
        {
            InitializeComponent();
            m_strAsset = strAsset;
            m_strQuery = strQuery;
            textBoxSearch.Text = m_strQuery;
        }

        private void FormQueryRaw_Load(object sender, EventArgs e)
        {
            textBoxSearch.Text = m_strQuery;
            if (m_strAttribute != "")
            {
                LoadAttribute();
            }
            if (m_strAsset != "")
            {
                this.Text = "Asset Query";
                LoadAsset();
            }
        }

        private void LoadAsset()
        {
            String strQuery = "SELECT column_name FROM information_schema.columns WHERE table_name = '" + m_strAsset + "'";
            DataSet ds = null;
            try
            {
                ds = DBMgr.ExecuteQuery(strQuery);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    listBoxField.Items.Add(dr[0].ToString());
                }
            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: " + exc.Message);
            }

            

        }

        private void LoadAttribute()
        {
			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(m_strAttribute);
            if (m_bLinear)
            {
                listBoxField.Items.Add("ROUTES");
                listBoxField.Items.Add("BEGIN_STATION");
                listBoxField.Items.Add("END_STATION");
                listBoxField.Items.Add("DIRECTION");
                listBoxField.Items.Add("YEARS");
                listBoxField.Items.Add("DATE_");
                listBoxField.Items.Add(m_strAttribute);
            }
            else
            {
                listBoxField.Items.Add("FACILITY");
                listBoxField.Items.Add("SECTION");
                listBoxField.Items.Add("SAMPLE_");
                listBoxField.Items.Add("YEARS");
                listBoxField.Items.Add("DATE_");
                listBoxField.Items.Add(m_strAttribute);
            }
            listBoxField.Text = m_strAttribute;


            this.Text = "Query " + m_strAttribute;
            String strQuery;
            DataSet ds;
            if (m_bLinear)
            {
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT DATA_ FROM " + m_strAttribute + " WHERE (ABS(CAST((BINARY_CHECKSUM(DATA_, NEWID())) as int))% 100) < 1 ORDER BY DATA_";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT DATA_ FROM " + m_strAttribute + " SAMPLE(10) ORDER BY DATA_";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for LoadAttribute()");
						//break;
				}
            }
            else
            {
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT DATA_ FROM " + m_strAttribute + " WHERE (ABS(CAST((BINARY_CHECKSUM(DATA_, NEWID())) as int))% 100) < 1 ORDER BY DATA_";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT DATA_ FROM " + m_strAttribute + " SAMPLE(10) ORDER BY DATA_";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for LoadAttribute()");
						//break;
				}
            }

            ds = DBMgr.ExecuteQuery(strQuery, cp);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                listBoxValue.Items.Add(row.ItemArray[0].ToString());
            }


            hashDataSet.Add(m_strAttribute, ds);


            if (m_bLinear)
            {
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT year(DATE_) AS YEARS FROM " + m_strAttribute + " WHERE ROUTES<>'' ORDER BY YEARS";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT TO_CHAR(\"DATE_\",'YYYY') AS YEARS FROM " + m_strAttribute.Replace( "[", "" ).Replace( "]", "" ) + " WHERE ROUTES LIKE '_%' ORDER BY YEARS";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for LoadAttribute()");
						//break;
				}
            }
            else
            {
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT year(DATE_) AS YEARS FROM " + m_strAttribute + " WHERE FACILITY<>'' ORDER BY YEARS";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT TO_CHAR(\"DATE_\",'YYYY') AS YEARS FROM " + m_strAttribute.Replace( "[", "" ).Replace( "]", "" ) + " WHERE FACILITY LIKE '_%' ORDER BY YEARS";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for LoadAttribute()");
						//break;
				}
            }
            ds = DBMgr.ExecuteQuery(strQuery, cp);
            hashDataSet.Add("YEARS", ds);
            hashDataSet.Add("DATE_", new DataSet());


            if (m_bLinear)
            {
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT ROUTES FROM " + m_strAttribute + " WHERE ROUTES<>'' ORDER BY ROUTES";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT ROUTES FROM " + m_strAttribute.Replace( "[", "" ).Replace( "]", "" ) + " WHERE ROUTES LIKE '_%' ORDER BY ROUTES";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
                ds = DBMgr.ExecuteQuery(strQuery, cp);
                hashDataSet.Add("ROUTES", ds);
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT DIRECTION FROM " + m_strAttribute + " WHERE ROUTES<>'' ORDER BY DIRECTION";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT DIRECTION FROM " + m_strAttribute.Replace( "[", "" ).Replace( "]", "" ) + " WHERE ROUTES LIKE '_%' ORDER BY DIRECTION";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
                ds = DBMgr.ExecuteQuery(strQuery, cp);
                hashDataSet.Add("DIRECTION", ds);
                hashDataSet.Add("BEGIN_STATION", new DataSet());
                hashDataSet.Add("END_STATION", new DataSet());


            }
            else
            {
				switch (DBMgr.NativeConnectionParameters.Provider)
				{
					case "MSSQL":
						strQuery = "SELECT DISTINCT FACILITY FROM " + m_strAttribute + " WHERE FACILITY<>'' ORDER BY FACILITY";
						break;
					case "ORACLE":
						strQuery = "SELECT DISTINCT FACILITY FROM " + m_strAttribute.Replace( "[", "" ).Replace( "]", "" ) + " WHERE FACILITY LIKE '_%' ORDER BY FACILITY";
						break;
					default:
						throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
						//break;
				}
                ds = DBMgr.ExecuteQuery(strQuery, cp);
                hashDataSet.Add("FACILITY", ds);
                hashDataSet.Add("SECTION", new DataSet());
                hashDataSet.Add("SAMPLE_", new DataSet());
            }
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (m_strAttribute != "")
            {
                CheckAttributeQuery();
            }
            if (m_strAsset != "")
            {
                CheckAssetQuery();
            }
        }

        private bool CheckAssetQuery()
        {
            bool bGoodQuery = false;
            String strWhere = textBoxSearch.Text;
            String strQuery = "SELECT COUNT(*) FROM " + m_strAsset + " WHERE " + strWhere;
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery);
                if (ds.Tables.Count == 1)
                {
                    String strOut = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    strOut += " entries returned.";
                    labelReturn.Text = strOut;
                    labelReturn.Visible = true;
                    bGoodQuery = true;
                }

            }
            catch (Exception exc)
            {
                Global.WriteOutput("Error: Could not get attribute count from data table. " + exc.Message);
            }
            return bGoodQuery;
        }

        private bool CheckAttributeQuery()
        {
			ConnectionParameters cp = DBMgr.GetAttributeConnectionObject(m_strAttribute);
            bool bGoodQuery = false;
            String strWhere = textBoxSearch.Text.ToString();
			strWhere = strWhere.Replace("[" + m_strAttribute + "]", " DATA_ " );//.Replace( "DATE", " to_char( DATE_, 'MM/DD/YYYY' ) " );

            String strQuery;
			String strTable = m_strAttribute;

			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					if( m_bLinear )
					{
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE ROUTES<>'' AND (" + strWhere + ")";
					}
					else
					{
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE FACILITY<>'' AND (" + strWhere + ")";
					}
					break;
				case "ORACLE":
					if( m_bLinear )
					{
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE ROUTES LIKE '_%' AND (" + strWhere + ")";
					}
					else
					{
						strQuery = "SELECT COUNT(*) FROM " + strTable + " WHERE FACILITY LIKE '_%' AND (" + strWhere + ")";
					}
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for XXXXXXXXXXXX" );
				//break;
			}
            try
            {
                DataSet ds = DBMgr.ExecuteQuery(strQuery, cp);
                if (ds.Tables.Count == 1)
                {
                    String strOut = ds.Tables[0].Rows[0].ItemArray[0].ToString();
                    strOut += " entries returned.";
                    labelReturn.Text = strOut;
                    labelReturn.Visible = true;
                    bGoodQuery = true;
                }

            }
            catch (Exception sqlE)
            {
                string[] str = sqlE.ToString().Split('\n');
                String strError = str[0].Replace("DATA_", "[" + m_strAttribute + "]");
                Global.WriteOutput("Error: Could not get attribute count from data table. " + sqlE.Message);
            }
            return bGoodQuery;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (m_strAttribute != "")
            {
                if (CheckAttributeQuery())
                {
                    this.m_strQuery = textBoxSearch.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            if (m_strAsset != "")
            {
                if (CheckAssetQuery())
                {
                    this.m_strQuery = textBoxSearch.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void listBoxField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_strAttribute != "")
            {
                listBoxValue.Items.Clear();
                DataSet ds = (DataSet)hashDataSet[listBoxField.SelectedItem.ToString()];
                if (ds == null) return;
                if (ds.Tables.Count == 1)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        listBoxValue.Items.Add(row.ItemArray[0].ToString());
                    }
                }
            }
            if (m_strAsset != "")
            {
                listBoxValue.Items.Clear();
				String strQuery = "";
				switch( DBMgr.NativeConnectionParameters.Provider )
				{
					case "MSSQL":
						strQuery = "SELECT TOP 25 " + listBoxField.SelectedItem.ToString() + " FROM " + m_strAsset;
						break;
					case "ORACLE":
						strQuery = "SELECT " + listBoxField.SelectedItem.ToString() + " FROM " + m_strAsset + " WHERE ROWNUM < 26";
						break;
					default:
						throw new NotImplementedException( "TODO: Implement ANSI version of listBoxField_SelectedIndexChanged()" );
						//break;
				}
                try
                {
                    DataSet ds = DBMgr.ExecuteQuery(strQuery);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        listBoxValue.Items.Add(dr[0].ToString());
                    }
                }
                catch (Exception exc)
                {
                    Global.WriteOutput("Error: " + exc.Message);
                }
            }
        }

        private void listBoxField_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strField = "[" + listBoxField.SelectedItem.ToString() + "] ";
			switch( DBMgr.NativeConnectionParameters.Provider )
			{
				case "MSSQL":
					switch( listBoxField.SelectedItem.ToString() )
					{
						case "YEARS":
							strField = "year(DATE_)";
							break;
					}
					break;
				case "ORACLE":
					switch( listBoxField.SelectedItem.ToString() )
					{
						case "YEARS":
							strField = "TO_CHAR(DATE_,'YYYY')";
							break;
						case "DATE_":
							strField = "TO_CHAR(DATE_,'MM/DD/YYYY')";
							break;
					}
					break;
				default:
					throw new NotImplementedException( "TODO: Create ANSI implementation for listBoxField_MouseDoubleClick()" );

			}
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strField);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strField.Length;
        }

        private void buttonEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " = ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonLessEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " >= ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonLessThan_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " > ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonGreaterEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " <= ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;
        }

        private void buttonGreaterThan_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " < ";
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

        private void buttonNotEqual_Click(object sender, EventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = " <> ";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;

        }

        private void listBoxValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int nPosition = textBoxSearch.SelectionStart;
            String strValue = "'" + listBoxValue.SelectedItem.ToString() + "'";
            String strSelect = textBoxSearch.Text.ToString();
            strSelect = strSelect.Insert(nPosition, strValue);
            textBoxSearch.Text = strSelect;
            textBoxSearch.SelectionStart = nPosition + strValue.Length;

        }
    }
}
