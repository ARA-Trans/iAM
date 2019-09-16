using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DatabaseManager;

namespace RoadCare3
{
    public partial class FormAssetKML : Form
    {
        private List<string> m_listColumns = new List<string>();
        public FormAssetKML(String strAsset)
        {
            InitializeComponent();
			//String strSelect = "SELECT TOP 1 * FROM " + strAsset;
			//try
			//{
			//    DataSet ds = DBMgr.ExecuteQuery(strSelect);

			//    foreach (DataColumn dc in ds.Tables[0].Columns)
			//    {
			//        m_listColumns.Add(dc.ColumnName);
			//    }
			//}
			//catch (Exception except)
			//{

			//    Global.WriteOutput("Error: Loading asset columns." + except.Message);
			//}
			m_listColumns = DBMgr.GetTableColumns( strAsset );
        }

        private void FormAssetKML_Load(object sender, EventArgs e)
        {
            foreach (String strProperty in m_listColumns)
            {
                comboBoxAttribute.Items.Add(strProperty);
                listBoxAvailable.Items.Add(strProperty);
                if (comboBoxAttribute.Items.Count > 0)
                {
                    comboBoxAttribute.SelectedIndex = 0;
                }
            }
        }
    }
}
