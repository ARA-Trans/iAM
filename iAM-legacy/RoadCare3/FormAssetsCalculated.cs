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
    public partial class FormAssetsCalculated : BaseForm
    {
        List<AssetObject> m_listAssets;
        List<CalculatedAssetObject> m_listCalculatedAssets;
        public FormAssetsCalculated()
        {
            InitializeComponent();
        }

        private void FormAssetsCalculated_Load(object sender, EventArgs e)
        {
            LoadCalculatedAssets();
        }

        private void LoadCalculatedAssets()
        {
            try
            {
                m_listAssets = GlobalDatabaseOperations.GetAssets();
            }
            catch (Exception ex)
            {
                Global.WriteOutput("Error: Retrieving RoadCare Assets from ASSET table." + ex.Message);
                return;
            }


            try
            {
                m_listCalculatedAssets = GlobalDatabaseOperations.GetCalculatedAssets();
            }
            catch (Exception ex)
            {
                Global.WriteOutput("Error: Retrieving RoadCare Calculated Assets from CALCULATED_ASSETS table. " + ex.Message);
                return;
            }
            DataGridViewComboBoxCell dgvComboAsset;
            foreach (CalculatedAssetObject asset in m_listCalculatedAssets)
            {
                int nIndex = dataGridViewCalculatedAssets.Rows.Add(asset.Asset, asset.CalculatedProperty, asset.Type, asset.Equation.Replace("|", "'"), asset.Criteria.Replace("|", "'"));
                dataGridViewCalculatedAssets.Rows[nIndex].Tag = asset.PrimaryKey;
                dgvComboAsset = (DataGridViewComboBoxCell)dataGridViewCalculatedAssets[0,nIndex];

                foreach (AssetObject assetTable in m_listAssets)
                {
                    dgvComboAsset.Items.Add(assetTable.Asset);
                }

            }

            dgvComboAsset = (DataGridViewComboBoxCell)dataGridViewCalculatedAssets[0, dataGridViewCalculatedAssets.Rows.Count - 1];
            foreach (AssetObject assetTable in m_listAssets)
            {
                dgvComboAsset.Items.Add(assetTable.Asset);
            }
        }

        private void dataGridViewCalculatedAssets_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridViewComboBoxCell dgvComboAsset;

            dgvComboAsset = (DataGridViewComboBoxCell)dataGridViewCalculatedAssets[0, e.RowIndex];
            foreach (AssetObject assetTable in m_listAssets)
            {
                dgvComboAsset.Items.Add(assetTable.Asset);
            }
 
        }

        private void FormAssetsCalculated_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.RemoveFormCalculatedAssets(this);
        }

        private void dataGridViewCalculatedAssets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 3) return;

            if (e.ColumnIndex == 3)
            {
                if (dataGridViewCalculatedAssets[0, e.RowIndex].Value == null || dataGridViewCalculatedAssets[2,0].Value==null)
                {
                    return;
                }
                String strAsset = dataGridViewCalculatedAssets[0, e.RowIndex].Value.ToString();
                String strType = dataGridViewCalculatedAssets[2, e.RowIndex].Value.ToString();
                String strEquation = "";
                if (dataGridViewCalculatedAssets[e.ColumnIndex, e.RowIndex].Value != null)
                {
                    strEquation = dataGridViewCalculatedAssets[e.ColumnIndex, e.RowIndex].Value.ToString();
                }
                FormEditAssetEquation formEquation = new FormEditAssetEquation(strEquation,strAsset,strType);
                if (formEquation.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewCalculatedAssets[e.ColumnIndex, e.RowIndex].Value = formEquation.Equation;
                }
            }

            if (e.ColumnIndex == 4)
            {
                if (dataGridViewCalculatedAssets[0, e.RowIndex].Value == null || dataGridViewCalculatedAssets[2, 0].Value == null)
                {
                    return;
                }
                String strAsset = dataGridViewCalculatedAssets[0, e.RowIndex].Value.ToString();
                String strType = dataGridViewCalculatedAssets[2, e.RowIndex].Value.ToString();
                String strCriteria = "";
                if (dataGridViewCalculatedAssets[e.ColumnIndex, e.RowIndex].Value != null)
                {
                    strCriteria = dataGridViewCalculatedAssets[e.ColumnIndex, e.RowIndex].Value.ToString();
                }

                FormAdvancedSearch formSearch = new FormAdvancedSearch(strCriteria, strAsset);
                if (formSearch.ShowDialog() == DialogResult.OK)
                {
                    dataGridViewCalculatedAssets[e.ColumnIndex, e.RowIndex].Value = formSearch.Query;
                }
            }

        }

        private void dataGridViewCalculatedAssets_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewCalculatedAssets.Rows[e.RowIndex].Tag == null)
            {
                InsertNewCalculatedAsset(e.RowIndex, e.ColumnIndex);
            }
            else
            {
                UpdateCalculatedAsset(e.RowIndex,e.ColumnIndex);
            }
        }

        private void InsertNewCalculatedAsset(int nRowIndex, int nColIndex)
        {
            String strTable = "";
            String strProperty = "";
            String strType = "";
            String strEquation = "";
            String strCriteria = "";

            if (dataGridViewCalculatedAssets[0, nRowIndex].Value != null) strTable = dataGridViewCalculatedAssets[0, nRowIndex].Value.ToString();
            if (dataGridViewCalculatedAssets[1, nRowIndex].Value != null) strProperty = dataGridViewCalculatedAssets[1, nRowIndex].Value.ToString();
            if (dataGridViewCalculatedAssets[2, nRowIndex].Value != null) strType = dataGridViewCalculatedAssets[2, nRowIndex].Value.ToString();
            if (dataGridViewCalculatedAssets[3, nRowIndex].Value != null) strEquation = dataGridViewCalculatedAssets[3, nRowIndex].Value.ToString();
            if (dataGridViewCalculatedAssets[4, nRowIndex].Value != null) strCriteria = dataGridViewCalculatedAssets[4, nRowIndex].Value.ToString();

            if (String.IsNullOrEmpty(strTable) || String.IsNullOrEmpty(strProperty) || String.IsNullOrEmpty(strType) || String.IsNullOrEmpty(strEquation))
            {
                return;
            }
            strEquation = strEquation.Replace("'", "|");
            strCriteria = strCriteria.Replace("'", "|");
            String strInsert = "INSERT INTO ASSETS_CALCULATED (ASSET,CALCULATED_PROPERTY,RETURN_TYPE,EQUATION,CRITERIA) VALUES ('" + strTable + "','" + strProperty + "','" + strType + "','" + strEquation + "','" + strCriteria + "')";
            try
            {
                DatabaseManager.DBMgr.ExecuteNonQuery(strInsert);
            }
            catch(Exception ex)
            {
                Global.WriteOutput("Error: Error inserting Calculated Asset. " + ex.Message);
            }

            String strIdentity = "";
                switch (DBMgr.NativeConnectionParameters.Provider)
                {
                    case "MSSQL":
                        strIdentity = "SELECT IDENT_CURRENT ('ASSETS_CALCULATED') FROM ASSETS_CALCULATED";
                        break;
                    case "ORACLE":
                        //strIdentity = "SELECT ASSETS_CALCULATED_SEQ.CURRVAL FROM DUAL";
						//strIdentity = "SELECT LAST_NUMBER - CACHE_SIZE  FROM USER_SEQUENCES WHERE SEQUENCE_NAME = 'ASSETS_CALCULATED_SEQ'";
						strIdentity = "SELECT MAX(ID_) FROM ASSETS_CALCULATED";
                        break;
                    default:
                        throw new NotImplementedException("TODO: Create ANSI implementation for XXXXXXXXXXXX");
                        //break;
                }
            DataSet ds = DBMgr.ExecuteQuery(strIdentity);
            strIdentity = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            dataGridViewCalculatedAssets.Rows[nRowIndex].Tag = strIdentity;
        
        }

        private void UpdateCalculatedAsset(int nRowIndex, int nColIndex)
        {
            String strID = dataGridViewCalculatedAssets.Rows[nRowIndex].Tag.ToString();
            String strUpdate = "UPDATE ASSETS_CALCULATED SET ";
            String strValue = dataGridViewCalculatedAssets[nColIndex, nRowIndex].Value.ToString();
            switch (nColIndex)
            {
                case 0:
                    strUpdate += "ASSET='" + strValue + "'";
                    break;
                case 1:
                    strUpdate += "CALCULATED_PROPERTY='" + strValue + "'";
                    break;
                case 3:
                    strUpdate += "EQUATION='" + strValue.Replace("'","|") + "'";
                    break;
                case 4:
                    strUpdate += "CRITERIA='" + strValue.Replace("'","|") + "'";
                    break;
                case 2:
                    strUpdate += "RETURN_TYPE='" + strValue + "'";
                    break;
                default:
                    return;
            }

            strUpdate += " WHERE ID='" + strID + "'";

            try
            {
                DatabaseManager.DBMgr.ExecuteNonQuery(strUpdate);
            }
            catch (Exception ex)
            {
                Global.WriteOutput("Error: Error inserting Calculated Asset. " + ex.Message);
            }
        }

    }
}
