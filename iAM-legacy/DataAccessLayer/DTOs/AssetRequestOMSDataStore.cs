using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    public class AssetRequestOMSDataStore
    {
        DateTime _beforeDate;
        string _assetTypeName;
        List<AssetRequestOMSTableStore> _tables;
        OMSAssetConditionIndexStore _conditionIndex;


        /// <summary>
        /// Asset for which tables are desired.
        /// </summary>
        public string AssetTypeName
        {
            get { return _assetTypeName; }
            set { _assetTypeName = value; }
        }

        /// <summary>
        /// Entry date on or before which data is desired.
        /// </summary>
        public DateTime BeforeDate
        {
            get { return _beforeDate; }
            set { _beforeDate = value; }
        }

        /// <summary>
        /// List of tables for which data is desired.
        /// </summary>
        public List<AssetRequestOMSTableStore> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        public OMSAssetConditionIndexStore ConditionIndex
        {
            get { return _conditionIndex; }
            set { _conditionIndex = value; }
        }

        /// <summary>
        /// This object is passed to the OMS class to request data for the contained tables.
        /// </summary>
        /// <param name="beforeDate">The date on of before which the data is desired.</param>
        /// <param name="attributes">A unique list of Attribute Stores required to perform the simulation</param>
        public AssetRequestOMSDataStore(DateTime beforeDate, List<AttributeStore> attributes, OMSAssetConditionIndexStore conditionIndex)

        {
            _beforeDate = beforeDate;
            _conditionIndex = conditionIndex;
            _tables = new List<AssetRequestOMSTableStore>();
            if (attributes != null && attributes.Count > 0)
            {
                _assetTypeName = attributes[0].AssetType;

                foreach (AttributeStore attribute in attributes)
                {
                    if (!attribute.IsConditionCategory && attribute.AttributeField != "AGE")//Ignore AGE which is calculated in DecisionEngine and ConditionIndices which are got with different.
                    {
                        int index = Tables.FindIndex(delegate(AssetRequestOMSTableStore art) { return art.TableName == attribute.OmsTable; });
                        if (index < 0)
                        {
                            string foreignKey = null;
                            string primaryKey = null;
                            string foreignKeyTable = null;
                            AssetRequestOMSTableStore table = new AssetRequestOMSTableStore(attribute.OmsTable);
                            table.Columns.Add(new AssetRequestOMSColumnStore(attribute.AttributeField.ToString(), attribute.OmsObjectUserIDHierarchy.ToString(),attribute.AttributeField));
                            OMS.GetPrimaryKeyForLookups(attribute.OmsOIDHierarchy, out primaryKey, out foreignKey, out foreignKeyTable);
                            table.PrimaryKeyColumn = primaryKey;
                            table.ForeignKeyColumn = foreignKey;
                            table.ForeignKeyTable = foreignKeyTable;
                            _tables.Add(table);
                        }
                        else
                        {
                            Tables[index].Columns.Add(new AssetRequestOMSColumnStore(attribute.AttributeField.ToString(), attribute.OmsObjectUserIDHierarchy.ToString(),attribute.AttributeField));
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return _assetTypeName + " before " + _beforeDate.ToString();
        }
    }
}
