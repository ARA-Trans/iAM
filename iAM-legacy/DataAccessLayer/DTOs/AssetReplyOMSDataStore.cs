using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccessLayer.DTOs
{
    /// <summary>
    /// Reply from OMS database to DecisionEngine
    /// </summary>
    public class AssetReplyOMSDataStore
    {
        string _OID;
        List<AssetReplyOMSTableStore> _tables;
        List<AssetReplyOMSConditionIndex> _conditionIndices;


        /// <summary>
        /// OID of the asset
        /// </summary>
        public string OID
        {
            get { return _OID; }
            set { _OID = value; }
        }

        /// <summary>
        /// List of tables for which OMS data was returned.
        /// </summary>
        public List<AssetReplyOMSTableStore> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }


        /// <summary>
        /// List of latest condition indices
        /// </summary>
        public List<AssetReplyOMSConditionIndex> ConditionIndices
        {
            get { return _conditionIndices; }
            set { _conditionIndices = value; }
        }

        /// <summary>
        /// All of the asset properties requested by Decision engine along with the assets uniqueID.
        /// </summary>
        /// <param name="_OID"></param>
        /// <param name="tables"></param>
        public AssetReplyOMSDataStore(string OID)
        {
            _OID = OID;
            _tables = new List<AssetReplyOMSTableStore>();
        }
       

        /// <summary>
        /// Gets the OMS asset.properties data in a form desired by the DecisionEngine
        /// </summary>
        /// <returns>A list of AttribueValueDE containing OMS ObjectUserID Hierarchy, Values and LastEntry date</returns>
        public List<AttributeValueDEStore> GetDecisionEngineAttributes(List<AttributeStore> attributes,AssetRequestOMSDataStore assetRequest,Dictionary<string, AssetReplyOMSLookupTable> assetLookups)
        {
            List<AttributeValueDEStore> deAttributes = new List<AttributeValueDEStore>();

            foreach (AssetReplyOMSTableStore table in _tables)
            {
                AssetRequestOMSTableStore requestedLookupTables = assetRequest.Tables.Find(delegate(AssetRequestOMSTableStore a) { return a.ForeignKeyTable == table.OmsTable; });
                string key = "";
                foreach (AssetReplyOMSColumnStore column in table.Columns)
                {
                    deAttributes.Add(new AttributeValueDEStore(column.OmsObjectUserIDHierarchy, column.Value, table.DateLastEntry,column.AttributeField));

                    if(requestedLookupTables != null && column.OmsObjectUserIDHierarchy == requestedLookupTables.ForeignKeyColumn)
                    {
                        key = column.Value;
                    }
                }

                
                if(requestedLookupTables != null)
                {
                    string primaryKey = requestedLookupTables.PrimaryKeyColumn;
                    
                    if(assetLookups.ContainsKey(requestedLookupTables.TableName))
                    {
                        AssetReplyOMSLookupTable lookup = assetLookups[requestedLookupTables.TableName];
                        
                        if(key != null && lookup.LookupData.ContainsKey(key))
                        {
                            Dictionary<string,string> values = lookup.LookupData[key];
                        
                            foreach (AssetRequestOMSColumnStore column in requestedLookupTables.Columns)
                            {
                                if (column.OmsObjectUserIDHierarchy != null)
                                {
                                
                                    deAttributes.Add(new AttributeValueDEStore(column.OmsObjectUserIDHierarchy,values[column.ColumnName] , table.DateLastEntry,column.AttributeField));
                                }
                            }
                        }
                        else if(key == null)
                        {
                            foreach (AssetRequestOMSColumnStore column in requestedLookupTables.Columns)
                            {
                                if (column.OmsObjectUserIDHierarchy != null)
                                {

                                    deAttributes.Add(new AttributeValueDEStore(column.OmsObjectUserIDHierarchy, null, table.DateLastEntry,column.AttributeField));
                                }
                            }
                        }
                    }
                }
            }

            foreach (AssetReplyOMSConditionIndex aroci in this.ConditionIndices)
            {
                deAttributes.Add(new AttributeValueDEStore("__" + aroci.ConditionIndex.Replace(" ", "").Replace("/", ""), aroci.Value.ToString(), aroci.InspectionDate));
            }

            return deAttributes;
        }

        public override string ToString()
        {
            return _OID;
        }
    }
}
