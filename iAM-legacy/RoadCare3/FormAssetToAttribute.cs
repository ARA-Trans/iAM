using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DataObjects;
using RoadCareGlobalOperations;
using DatabaseManager;

namespace RoadCare3
{
    public partial class FormAssetToAttribute : BaseForm
    {
        List<AssetObject> m_listAsset;
        public FormAssetToAttribute()
        {
            InitializeComponent();
        }

        private void FormAssetToAttribute_Load(object sender, EventArgs e)
        {
            List<AttributeObject> listAssetAttributes = GlobalDatabaseOperations.GetAssetToAttributes();
            try
            {
                m_listAsset = GlobalDatabaseOperations.GetAssets();
            }
            catch (Exception ex)
            {
                Global.WriteOutput("Error: Retrieving RoadCare Assets from ASSET table." + ex.Message);
                return;
            }






            String strAsset = "";
            foreach (AttributeObject attributeObject in listAssetAttributes)
            {
                
                if(attributeObject.Type == "NUMBER")
                {
                    int nIndexTag = dataGridViewAssetToAttribute.Rows.Add(attributeObject.Asset, attributeObject.AssetProperty, attributeObject.Attribute, attributeObject.Format, attributeObject.DefaultNumber, attributeObject.Minimum, attributeObject.Maximum);
                    dataGridViewAssetToAttribute.Rows[nIndexTag].Tag = attributeObject.Type;
                }
                else
                {
                    int nIndexTag = dataGridViewAssetToAttribute.Rows.Add(attributeObject.Asset, attributeObject.AssetProperty, attributeObject.Attribute, "", attributeObject.DefaultString, "", "");
                    dataGridViewAssetToAttribute.Rows[nIndexTag].Tag = attributeObject.Type;
                }

            }

            int nIndex = 0;
            DataGridViewComboBoxCell dgvComboAsset;
            foreach (DataGridViewRow row in dataGridViewAssetToAttribute.Rows)
            {
                dgvComboAsset = (DataGridViewComboBoxCell)dataGridViewAssetToAttribute[0, nIndex];
                dataGridViewAssetToAttribute[2, nIndex].ReadOnly = true;
                foreach (AssetObject assetTable in m_listAsset)
                {
                    dgvComboAsset.Items.Add(assetTable.Asset);
                }
                nIndex++;
            }
            DataGridViewComboBoxCell dgvComboProperty;
            List<String> listProperty;
            nIndex = 0;
            foreach (DataGridViewRow row in dataGridViewAssetToAttribute.Rows)
            {
                if (dataGridViewAssetToAttribute[0, nIndex].Value != null)
                {
                    dgvComboProperty = (DataGridViewComboBoxCell)dataGridViewAssetToAttribute[1, nIndex];
                    strAsset = dataGridViewAssetToAttribute[0, nIndex].Value.ToString();
                    listProperty = DBMgr.GetTableColumns(strAsset);
                    foreach (String strProperty in listProperty)
                    {
                        dgvComboProperty.Items.Add(strProperty);
                    }
                }
                nIndex++;
            }
        }

        private void dataGridViewAssetToAttribute_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            String strUpdate;
            if (e.ColumnIndex == 0)
            {
                DataGridViewComboBoxCell dgvComboProperty = (DataGridViewComboBoxCell)dataGridViewAssetToAttribute[1, e.RowIndex];
                dgvComboProperty.Items.Clear();
                String strAsset = dataGridViewAssetToAttribute[0, e.RowIndex].Value.ToString();
                List<String>listProperty = DBMgr.GetTableColumns(strAsset);
                foreach (String strProperty in listProperty)
                {
                    dgvComboProperty.Items.Add(strProperty);
                   
                }
                dgvComboProperty.Value = "";
                strUpdate = "UPDATE ATTRIBUTES_ SET ASSET='" + strAsset + "' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch(Exception exception)
                {
                    Global.WriteOutput("Error: Updating ASSET column in ATTRIBUTES table." + exception.Message);
                }
            }
            else if(e.ColumnIndex == 1)
            {
                strUpdate = "UPDATE ATTRIBUTES_ SET ASSET_PROPERTY='" + dataGridViewAssetToAttribute[1,e.RowIndex].Value.ToString() + "' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Updating ASSET_PROPERTY column in ATTRIBUTES table." + exception.Message);
                }
                String strType = DBMgr.IsColumnTypeString(dataGridViewAssetToAttribute[0, e.RowIndex].Value.ToString(), dataGridViewAssetToAttribute[1, e.RowIndex].Value.ToString());

                if (strType == "String" || strType == "Boolean")
                {
                    strUpdate = "UPDATE ATTRIBUTES_ SET TYPE_='STRING' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                    dataGridViewAssetToAttribute.Rows[e.RowIndex].Tag = "STRING";
                }
                else
                {
                    strUpdate = "UPDATE ATTRIBUTES_ SET TYPE_='NUMBER' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                    dataGridViewAssetToAttribute.Rows[e.RowIndex].Tag = "NUMBER";
                }
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Updating TYPE column in ATTRIBUTES table." + exception.Message);
                }
            }
            else if (e.ColumnIndex == 3)
            {
                strUpdate = "UPDATE ATTRIBUTES_ SET FORMAT='" + dataGridViewAssetToAttribute[3, e.RowIndex].Value.ToString() + "' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Updating FORMAT column in ATTRIBUTES table." + exception.Message);
                }
            }
            else if (e.ColumnIndex == 4)
            {
                strUpdate = "UPDATE ATTRIBUTES_ SET DEFAULT_VALUE='" + dataGridViewAssetToAttribute[4, e.RowIndex].Value.ToString() + "' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Updating DEFAULT_VALUE column in ATTRIBUTES table." + exception.Message);
                }
            }
            else if (e.ColumnIndex == 5)
            {
                if (dataGridViewAssetToAttribute.Rows[e.RowIndex].Tag.ToString() == "STRING")
                {
                    strUpdate = "UPDATE ATTRIBUTES_ SET MINIMUM=NULL WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                }
                else
                {
                    strUpdate = "UPDATE ATTRIBUTES_ SET MINIMUM_='" + dataGridViewAssetToAttribute[5, e.RowIndex].Value.ToString() + "' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                }
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Updating MINIMUM column in ATTRIBUTES table." + exception.Message);
                }
            }
            else if (e.ColumnIndex == 6)
            {
                if (dataGridViewAssetToAttribute.Rows[e.RowIndex].Tag.ToString() == "STRING")
                {
                    strUpdate = "UPDATE ATTRIBUTES_ SET MAXIMUM=NULL WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                }
                else
                {
                    strUpdate = "UPDATE ATTRIBUTES_ SET MAXIMUM='" + dataGridViewAssetToAttribute[6, e.RowIndex].Value.ToString() + "' WHERE ATTRIBUTE_='" + dataGridViewAssetToAttribute[2, e.RowIndex].Value.ToString() + "'";
                }
                try
                {
                    DBMgr.ExecuteNonQuery(strUpdate);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Updating MAXIMUM column in ATTRIBUTES table." + exception.Message);
                }
            }
        }

        private void buttonAddAttribute_Click(object sender, EventArgs e)
        {
            FormAddAssetToAttribute form = new FormAddAssetToAttribute();
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int nIndex = dataGridViewAssetToAttribute.Rows.Add("", "", form.Attribute);
                    DataGridViewComboBoxCell dgvComboAsset;
                    dgvComboAsset = (DataGridViewComboBoxCell)dataGridViewAssetToAttribute[0, nIndex];
                    dataGridViewAssetToAttribute[2, nIndex].ReadOnly = true;
                    foreach (AssetObject assetTable in m_listAsset)
                    {
                        dgvComboAsset.Items.Add(assetTable.Asset);
                    }

                    String strInsert = "INSERT INTO ATTRIBUTES_ (ATTRIBUTE_,NATIVE_,PROVIDER,CALCULATED) VALUES('" + form.Attribute + "','True','MSSQL','True')";
                    DBMgr.ExecuteNonQuery(strInsert);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Creating new Asset to Attribute. " + exception.Message);
                }
            }
        }

        private void FormAssetToAttribute_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveAssetToAttribute(this);
        }

        private void dataGridViewAssetToAttribute_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
           
        }

        private void dataGridViewAssetToAttribute_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Cells[2].Value.ToString() != null)
            {
                String strAttribute = e.Row.Cells[2].Value.ToString();
                String strDelete = "DELETE FROM ATTRIBUTES_ WHERE ATTRIBUTE_='" + strAttribute + "'";
                try
                {
                    DBMgr.ExecuteNonQuery(strDelete);
                }
                catch (Exception exception)
                {
                    Global.WriteOutput("Error: Removing ATTRIBUTE " + strAttribute + " ATTRIBUTES table." + exception.Message);
                }
            }
        }
    }
}
