using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Utility.ExceptionHandling;
using DataAccessLayer.DTOs;
using DataAccessLayer.MSSQL;

namespace DataAccessLayer
{
    public static class OMS
    {

        static Dictionary<string, List<AttributeStore>> _assetAttributeDictionary;
        static List<AssetTypeStore> _assetTypes;
        static List<OMSAssetConditionIndexStore> _conditionIndexes;
        static List<SessionStore> _sessionOIDs = new List<SessionStore>();

        public static List<SessionStore> Sessions
        {
            get { return _sessionOIDs; }
            set { _sessionOIDs = value; }
        }


        public static string ThrowException()
        {
            
            try
            {
                Exception ex = new Exception("Example of an DataAccessLayer ELMAH exception");
                throw ex;
            }
            catch (Exception e)
            {
                Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(e, false);
            }
            return "Success";
        }


        /// <summary>
        /// Gets all of the asset types in the OMS database
        /// </summary>
        /// <returns>Asset types in the OMS database as a string</returns>
        public static List<AssetTypeStore> GetAssetTypes()
        {
            List<AssetTypeStore> assetTypes = new List<AssetTypeStore>();
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT AssetType, AssetTypesOID FROM AssetTypes WHERE AssetType<>'Non-Asset'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        AssetTypeStore assetType = new AssetTypeStore(dr["AssetType"].ToString(), dr["AssetTypesOID"].ToString());
                        assetTypes.Add(assetType);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    assetTypes = null;
                }
            }
            _assetTypes = assetTypes;
            return assetTypes;
        }

        // Calls extended version of this function.
        public static List<AttributeStore> GetAssetAttributes(string OID, string assetName, StringBuilder omsDisplayNameHierarchy, StringBuilder omsObjectUserIDHierarchy, StringBuilder omsOIDHierarchy, List<AttributeStore> attributes)
        {
            return GetAssetAttributes(OID, assetName, omsDisplayNameHierarchy, omsObjectUserIDHierarchy, omsOIDHierarchy, attributes, true);
        }


        /// <summary>
        /// Gets a list of AssetAttributes associated with the passed in asset type.
        /// </summary>
        /// <param name="assetType">The name of the asset type to lookup its attribute fields. (signs, pavement, etc)</param>
        /// <returns>A list of AttributeStore which contain data for each attribute associated with the passed in asset type.</returns>
        public static List<AttributeStore> GetAssetAttributes(string OID, string assetName, StringBuilder omsDisplayNameHierarchy, StringBuilder omsObjectUserIDHierarchy, StringBuilder omsOIDHierarchy, List<AttributeStore> attributes, bool isRecursive)
        {
            string inspectionOID = null;

            if (OID != null && assetName != null)
            {
                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    try
                    {
                        connection.Open();

                        // Get the parent ID of the passed in OID along with some other necessary data.
                        string query = "SELECT OID, ObjectName, ObjectType, ObjectUserID FROM cgSysObjects WHERE ParentID=@parentOID ORDER BY ObjectType";
                        SqlCommand cmd = new SqlCommand(query, connection);
                        cmd.Parameters.Add(new SqlParameter("parentOID", OID));

                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            string retrievedOID = dr["OID"].ToString();

                            // Builds the RoadCare equivalent hierarchy of OMS "display names".
                            string objectDisplayName = dr["ObjectName"].ToString();
                            string objectUserID = dr["ObjectUserID"].ToString();
                            string objectType = dr["ObjectType"].ToString();
                            if(objectType == "2" && objectDisplayName != "Inspections")
                            {
                                continue;
                            }

                            if (omsDisplayNameHierarchy == null)
                            {
                                omsDisplayNameHierarchy = new StringBuilder();
                            }
                            if (omsDisplayNameHierarchy.Length > 0)
                            {
                                omsDisplayNameHierarchy.Append("|");
                            }
                            omsDisplayNameHierarchy.Append(objectDisplayName);

                            if (omsObjectUserIDHierarchy == null)
                            {
                                omsObjectUserIDHierarchy = new StringBuilder();
                            }
                            if (omsObjectUserIDHierarchy.Length > 0)
                            {
                                omsObjectUserIDHierarchy.Append("|");
                            }
                            omsObjectUserIDHierarchy.Append(objectUserID);

                            if (omsOIDHierarchy == null)
                            {
                                omsOIDHierarchy = new StringBuilder();
                            }
                            if (omsOIDHierarchy.Length > 0)
                            {
                                omsOIDHierarchy.Append("|");
                            }
                            omsOIDHierarchy.Append(retrievedOID);



                            // CRITICAL:  4 is a top level column or field.  This means it has no associated child recordsets (can still have lookups to recordsets here).
                            // if we get a 2, then we need to recursively call GetAssetAttributes and pass in the retrievedOID to lookup against the parent OID.
                            // Once completed, we have complete information about a single OMS field.
                            
                            if (objectType == "4")
                            {
                                AttributeStore currentAttribute = BuildAttributeStore(retrievedOID, assetName, omsDisplayNameHierarchy.ToString(), omsObjectUserIDHierarchy.ToString(), omsOIDHierarchy.ToString());
                                string[] oids = currentAttribute.OmsOIDHierarchy.Split('|');

                                int toFind = omsDisplayNameHierarchy.ToString().LastIndexOf('|');
                                if (toFind == -1)
                                {
                                    // No pipe, new the omsBuilder
                                    omsDisplayNameHierarchy = new StringBuilder();
                                }
                                else
                                {
                                    string temp = omsDisplayNameHierarchy.ToString().Substring(0, toFind);
                                    omsDisplayNameHierarchy.Clear();
                                    omsDisplayNameHierarchy.Append(temp);
                                }

                                int objectUserIDToFind = omsObjectUserIDHierarchy.ToString().LastIndexOf('|');
                                if (objectUserIDToFind == -1)
                                {
                                    // No pipe, new the omsBuilder
                                    omsObjectUserIDHierarchy = new StringBuilder();
                                }
                                else
                                {
                                    string temp = omsObjectUserIDHierarchy.ToString().Substring(0, objectUserIDToFind);
                                    omsObjectUserIDHierarchy.Clear();
                                    omsObjectUserIDHierarchy.Append(temp);
                                }

                                int oidToFind = omsOIDHierarchy.ToString().LastIndexOf('|');
                                if (oidToFind == -1)
                                {
                                    // No pipe, new the omsBuilder
                                    omsOIDHierarchy = new StringBuilder();
                                }
                                else
                                {
                                    string temp = omsOIDHierarchy.ToString().Substring(0, oidToFind);
                                    omsOIDHierarchy.Clear();
                                    omsOIDHierarchy.Append(temp);
                                }

                                attributes.Add(currentAttribute);
                                //AppendLookupData(currentAttribute, attributes, isRecursive);
                            }
                            else
                            {
                                if (isRecursive)
                                {
                                    if (omsDisplayNameHierarchy.ToString() != "Inspections")
                                    {
                                        GetAssetAttributes(retrievedOID, assetName, omsDisplayNameHierarchy, omsObjectUserIDHierarchy, omsOIDHierarchy, attributes);
                                    }
                                    else
                                    {
                                        inspectionOID = retrievedOID;
                                    }
                                }

                                int toFind = omsDisplayNameHierarchy.ToString().LastIndexOf('|');
                                if (toFind == -1)
                                {
                                    omsDisplayNameHierarchy = new StringBuilder();
                                }
                                else
                                {
                                    string temp = omsDisplayNameHierarchy.ToString().Substring(0, toFind);
                                    omsDisplayNameHierarchy.Clear();
                                    omsDisplayNameHierarchy.Append(temp);
                                }

                                int objectUserIDToFind = omsObjectUserIDHierarchy.ToString().LastIndexOf('|');
                                if (objectUserIDToFind == -1)
                                {
                                    omsObjectUserIDHierarchy = new StringBuilder();
                                }
                                else
                                {
                                    string temp = omsObjectUserIDHierarchy.ToString().Substring(0, objectUserIDToFind);
                                    omsObjectUserIDHierarchy.Clear();
                                    omsObjectUserIDHierarchy.Append(temp);
                                }

                                int oidToFind = omsOIDHierarchy.ToString().LastIndexOf('|');
                                if (oidToFind == -1)
                                {
                                    omsOIDHierarchy = new StringBuilder();
                                }
                                else
                                {
                                    string temp = omsOIDHierarchy.ToString().Substring(0, oidToFind);
                                    omsOIDHierarchy.Clear();
                                    omsOIDHierarchy.Append(temp);
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        DataAccessExceptionHandler.HandleException(e, false);
                        attributes = null;
                    }
                }
            }

            if (inspectionOID != null)
            {
                GetConditionAttribute(inspectionOID,assetName,attributes);
            }
            return attributes;
        }

        private static List<AttributeStore> GetConditionAttribute(string inspectionOID, string assetName, List<AttributeStore> attributes)
        {
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();

                    // Get the parent ID of the passed in OID along with some other necessary data.
                    string query = "SELECT OID, ObjectName, ObjectType, ObjectUserID FROM cgSysObjects WHERE ParentID=@parentOID  AND ObjectUserID='cgConditionCategories' ORDER BY ObjectType";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("parentOID", inspectionOID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string retrievedOID = dr["OID"].ToString();
                        string objectDisplayName = dr["ObjectName"].ToString();
                        string objectUserID = dr["ObjectUserID"].ToString();

                        StringBuilder omsDisplayNameHierarchy = new StringBuilder("Inspections|Condition Categories");
                        StringBuilder omsObjectUserIDHierarchy = new StringBuilder("cgInpections|cgConditionCategories");
                        StringBuilder omsOIDHierarchy = new StringBuilder(inspectionOID + "|" + retrievedOID);
                        GetAssetAttributes(retrievedOID, assetName, omsDisplayNameHierarchy, omsObjectUserIDHierarchy, omsOIDHierarchy, attributes);
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            return attributes;
        }

        private static void AppendLookupData(AttributeStore currentAttribute, List<AttributeStore> attributes, bool isRecursive)
        {
            if (currentAttribute.LookupOID != "0")
            {
                List<string> columnOIDsInLookupTable = GetLookupFieldTableColumns(currentAttribute.LookupOID);

                // Did we find at least 1 lookup field?  Ok, go get all of the columns associated with the lookup.
                if (columnOIDsInLookupTable.Count > 0)
                {
                    foreach (string columnOID in columnOIDsInLookupTable)
                    {
                        
                        
                        AttributeStore lookupAttribute = BuildAttributeStore(columnOID, currentAttribute.AssetType, currentAttribute.OmsHierarchy, currentAttribute.OmsObjectUserIDHierarchy,currentAttribute.OmsOIDHierarchy);
                        if (!currentAttribute.OmsObjectUserIDHierarchy.Contains("CurrentInspectionID|TaskID|"))//THIS CHECK STOP EXTREMELY DEEP RECURSION
                        {
                            // If our lookup field does not lead to a circular reference attributes.exists will return false
                            string[] oids = currentAttribute.OmsOIDHierarchy.Split('|');
                            bool isExist = oids.ToList().Contains(lookupAttribute.OID); 
                            if (!isExist && currentAttribute.LookupOID != lookupAttribute.OID)
                            {
                                attributes.Add(lookupAttribute);
                                lookupAttribute.OmsHierarchy = currentAttribute.OmsHierarchy + "|" + lookupAttribute.DisplayName;
                                lookupAttribute.OmsObjectUserIDHierarchy = currentAttribute.OmsObjectUserIDHierarchy + "|" + lookupAttribute.AttributeField;
                                lookupAttribute.OmsOIDHierarchy = currentAttribute.OmsOIDHierarchy + "|" + lookupAttribute.OID;
                                if (isRecursive)
                                {
                                    AppendLookupData(lookupAttribute, attributes, isRecursive);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static List<string> GetLookupFieldTableColumns(string lookupOID)
        {
            List<string> columnOIDsInLookupTable = new List<string>();
            string tableName = null;
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                connection.Open();
                string query = "SELECT TableName FROM cgSysFields WHERE OID = " + lookupOID;
                SqlCommand cmd = new SqlCommand(query, connection);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    tableName = dr["TableName"].ToString();
                }
            }
            if (tableName != null)
            {
                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    connection.Open();
                    string query = "SELECT OID FROM cgSysFields WHERE TableName = '" + tableName + "'";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        columnOIDsInLookupTable.Add(dr["OID"].ToString());
                    }
                }
            }
            return columnOIDsInLookupTable;
        }




        private static string GetRecordOID(string tableName, string columnName)
        {
            string OID = null;
            string query = "SELECT OID FROM cgSysFields WHERE TableName = '" + tableName + "' AND ColumnMap = '" + columnName + "'";
            using (SqlConnection connFieldType = new SqlConnection(DB.OMSConnectionString))
            {
                connFieldType.Open();
                SqlCommand cmdFieldType = new SqlCommand(query, connFieldType);
                OID = cmdFieldType.ExecuteScalar().ToString();
            }
            return OID;
        }

        /// <summary>
        /// Creates an AttributeStore for a given table/column, represented as a single OMS field.
        /// </summary>
        /// <param name="OID">The OID of the recordset or column you are looking for.</param>
        /// <param name="assetName">The asset type name.</param>
        /// <returns>All meta data associated with a given column.</returns>
        private static AttributeStore BuildAttributeStore(string OID, string assetName, string omsHierarchy, string omsObjectUserIDHierarchy, string omsOIDHierarchy)
        {
            AttributeStore toReturn = null;
            List<string> lookupValues = null;
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();

                    // Gets information related to the AttributeStore, which represents a single OMS field.
                    string query = "SELECT ObjectName, TableName, ColumnMap, LookupField, FieldType, ObjectDescription, ValidationRule, InitialValue, UnitDefaultID FROM cgSysObjects objs INNER JOIN cgSysFields fields ON objs.OID = fields.OID WHERE objs.OID=@OID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("OID", OID));

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string objectName = dr["ObjectName"].ToString();
                        string tableName = dr["TableName"].ToString();
                        string columnMap = dr["ColumnMap"].ToString();
                        string fieldType = dr["FieldType"].ToString();
                        
                        string objDescription = null;
                        if(dr["ObjectDescription"] != DBNull.Value)
                        {
                            objDescription = dr["ObjectDescription"].ToString();
                        }

                        string validationRule = null;
                        if(dr["ValidationRule"] != DBNull.Value)
                        {
                            validationRule = dr["ValidationRule"].ToString();
                        }

                        string initialValue = null;
                        if(dr["InitialValue"] != DBNull.Value)
                        {
                            initialValue = dr["InitialValue"].ToString();
                        }

                        // If 0, then we are done, if > 0 we need to get the values associated with the lookup field.
                        string lookupFieldOID = dr["LookupField"].ToString();
                        if (lookupFieldOID != "0")
                        {
                            lookupValues = GetLookupTableColumn(lookupFieldOID);
                        }

                        // Lets lookup the default unit type.
                        string defaultUnitOID = dr["UnitDefaultID"].ToString();
                        string defaultUnit = null;
                        if (defaultUnitOID != "0")
                        {
                            defaultUnit = GetDefaultUnits(defaultUnitOID);
                        }

                        // Get the format of the field
                        string format = GetFormat(/* Format1, Format2, Format3 */);
                        toReturn = new AttributeStore(OID,assetName, tableName, columnMap, objectName, objDescription, lookupValues, initialValue,
                            format, defaultUnit, omsHierarchy.ToString(), omsObjectUserIDHierarchy.ToString(), omsOIDHierarchy.ToString(), lookupFieldOID, GetDecisionEngineFieldType(fieldType));
                    }


                }
                catch (Exception exc)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(exc, false);
                    toReturn = null;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Gets the format (G, F1, F2, etc) of a given field.
        /// </summary>
        /// <returns>The correct .ToString() format statement.</returns>
        private static string GetFormat()
        {
            // TODO: Implement this.
            return null;
        }

        /// <summary>
        /// Gets the default units for a given field in OMS.
        /// </summary>
        /// <param name="defaultUnitOID">The field we want to know the default units for.</param>
        /// <returns>The default units as a string.</returns>
        private static string GetDefaultUnits(string defaultUnitOID)
        {
            // TODO: Implement this.
            return null;
        }

        private static string GetDecisionEngineFieldType(string omsFieldType)
        {
            //public enum OMSFieldTypes { Text, YesNo, Integer, Number, Currency, DateTime, Lookup, OLEObj, Coordinate, Attachment, Quantity, Date, Time, Unknown=100 };

            string decisionEngineType = "";
            int omsType = int.Parse(omsFieldType);
            switch(omsType)
            {
                case (int)DB.OMSFieldTypes.Text:
                    decisionEngineType = "Text";
                    break;
                case (int)DB.OMSFieldTypes.YesNo:
                    decisionEngineType = "YesNo";
                    break;
                case (int)DB.OMSFieldTypes.Integer:
                    decisionEngineType = "Integer";
                    break;
                case (int)DB.OMSFieldTypes.Number:
                    decisionEngineType = "Number";
                    break;
                case (int)DB.OMSFieldTypes.Currency:
                    decisionEngineType = "Currency";
                    break;
                case (int)DB.OMSFieldTypes.DateTime:
                    decisionEngineType = "DateTime";
                    break;
                case (int)DB.OMSFieldTypes.Lookup:
                    decisionEngineType = "Lookup";
                    break;
                case (int)DB.OMSFieldTypes.Quantity:
                    decisionEngineType = "Quantity";
                    break;
                case (int)DB.OMSFieldTypes.Date:
                    decisionEngineType = "Date";
                    break;
                case (int)DB.OMSFieldTypes.Time:
                    decisionEngineType = "Time";
                    break;
            }
            return decisionEngineType;
        }

        /// <summary>
        /// Gets the table and columns for each OMS field.
        /// </summary>
        /// <param name="lookupFieldOID">The OID of the table and column.</param>
        /// <returns>List of unqiue lookup values.</returns>
        private static List<string> GetLookupTableColumn(string lookupFieldOID)
        {
            List<string> lookupValues = null;
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();

                    // Get the table and column of the desired lookup field
                    string query = "SELECT TableName, ColumnMap FROM cgSysFields WHERE OID=@lookupFieldOID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("lookupFieldOID", lookupFieldOID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string tableName = dr["TableName"].ToString();
                        string columnMap = dr["ColumnMap"].ToString();
                        lookupValues = GetLookupValues(tableName, columnMap);
                    }
                }
                catch (Exception exc)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(exc, false);
                    lookupValues = null;
                }
            }
            return lookupValues;
        }

        public static void GetLookupTableColumn(string lookupFieldOID, out string table, out string column)
        {
            table = null;
            column = null;

            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    // Get the table and column of the desired lookup field
                    string query = "SELECT TableName, ColumnMap FROM cgSysFields WHERE OID=@lookupFieldOID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("lookupFieldOID", lookupFieldOID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        table = dr["TableName"].ToString();
                        column = dr["ColumnMap"].ToString();
                    }
                }
                catch (Exception exc)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(exc, false);
                }
            }
        }

        /// <summary>
        /// Gets the unique values for a lookup field for the table and column provided.
        /// </summary>
        /// <param name="tableName">The OMS table name.</param>
        /// <param name="columnMap">The OMS field name.</param>
        /// <returns>Unique lookup values.</returns>
        private static List<string> GetLookupValues(string tableName, string columnMap)
        {
            List<string> toReturn = null;
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT "+ columnMap + " FROM " + tableName;
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();
                    toReturn = new List<string>();
                    while (dr.Read())
                    {
                        toReturn.Add(dr[columnMap].ToString());
                    }
                }
                catch (Exception exc)
                {
                    Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(exc, false);
                    toReturn = null;
                }
            }
            return toReturn;
        }
        
        /// <summary>
        /// Gets list of Budget Categories from a given OMS Asset type.
        /// </summary>
        /// <param name="assetType">OMS asset type</param>
        /// <returns></returns>
        //public static List<string> GetBudgetCategories(string assetType)
        //{
        //    List<string> budgetCategoryList = new List<string>();
        //    return budgetCategoryList;
        //}

        /// <summary>
        /// Get the number of assets which match given filter.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static int GetNumberAsset(string simulationID)
        {
            int countAsset = 0;
            string reportTable = DB.TablePrefix + "REPORT_1_" + simulationID;
            if (DB.CheckIfTableExists(reportTable) != 0)
            {


                using (SqlConnection connection = new SqlConnection(DB.ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT COUNT(DISTINCT SECTIONID) FROM " + reportTable;
                        SqlCommand cmd = new SqlCommand(query, connection);
                        object o = cmd.ExecuteScalar();
                        countAsset = Convert.ToInt32(o);
                    }
                    catch (Exception exc)
                    {
                        Utility.ExceptionHandling.DataAccessExceptionHandler.HandleException(exc, false);
                        countAsset = 0;
                    }
                }
            }
            return countAsset;
        }

        /// <summary>
        /// Get the current average of all OCI (correct for AREA) for given Asset and Filter.
        /// </summary>
        /// <param name="asset">OMS asset type</param>
        /// <param name="filter">OMS scope filter</param>
        /// <returns></returns>
        public static string GetCurrentOCI(string asset, string filter)
        {
            //Chad need you to retreive this information from OMS
            string tempReturn = "42";
            return tempReturn;
        }

        /// <summary>
        /// Given an asset name, we return the asset OID for that asset type.
        /// </summary>
        /// <param name="assetName">The display name of the asset to lookup.</param>
        /// <returns>The OID of the asset type.</returns>
        public static string GetAssetTypeOID(string assetName)
        {
            string oidToReturn = null;
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT AssetTypesOID FROM AssetTypes WHERE AssetType = @assetName";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("assetName", assetName));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        oidToReturn = dr["AssetTypesOID"].ToString();
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    oidToReturn = null;
                }
            }
            return oidToReturn;
        }


        /// <summary>
        /// This functions intializes the _assetAttributeDictionary so we are not constantly regenerating the List<AttributeStores> for a give asset
        /// </summary>
        public static void FillAllAssetAttributes(string assetType)
        {
            List<AssetTypeStore> listAssets = GetAssetTypes();
            if (_assetAttributeDictionary == null)
            {
                _assetAttributeDictionary = new Dictionary<string, List<AttributeStore>>();
            }

            if(listAssets != null)
            {
                AssetTypeStore asset = listAssets.Find(delegate(AssetTypeStore at) { return at.AssetName == assetType; });
                List<AttributeStore> attributes = OMS.GetAssetAttributes(asset.OID,asset.AssetName, null, null,null, new List<AttributeStore>());
                _assetAttributeDictionary.Add(asset.AssetName.ToString(), attributes);
            }
        }

        /// <summary>
        /// Gets a list of all attributes associated with an asset (Sign, Pavement, etc).
        /// </summary>
        /// <param name="assetName">Name of the Asset (Sign, Pavement, etc)</param>
        /// <returns>List of OMS attributes with type, lookups, filter structure.</returns>
        public static List<AttributeStore> GetAssetAttributes(string assetName)
        {
            if (_assetAttributeDictionary == null) FillAllAssetAttributes(assetName);
            if (!_assetAttributeDictionary.ContainsKey(assetName))
            {
                FillAllAssetAttributes(assetName);
            }
            List<AttributeStore> attributes = null;
            if (_assetAttributeDictionary != null && _assetAttributeDictionary.ContainsKey(assetName))
            {
                attributes = _assetAttributeDictionary[assetName];
            }
            return attributes;
        }


        /// <summary>
        /// Implementation of lazy load.
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="attributeBranch"></param>
        /// <returns></returns>
        public static List<AttributeStore> GetAssetAttributes(string assetName, string attributeBranch)
        {
            List<AssetTypeStore> listAssets = GetAssetTypes();
            List<AttributeStore> attributes = null;
            if(listAssets != null)
            {
                AssetTypeStore asset = listAssets.Find(delegate(AssetTypeStore at) { return at.AssetName == assetName; });
                attributes = OMS.GetAssetAttributes(asset.OID,asset.AssetName, null, null,null, new List<AttributeStore>(),true);
            }
            return attributes;
        }



        /// <summary>
        /// Called to have DecisionEngine reinitialize attributes from OMS.  This should be rarely used.  Only if OMS values change.
        /// </summary>
        public static void ResetAttributes()
        {
            _assetAttributeDictionary = null;
            _assetTypes = null;
            _conditionIndexes = null;
            //FillAllAssetAttributes();
        }

        /// <summary>
        /// Get all of the activities/Treatments associated with a give asset
        /// </summary>
        /// <param name="assetName">Asset name for which Activites are desired</param>
        /// <returns>List of activities</returns>
        public static List<OMSActivityStore> GetActivities(string assetName)
        {
            string assetTypesOID = GetAssetTypeOID(assetName);

            List<OMSActivityStore> activities = null;
            if (assetTypesOID != null)
            {


                OMSAssetConditionIndexStore assetOCI = GetAssetConditionIndex(assetName);
                string condCategoriesTable = assetOCI.ConditionCategoryTable.Replace("ConditionCategories", "CondCategories");
                string impactTable = assetOCI.ConditionCategoryTable.Replace("ConditionCategories", "Impacts");      //GetImpactTable(assetTypesOID, condCategoriesTable);//This works to get xxxxCondCategories and does a ForeignKey lookup.
                string cgName = GetObjectUserID(assetTypesOID);
                
                if(cgName != null && impactTable != null && condCategoriesTable != null)
                {
                    using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "SELECT ActivitiesOID, Activity, UnitCost,UnitCost_unit FROM Activities WHERE Inspect='0' AND RetireValue='0' AND InstallValue='0' AND AppliesTo" + cgName +"='1'";
                            SqlCommand cmd = new SqlCommand(query, connection);
                            SqlDataReader dr = cmd.ExecuteReader();
                            activities = new List<OMSActivityStore>();
                            while (dr.Read())
                            {
                                string activitiesOID = null;
                                string activityName = null;
                                double unitCost = -1;
                                string unitCostUnit = null;
                                
                                if (dr["ActivitiesOID"] != DBNull.Value) activitiesOID = dr["ActivitiesOID"].ToString();
                                if (dr["Activity"] != DBNull.Value) activityName = dr["Activity"].ToString();
                                if (dr["UnitCost"] != DBNull.Value) unitCost = Convert.ToDouble(dr["UnitCost"]);
                                if (dr["UnitCost_unit"] != DBNull.Value) unitCostUnit = dr["UnitCost_unit"].ToString();
                                OMSActivityStore activity = new OMSActivityStore(activitiesOID,activityName,unitCost,unitCostUnit);
                                activities.Add(activity);

                                FillOMSConsequences(impactTable, condCategoriesTable,activity);
                            }
                        }
                        catch (Exception e)
                        {
                            DataAccessExceptionHandler.HandleException(e, false);
                            activities = null;
                        }
                    }
                }
            }
            return activities;
        }

        /// <summary>
        /// Returns the Cartegraph object name of the asset to build activities with.
        /// </summary>
        /// <param name="OID">assetTypesOID from AssetTypes table</param>
        /// <returns></returns>
        public static string GetObjectUserID(string OID)
        {
            string objectUserID = null;

            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ObjectUserID FROM cgSysObjects WHERE OID=@OID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("OID",OID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["ObjectUserID"] != DBNull.Value)
                        {
                            objectUserID = dr["ObjectUserID"].ToString();
                        }
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                    objectUserID = null;
                }
            }
            return objectUserID;
        }

        /// <summary>
        /// Complicated way to get Impact table.  Much easier just to do a replace against the xxxxConditionCategories to get Impact table. 
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="condCategoryTable"></param>
        /// <returns></returns>
        private static string GetImpactTable(string assetName, string condCategoryTable)
        {
            string impactTable = condCategoryTable.Replace("ConditionCategory", "Impacts");

            //string impactTable = null;
            //List<string> listImpactParentID = new List<string>();
            ////Find all 'cgImpacts' -> Get parentID -> Get xxxxCondCondition for asset in question.
            //using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            //{
            //    try
            //    {
            //        connection.Open();
            //        string query = "SELECT parentID  FROM cgSysObjects WHERE ObjectName='Impacts' AND ObjectUserID='cgImpacts'";
            //        SqlCommand cmd = new SqlCommand(query, connection);
            //        SqlDataReader dr = cmd.ExecuteReader();
            //        while (dr.Read())
            //        {
            //            listImpactParentID.Add(dr["parentID"].ToString());
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        DataAccessExceptionHandler.HandleException(e, false);
            //        listImpactParentID = null;
            //    }
            //}
            //foreach (string parentID in listImpactParentID)
            //{

            //    using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            //    {
            //        try
            //        {
            //            connection.Open();
            //            string query = "SELECT TableName FROM cgSysObjects obj inner join cgSysFields fld on obj.OID= fld.oid WHERE ParentID=@parentID AND ObjectName = 'Condition Category'";
            //            SqlCommand cmd = new SqlCommand(query, connection);
            //            cmd.Parameters.Add(new SqlParameter("parentID", parentID));
            //            SqlDataReader dr = cmd.ExecuteReader();
            //            if(dr.Read())
            //            {
            //                condCategoryTable = dr["TableName"].ToString();
            //                if (condCategoryTable == oci.OCITable)
            //                {
            //                    break;
            //                }
            //            }
            //            condCategoryTable = null;
            //        }
            //        catch (Exception e)
            //        {
            //            DataAccessExceptionHandler.HandleException(e, false);
            //            continue;
            //        }
            //    }
            //}

            //At this point we have the xxxxCondCategories.  We can use the Foreign Key to look up the corresponding Impact table

            //if(condCategoryTable != null)
            //{

            //    using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            //    {
            //        try
            //        {
            //            connection.Open();
            //            string query = "SELECT OBJECT_NAME(f.parent_object_id) AS TableName FROM sys.foreign_keys AS f INNER JOIN sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id "
            //                        + " WHERE OBJECT_NAME (f.referenced_object_id)=@conditionTable AND OBJECT_NAME(f.parent_object_id) like '%Impacts%'";
            //            SqlCommand cmd = new SqlCommand(query, connection);
            //            cmd.Parameters.Add(new SqlParameter("conditionTable", condCategoryTable));
            //            SqlDataReader dr = cmd.ExecuteReader();
            //            if (dr.Read())
            //            {
            //                impactTable = dr["TableName"].ToString();
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            DataAccessExceptionHandler.HandleException(e, false);
            //            impactTable = null;
            //        }
            //    }
            //}
            return impactTable;
        }

        /// <summary>
        /// Add all of the Impacts/Consequences for a given activity.
        /// </summary>
        /// <param name="impactTable">Table from which to read impacts</param>
        /// <param name="conditionTable">Table from which to read ConditionCategories</param>
        /// <param name="activity">Activity to which to add the selected consequences.</param>
        private static void FillOMSConsequences(string impactTable, string conditionTable, OMSActivityStore activity)
        {
            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT ConditionCategory, Impact, Impact_unit FROM " + impactTable + " imp inner join " + conditionTable + " con on imp." + conditionTable + "OID=con." + conditionTable + "OID  WHERE Activity=@activityName AND con.Inactive=0";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("activityName",activity.ActivityName));
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string impact = null;
                        string impactUnit = null;
                        string conditionCategory = null;

                        if (dr["ConditionCategory"] != DBNull.Value) conditionCategory = dr["ConditionCategory"].ToString();
                        if (dr["Impact"] != DBNull.Value) impact = dr["Impact"].ToString();
                        if (dr["Impact_unit"] != DBNull.Value) impactUnit = dr["Impact_unit"].ToString();

                        OMSConsequenceStore consequence = new OMSConsequenceStore(conditionCategory, impact, impactUnit);
                        activity.ConsequenceList.Add(consequence);
                    }
                }
                catch (Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        /// <summary>
        /// Retrieves the information to calculate OCI.  Retrieves from cache if available.
        /// </summary>
        /// <param name="assetName">Name of the asset</param>
        /// <returns></returns>
        public static OMSAssetConditionIndexStore GetAssetConditionIndex(string assetName)
        {
            OMSAssetConditionIndexStore oci = null;
            if (_conditionIndexes == null)
            {
                if (_assetAttributeDictionary == null) GetAssetAttributes(assetName);
            }
            else
            {
                if (!_assetAttributeDictionary.ContainsKey(assetName)) GetAssetAttributes(assetName);
            }

            FillAssetConditionIndexes(assetName);
            oci = _conditionIndexes.Find(delegate(OMSAssetConditionIndexStore o) { return o.AssetName == assetName; });                
            return oci;
        }

        /// <summary>
        /// Fills the AssetConditionIndexes, which stores the information necessary to calculate OCI
        /// </summary>
        private static void FillAssetConditionIndexes(string assetName)
        {
            if(_conditionIndexes == null) _conditionIndexes = new List<OMSAssetConditionIndexStore>();
            OMSAssetConditionIndexStore assetConditionIndex = _conditionIndexes.Find(delegate(OMSAssetConditionIndexStore o) { return o.AssetName == assetName; });
            if(assetConditionIndex == null)
            {
                List<AttributeStore> attributes = _assetAttributeDictionary[assetName];
                AttributeStore conditionCategory = attributes.Find(delegate(AttributeStore attribute) { return attribute.OmsHierarchy == "Inspections|Condition Categories|Condition Category"; });
                List<OCIWeightStore> weights = GetOCIWeighting(assetName);
                
                if (conditionCategory != null && weights != null)
                {
                    
                    OMSAssetConditionIndexStore assetOCI = new OMSAssetConditionIndexStore(conditionCategory,weights);
                    if (assetOCI != null)
                    {
                        _conditionIndexes.Add(assetOCI);

                        AttributeStore ociAttribute = new AttributeStore(assetName, "OverallConditionIndex", "Overall Condition Index", null);
                        attributes.Add(ociAttribute);
                        
                        foreach (OMSConditionIndexStore ci in assetOCI.ConditionIndexes)
                        {
                            AttributeStore ciAttribute = new AttributeStore(assetName, ci.AttributeDE, ci.ConditionCategory, conditionCategory.OmsTable);
                            attributes.Add(ciAttribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get's the OCI weighting (and an object to calculate OCI for an asset) from OMS
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public static List<OCIWeightStore> GetOCIWeighting(string assetName)
        {
            List<AttributeStore> attributes = GetAssetAttributes(assetName);
            AttributeStore attributeOCI = attributes.Find(delegate(AttributeStore attribute) { return attribute.OmsHierarchy == "Inspections|Condition Categories|Condition Category"; });
            List<OCIWeightStore> weights = null;
            if (attributeOCI != null)
            {

                string table = null;
                string column = null;
                OMS.GetLookupTableColumn(attributeOCI.LookupOID, out table, out column);
                if (table != null)
                {
                    using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = "SELECT " + attributeOCI.AttributeField + ",WEIGHT,FILTERTEXT FROM " + table + " WHERE INACTIVE<>'1'";
                            SqlCommand cmd = new SqlCommand(query, connection);
                            SqlDataReader dr = cmd.ExecuteReader();
                            weights = new List<OCIWeightStore>();
                            while (dr.Read())
                            {
                                string category = dr[attributeOCI.AttributeField].ToString();
                                int weight = Convert.ToInt32(dr["WEIGHT"]);
                                string filterText = null;
                                if(dr["FILTERTEXT"] != DBNull.Value)
                                {
                                    filterText = dr["FILTERTEXT"].ToString();
                                }
                                weights.Add(new OCIWeightStore(category, weight,filterText));
                            }
                        }
                        catch (Exception e)
                        {
                            DataAccessExceptionHandler.HandleException(e, false);
                            weights = null;
                        }
                    }
                }
            }
            return weights;
        }

        /// <summary>
        /// Retrieves the Prediction groups for loading performance curves
        /// </summary>
        /// <param name="assetName">Asset name (Sign, Pavement, etc) to load prediction goups for.</param>
        /// <returns>List of Performance curves</returns>
        public static List<OMSPrediction> GetPredictionsCurves(string assetName)
        {
            List<OMSPrediction> predictions = null;
            OMSAssetConditionIndexStore assetOCI = GetAssetConditionIndex(assetName);
            if (assetOCI != null)
            {
                string predictionGroups = assetOCI.ConditionCategoryTable.Replace("ConditionCategories", "PredictionGroups");
                string predictionPoints = assetOCI.ConditionCategoryTable.Replace("ConditionCategories", "PredictionPoints");

                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    try
                    {
                        connection.Open();
                        string oidColumn = predictionGroups + "OID";
                        string query = "SELECT " + oidColumn + ",PredictionGroup,Filter FROM " + predictionGroups;
                        SqlCommand cmd = new SqlCommand(query, connection);
                        SqlDataReader dr = cmd.ExecuteReader();
                        predictions = new List<OMSPrediction>();
                        while (dr.Read())
                        {
                            string oid = dr[oidColumn].ToString();
                            string predictionGroup = dr["PredictionGroup"].ToString();
                            string filter = dr["Filter"].ToString();
                            OMSPrediction prediction = new OMSPrediction(assetName, predictionGroup, filter, oid, predictionGroups);
                            FillPredictionPoints(prediction);
                            predictions.Add(prediction);
                            
                        }
                    }
                    catch (Exception e)
                    {
                        DataAccessExceptionHandler.HandleException(e, false);

                    }
                }
            }
            return predictions;
        }

        /// <summary>
        /// Fills the prediction curves for each Prediction Group
        /// </summary>
        /// <param name="prediction">Each individual prediction curve in prediction group</param>
        private static void FillPredictionPoints(OMSPrediction prediction)
        {
            string predictionGroupsOID = prediction.PredictionGroupTable + "OID";
            string predictionPointsTable = prediction.PredictionGroupTable.Replace("PredictionGroups","PredictionPoints");


            using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT IndexValue,ConditionCategory,PredictionYear FROM " + predictionPointsTable + " WHERE " + predictionGroupsOID + "=@OID ORDER BY ConditionCategory,PredictionYear";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("OID",prediction.PredictionGroupsOID));
                    SqlDataReader dr = cmd.ExecuteReader();
                    OMSCategoryPrediction categoryPrediction = null;
                    string previousCategoryPrediction = "";
                    while (dr.Read())
                    {
                        double indexValue = double.NaN;
                        double predictionYear = double.NaN;
                        string conditionCategory = dr["ConditionCategory"].ToString();
                        if (dr["IndexValue"] != DBNull.Value) indexValue = Convert.ToDouble(dr["IndexValue"]);
                        if (dr["PredictionYear"] != DBNull.Value) predictionYear = Convert.ToDouble(dr["PredictionYear"]);

                        if (previousCategoryPrediction != conditionCategory)
                        {
                            categoryPrediction = new OMSCategoryPrediction(conditionCategory);
                            prediction.CategoryPredictions.Add(categoryPrediction);
                            previousCategoryPrediction = conditionCategory;
                        }
                        categoryPrediction.Points.Add(new OMSPredictionPoints(conditionCategory, predictionYear, indexValue));
                    }
                }
                catch(Exception e)
                {
                    DataAccessExceptionHandler.HandleException(e, false);
                }
            }
        }

        public static List<AssetReplyOMSDataStore> GetAssetData(AssetRequestOMSDataStore request)
        {
            List<AssetReplyOMSDataStore> assetReplies = new List<AssetReplyOMSDataStore>();
            foreach(AssetRequestOMSTableStore omsTable in request.Tables)
            {
                if (omsTable.PrimaryKeyColumn != null) continue;
                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    connection.Open();
                    string query = BuildAssetRequestQuery(omsTable,null);
                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();

                    string oidPrevious = "";
                    int indexPreviousOID = 0;
                    DateTime datePrevious = DateTime.MinValue;
                    AssetReplyOMSTableStore replyTable = null;
                    while (dr.Read())
                    {
                        DateTime dateEntry = DateTime.MinValue;
                        string oid = dr[1].ToString();//The OID is always second return for BuildAssetQuery
                        if (dr[0] != DBNull.Value)//Date always first return for BuildAssetQuery
                        {
                            string dateString = dr[0].ToString();
                            dateEntry = Convert.ToDateTime(dateString);
                        }
                        AssetReplyOMSDataStore replyData = null;
                        
                        if (oid != oidPrevious)
                        {
                            //Delegate find is inefficient.  Since the OIDs are in order, and each should be the next in the list, try that first.
                            string lookupOID = "";
                            if (indexPreviousOID < assetReplies.Count)
                            {
                                lookupOID = assetReplies[indexPreviousOID].OID;
                            }

                            // 
                            if (string.IsNullOrWhiteSpace(lookupOID) || lookupOID != oid)
                            {
                                int indexCurrentOID = assetReplies.FindIndex(delegate(AssetReplyOMSDataStore arods) { return arods.OID == oid; });
                                if (indexCurrentOID >= 0)
                                {
                                    replyData = assetReplies[indexCurrentOID];
                                    indexPreviousOID = indexCurrentOID;
                                }
                            }
                            else
                            {
                                replyData = assetReplies[indexPreviousOID];
                            }


                            if (replyData == null)
                            {
                                replyData = new AssetReplyOMSDataStore(oid);
                                assetReplies.Add(replyData);
                            }
                            replyTable = new AssetReplyOMSTableStore(omsTable.TableName,dateEntry);

                            foreach (AssetRequestOMSColumnStore column in omsTable.Columns)
                            {
                                replyTable.Columns.Add(new AssetReplyOMSColumnStore(column.OmsObjectUserIDHierarchy,null,column.AttributeField));
                            }
                            replyData.Tables.Add(replyTable);
                            datePrevious = DateTime.MinValue;
                            oidPrevious = oid;
                            indexPreviousOID++;
                        }


                        if (dateEntry > datePrevious && dateEntry <= request.BeforeDate)
                        {
                            replyTable.DateLastEntry = dateEntry;
                            foreach (AssetRequestOMSColumnStore column in omsTable.Columns)
                            {
                                string columnName = column.ColumnName;
                                string value = null;
                                if (dr[columnName] != DBNull.Value)
                                {
                                    value = dr[columnName].ToString();
                                    AssetReplyOMSColumnStore replyColumn = replyTable.Columns.Find(delegate(AssetReplyOMSColumnStore arocs) { return arocs.OmsObjectUserIDHierarchy == column.OmsObjectUserIDHierarchy; });
                                    if (replyColumn != null)
                                    {
                                        replyColumn.Value = value.ToString();                                    
                                    }
                                    else // The null column is key lookup column which is not in the hierarchy.
                                    {
                                        replyColumn = replyTable.Columns.Find(delegate(AssetReplyOMSColumnStore arocs) { return arocs.OmsObjectUserIDHierarchy == null; });
                                        if (replyColumn != null)
                                        {
                                            replyColumn.Value = value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            FillAssetReplyConditionIndex(request, assetReplies);
            return assetReplies;
        }



        public static Dictionary<string, AssetReplyOMSLookupTable> GetAssetLookupData(AssetRequestOMSDataStore request)
        {
            Dictionary<string, AssetReplyOMSLookupTable> assetLookupTables = new Dictionary<string, AssetReplyOMSLookupTable>();
            foreach (AssetRequestOMSTableStore omsTable in request.Tables)
            {
                if(omsTable.PrimaryKeyColumn != null)
                {
                    AssetReplyOMSLookupTable lookupTable = new AssetReplyOMSLookupTable(omsTable.TableName);
                    assetLookupTables.Add(omsTable.TableName, lookupTable);

                    using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                    {
                        connection.Open();
                        string query = BuildAssetRequestQuery(omsTable, null);
                        SqlCommand cmd = new SqlCommand(query, connection);
                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            string key = null;
                            Dictionary<string,string> lookupValues = new Dictionary<string,string>();
                            foreach (AssetRequestOMSColumnStore column in omsTable.Columns)
                            {
                                if(column.ColumnName == omsTable.PrimaryKeyColumn)
                                {
                                    key = dr[column.ColumnName].ToString();// Lookup Keys are never null.
                                }    
                                else
                                {
                                    string value = null;
                                    if (dr[column.ColumnName] != DBNull.Value) value =  dr[column.ColumnName].ToString();
                                    lookupValues.Add(column.ColumnName.ToString(), value);                                    
                                }
                            }
                            lookupTable.LookupData.Add(key, lookupValues);
                        }
                    }
                }
            }
            return assetLookupTables;
        }








        /// <summary>
        /// Fills the ConditionIndices for calculating OCI.
        /// </summary>
        /// <param name="request">The asset request which contains the OCI calculatoins</param>
        /// <param name="assetReplies">List of other assets</param>
        public static void FillAssetReplyConditionIndex(AssetRequestOMSDataStore request, List<AssetReplyOMSDataStore> assetReplies)
        {
            foreach (OMSConditionIndexStore conditionIndex in request.ConditionIndex.ConditionIndexes)
            {

                AssetRequestOMSTableStore omsTable = new AssetRequestOMSTableStore(request.ConditionIndex.ConditionCategoryTable);
                omsTable.Columns.Add(new AssetRequestOMSColumnStore("IndexValue", conditionIndex.ConditionCategory));

                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    connection.Open();
                    string query = BuildAssetRequestQuery(omsTable, conditionIndex);


                    SqlCommand cmd = new SqlCommand(query, connection);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        DateTime dateEntry = DateTime.MinValue;
                        string oid = dr[1].ToString();//The OID is always second return for BuildAssetQuery
                        if (dr[0] != DBNull.Value)//Date always first return for BuildAssetQuery
                        {
                            string dateString = dr[0].ToString();
                            dateEntry = Convert.ToDateTime(dateString);
                        }
                        string value = null;
                        if (dr["IndexValue"] != DBNull.Value)
                        {
                            value = dr["IndexValue"].ToString();
                        }
                        // Add to reply

                        string oidPrevious = "";
                        int indexPreviousOID = 0;
                        DateTime datePrevious = DateTime.MinValue;
                        AssetReplyOMSDataStore replyData = null;

                        if (oid != oidPrevious)
                        {
                            //Delegate find is inefficient.  Since the OIDs are in order, and each should be the next in the list, try that first.
                            string lookupOID = "";
                            if (indexPreviousOID < assetReplies.Count)
                            {
                                lookupOID = assetReplies[indexPreviousOID].OID;
                            }

                            if (string.IsNullOrWhiteSpace(lookupOID) || lookupOID != oid)
                            {
                                int indexCurrentOID = assetReplies.FindIndex(delegate(AssetReplyOMSDataStore arods) { return arods.OID == oid; });
                                if (indexCurrentOID >= 0)
                                {
                                    replyData = assetReplies[indexCurrentOID];
                                    indexPreviousOID = indexCurrentOID;
                                }
                            }
                            else
                            {
                                replyData = assetReplies[indexPreviousOID];
                            }


                            if (replyData == null)
                            {
                                replyData = new AssetReplyOMSDataStore(oid);
                                assetReplies.Add(replyData);
                            }

                            if (replyData.ConditionIndices == null)
                            {
                                replyData.ConditionIndices = new List<AssetReplyOMSConditionIndex>();
                            }

                            AssetReplyOMSConditionIndex replyCI = replyData.ConditionIndices.Find(delegate(AssetReplyOMSConditionIndex ci) { return ci.ConditionIndex == conditionIndex.ConditionCategory; });
                            if (replyCI == null)
                            {
                                if (value != null)
                                {
                                    replyCI = new AssetReplyOMSConditionIndex(conditionIndex.ConditionCategory, double.Parse(value), dateEntry);
                                    replyData.ConditionIndices.Add(replyCI);
                                }
                            }
                            else
                            {
                                if (value != null)
                                {
                                    replyCI.Update(double.Parse(value), dateEntry);
                                }
                            }
                            oidPrevious = oid;
                            indexPreviousOID++;
                        }
                    }
                }
            }


            //JWC_PROJECT
            foreach(AssetReplyOMSDataStore assetReply in assetReplies)
            {
              
                if (assetReply.ConditionIndices == null)
                {

                    DateTime dateInstalledReplaced = DateTime.MinValue;
                    foreach (AssetReplyOMSTableStore table in assetReply.Tables)
                    {
                        if (table.OmsTable.Contains("MainGeneral"))
                        {
                            foreach (AssetReplyOMSColumnStore column in table.Columns)
                            {
                                if (column.AttributeField == "Installed" || column.AttributeField == "Replaced")
                                {
                                    if (column.Value != null)
                                    {
                                        DateTime date = Convert.ToDateTime(column.Value);
                                        if (date > dateInstalledReplaced)
                                        {
                                            dateInstalledReplaced = date;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (dateInstalledReplaced > DateTime.MinValue)
                    {
                        assetReply.ConditionIndices = new List<AssetReplyOMSConditionIndex>();
                        foreach (OMSConditionIndexStore conditionIndex in request.ConditionIndex.ConditionIndexes)
                        {
                            AssetReplyOMSConditionIndex replyCI = new AssetReplyOMSConditionIndex(conditionIndex.ConditionCategory, 100, dateInstalledReplaced);
                            assetReply.ConditionIndices.Add(replyCI);
                        }
                    }
                }
            }

        }





        /// <summary>
        /// Build the AssetQuery
        /// </summary>
        /// <param name="omsTable"></param>
        /// <returns></returns>
        private static string BuildAssetRequestQuery(AssetRequestOMSTableStore omsTable, OMSConditionIndexStore conditionIndex)
        {
            string query = null;
            if (omsTable.Columns.Count > 0)
            {
                List<Constraint> primaryConstraints = GetOMSPrimaryKeyConstraints(omsTable.TableName);
                List<Constraint> foreignConstraints = GetOMSForeignKeyConstraints(omsTable.TableName);;
                if (primaryConstraints.Count > 0)
                {
                    string dateColumn = null;
                    Constraint inspectionConstraint = primaryConstraints.Find(delegate(Constraint c) { return c.constraint_keys.Contains("InspectionsOID"); });
                    //This determines if query uses inspection date or construction date.
                    if (omsTable.Columns[0].OmsObjectUserIDHierarchy.Contains("cgInspections") && inspectionConstraint != null)
                    {
                        dateColumn = inspectionConstraint.references + ".InspectionDate";
                    }
                    else if (omsTable.Columns[0].OmsObjectUserIDHierarchy.Contains("cgInspections") && inspectionConstraint == null)
                    {
                        dateColumn = omsTable.TableName + ".InspectionDate";
                    }
                    else if (conditionIndex != null)
                    {

                        dateColumn = primaryConstraints[1].references + ".InspectionDate";
                    }
                    else // Use entry date
                    {
                        dateColumn = omsTable.TableName + "." + "EntryDate";
                    }

                    query = "SELECT " + dateColumn + "," + primaryConstraints[0].references + "." + primaryConstraints[0].constraint_keys;
                    string columns = "";



                    //Add lookup key so can match with main general OID
                    if (omsTable.PrimaryKeyColumn != null)// has not yet been added
                    {
                        omsTable.Columns.Add(new AssetRequestOMSColumnStore(omsTable.PrimaryKeyColumn, null));
                    }

                    foreach (AssetRequestOMSColumnStore omsColumn in omsTable.Columns)
                    {
                        columns += ',';
                        columns += omsTable.TableName + "." + omsColumn.ColumnName;
                    }

                    string fromWhere = GetOMSFromWhere(foreignConstraints, primaryConstraints);
                    query += columns;
                    query += fromWhere;
                    if (conditionIndex != null)
                    {
                        query += " AND " + omsTable.TableName + ".ConditionCategory = '" + conditionIndex.ConditionCategory + "'";
                    }
                    query += " ORDER BY " + primaryConstraints[0].references + "." + primaryConstraints[0].constraint_keys + "," + dateColumn;
                }
            }
            return query;
        }





        /// <summary>
        /// Get asset OID (constraints[0]) and all foregin keys (any additional constraints)
        /// </summary>
        /// <param name="omsTableName">omsTable name recursive</param>
        /// <returns></returns>
        private static List<Constraint> GetOMSForeignKeyConstraints(string omsTableName)
        {
            List<Constraint> constraints = null;
            SPHelp tableInfo = new SPHelp(omsTableName, DB.OMSConnectionString);
           
            Constraint foreignKey = tableInfo.Constraints.Find(delegate (Constraint c){ return c.constraint_type == "FOREIGN KEY";});
            Constraint primaryKey = tableInfo.Constraints.Find(delegate(Constraint c) { return c.constraint_type.Contains("PRIMARY"); });
            if(foreignKey == null)
            {
                constraints = new List<Constraint>();
                primaryKey.references = omsTableName;
                constraints.Add(primaryKey);
            }
            else
            {
                constraints = GetOMSForeignKeyConstraints(foreignKey.references);
                foreignKey.references = omsTableName;
                constraints.Add(foreignKey);
            }
 
            return constraints;
        }

        /// <summary>
        /// Get asset OID (constraints[0]) and all foregin keys (any additional constraints)
        /// </summary>
        /// <param name="omsTableName">omsTable name recursive</param>
        /// <returns></returns>
        private static List<Constraint> GetOMSPrimaryKeyConstraints(string omsTableName)
        {
            List<Constraint> constraints = null;
            SPHelp tableInfo = new SPHelp(omsTableName, DB.OMSConnectionString);

            Constraint foreignKey = tableInfo.Constraints.Find(delegate(Constraint c) { return c.constraint_type == "FOREIGN KEY"; });
            Constraint primaryKey = tableInfo.Constraints.Find(delegate(Constraint c) { return c.constraint_type.Contains("PRIMARY"); });
            if (foreignKey == null)
            {
                constraints = new List<Constraint>();
            }
            else
            {
                constraints = GetOMSPrimaryKeyConstraints(foreignKey.references);
            }
            primaryKey.references = omsTableName;
            constraints.Add(primaryKey);

            return constraints;
        }

        /// <summary>
        /// Builds the OMS WHERE clause for extraction of asset data.
        /// </summary>
        /// <param name="foreignConstraints">Primary and foreign key constraints</param>
        /// <returns></returns>
        private static string GetOMSFromWhere(List<Constraint> foreignConstraints, List<Constraint> primaryConstraint)
        {
            string fromWhere = "";
            for (int i = foreignConstraints.Count - 1; i >= 0; i--)
            {
                if(string.IsNullOrWhiteSpace(fromWhere))
                {
                    fromWhere += " FROM ";
                }
                else
                {
                    fromWhere  += ",";
                }

                fromWhere += foreignConstraints[i].references;
            }

            if (foreignConstraints.Count > 1)
            {
                fromWhere += " WHERE ";
                bool firstClause = true;
                for (int i = 0; i < foreignConstraints.Count - 1; i++)
                {
                    if (!firstClause) fromWhere += " AND ";

                    fromWhere += primaryConstraint[i].references + "." + primaryConstraint[i].constraint_keys + "=" + foreignConstraints[i + 1].references + "." + foreignConstraints[i + 1].constraint_keys;
                    firstClause = false;
                }
            }
            return fromWhere;
        }

         /// <summary>
        /// Searches through a Query or Equation and extracts all of the database attributes encased in square brackets [attribute].
        /// </summary>
        /// <param name="strQuery">Query or Equation to parse.</param>
        /// <returns>List of variables in global variable.</returns>
        public static List<AttributeStore> ParseAttributes(string assetName, string criteriaEquation)
        {
            List<AttributeStore> attributes = new List<AttributeStore>();
            List<AttributeStore> attributeAsset = OMS.GetAssetAttributes(assetName);

            if (!string.IsNullOrWhiteSpace(assetName) && !string.IsNullOrWhiteSpace(criteriaEquation))
            {
                string attributeString;
                int nOpen = -1;
                for (int i = 0; i < criteriaEquation.Length; i++)
                {
                    if (criteriaEquation.Substring(i, 1) == "[")
                    {
                        nOpen = i;
                        continue;
                    }

                    if (criteriaEquation.Substring(i, 1) == "]" && nOpen > -1)
                    {
                        //Get the value between open and (i)
                        attributeString = criteriaEquation.Substring(nOpen + 1, i - nOpen - 1);
                        AttributeStore attribute = attributeAsset.Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == attributeString.Replace("\\","|"); });

                        if (attribute != null && !attributes.Contains(attribute))
                        {
                            attributes.Add(attribute);
                        }
                    }
                }
            }


            List<AttributeStore> additionalAttributes = new List<AttributeStore>();
            foreach(AttributeStore attribute in attributes)
            {
                if(attribute.OmsObjectUserIDHierarchy.Contains('|'))
                {
                    string[] parsedTables = attribute.OmsObjectUserIDHierarchy.Split('|');

                    string attributeTree = "";
                    for (int i = 0; i < parsedTables.Length - 1; i++)
                    {
                        if (attributeTree.Length > 0) attributeTree += "|";
                        attributeTree += parsedTables[i];
                        AttributeStore attributePartial = attributeAsset.Find(delegate(AttributeStore a) { return a.OmsObjectUserIDHierarchy == attributeTree; });
                        if (attributePartial != null && !additionalAttributes.Contains(attributePartial))
                        {
                            additionalAttributes.Add(attributePartial);

                            string lookupOID = attributePartial.LookupOID;
                            AttributeStore attributeLookup = attributeAsset.Find(delegate(AttributeStore a) { return a.OID== lookupOID; });

                        }
                    }
                }
            }

            foreach(AttributeStore attribute in additionalAttributes)
            {
                if (!attributes.Contains(attribute))
                {
                    attributes.Add(attribute);
                }
            }


            return attributes;
        }



        private static void DeleteExpiredWorkPlanSessions()
        {
            _sessionOIDs.RemoveAll(delegate(SessionStore s) { return s.SessionDate.AddHours(1) < DateTime.Now; });
        }

        public static void DeleteWorkPlanSessionByID(string sessionID)
        {
            _sessionOIDs.RemoveAll(delegate(SessionStore s) { return s.SessionID == sessionID; });
        }

        public static void DeleteWorkPlanByUser(string userName)
        {
            _sessionOIDs.RemoveAll(delegate(SessionStore s) { return s.UserName == userName; });
        }

        public static bool IsSessionValid(string sessionID)
        {
            SessionStore sessionOID = _sessionOIDs.Find(delegate(SessionStore s) { return s.SessionID == sessionID; });
            if (sessionOID == null) return false;
            //If we still have the SessionID, update the current session date/time.
            sessionOID.SessionDate = DateTime.Now;
            return true;
        }

        public static SessionStore GetWorkPlanSessionFromSessionID(string sessionID)
        {
            SessionStore sessionOID = _sessionOIDs.Find(delegate(SessionStore s) { return s.SessionID == sessionID; });
            if (sessionOID != null)
            {
                sessionOID.SessionDate = DateTime.Now; //Update 
            }
            return sessionOID;
        }

        public static WorkPlanStore GetWorkPlanSession(string simulationID,string userName, string orderByField)
        {
            SessionStore sessionOID = new SessionStore();
            sessionOID.SessionDate = DateTime.Now;
            sessionOID.UserName = userName;
            sessionOID.SimulationID = simulationID;
            sessionOID.OrderByField = orderByField;
            sessionOID.SessionID= userName + "_" + simulationID + "_" + sessionOID.SessionDate.Ticks;
            sessionOID.OIDs = SelectScenario.GetResultOIDs(simulationID, orderByField);
 
            sessionOID.Simulation =  SelectScenario.GetSimulationStore(simulationID);
            sessionOID.TreatmentsPerYear = SelectScenario.GetTreatmentsPerYear(simulationID);
            sessionOID.Activities = SelectScenario.GetActivities(simulationID,false);
            sessionOID.OIDResults = SelectScenario.GetSimulationResults(simulationID);
            WorkPlanFilter filter = new WorkPlanFilter();
            if (sessionOID.Simulation != null)
            {
                _sessionOIDs.Add(sessionOID);
                SelectScenario.GetActivityResults(sessionOID.SessionID,filter);
                int numberYear = sessionOID.Simulation.Years;
                int startYear = sessionOID.Simulation.StartDate.Year;
                for (int i = 0; i < sessionOID.OIDs.Count; i++)
                {
                    string oid = sessionOID.OIDs[i];
                    byte[] yearOCI = new byte[numberYear];
                    for (int year = startYear; year < startYear + numberYear; year++)
                    {
                        string overallConditionIndex = sessionOID.OIDResults[oid]["OverallConditionIndex"][year.ToString()];
                        int oci = (int) Math.Round(Convert.ToDouble(overallConditionIndex));
                        yearOCI[year - startYear] = (byte)oci; 
                    }
                    string oci64 = Convert.ToBase64String(yearOCI);
                    sessionOID.EncodedActivities[i] = oci64 + ":" + sessionOID.EncodedActivities[i];
                }
            }
            else
            {
                sessionOID = null;
            }

            WorkPlanStore workPlan = null;
            if(sessionOID != null)
            {
                workPlan = new WorkPlanStore(sessionOID);
            }

            List<TargetStore> targets = SelectScenario.GetTargetsForCopy(simulationID);
            List<TargetResultStore> results = SelectScenario.GetYearlyTargets(simulationID);
            foreach (WorkPlanYearStore year in workPlan.Years)
            {
                TargetStore target = targets.Find(delegate(TargetStore t) { return t.Year == year.Year.ToString(); });
                if (target != null)
                {
                    year.TargetOCI = target.TargetOCI.ToString();
                }
                
                
                TargetResultStore result = results.Find(delegate(TargetResultStore t) { return t.Years == year.Year.ToString(); });
                if (result != null)
                {
                    year.CurrentOCI = result.TargetMet.ToString("f2");
                }
                
            }
            return workPlan;
        }



        public static WorkPlanStore GetUpdatedWorkPlan(string sessionID)
        {
            SessionStore session = GetWorkPlanSessionFromSessionID(sessionID);
            session.Simulation = SelectScenario.GetSimulationStore(session.SimulationID);
            session.TreatmentsPerYear = SelectScenario.GetTreatmentsPerYear(session.SimulationID);
            
            WorkPlanStore workPlan = null;
            if (session != null)
            {
                workPlan = new WorkPlanStore(session);
            }
            List<TargetStore> targets = SelectScenario.GetTargetsForCopy(session.SimulationID);
            List<TargetResultStore> results = SelectScenario.GetYearlyTargets(session.SimulationID);
            foreach (WorkPlanYearStore year in workPlan.Years)
            {
                TargetStore target = targets.Find(delegate(TargetStore t) { return t.Year == year.Year.ToString(); });
                if (target != null)
                {
                    year.TargetOCI = target.TargetOCI.ToString();
                }


                TargetResultStore result = results.Find(delegate(TargetResultStore t) { return t.Years == year.Year.ToString(); });
                if (result != null)
                {
                    year.CurrentOCI = result.TargetMet.ToString("f2");
                }

            }
            return workPlan;
        }



        public static List<YearBudgetStore> GetYearlyBudget(string sessionID)
        {
            SessionStore session = GetWorkPlanSessionFromSessionID(sessionID);
            if (session != null)
            {
                return session.Simulation.YearBudgets;
            }
            else
            {
                return null;
            }
        }

        public static List<string> GetTreatmentsPerYear(string sessionID)
        {
            SessionStore session = GetWorkPlanSessionFromSessionID(sessionID);
            if (session != null)
            {
                return session.TreatmentsPerYear;
            }
            else
            {
                return null;
            }
        }


        public static List<string> GetWorkPlanActivities(string sessionID)
        {
            SessionStore session = GetWorkPlanSessionFromSessionID(sessionID);
            if (session != null)
            {
                List<string> listActivity = new List<string>();
                foreach (ActivityStore activity in session.Activities)
                {
                    listActivity.Add(activity.Activity);
                }
                return listActivity;
            }
            else
            {
                return null;
            }
        }


        public static List<string> GetWorkPlanBudgets(string sessionID)
        {
            SessionStore session = GetWorkPlanSessionFromSessionID(sessionID);
            if (session != null)
            {
                List<string> listBudget = new List<string>();
                listBudget.Add("");//Add blank budget for No Treatment
                foreach (CategoryBudgetStore budget in session.Simulation.CategoryBudgets)
                {
                    listBudget.Add(budget.Key);
                }
                return listBudget;
            }
            else
            {
                return null;
            }
        }

        public static string GetEncodedAppliedActivities(string sessionID, int beginRow, int endRow)
        {
            string encodedActivities = "";
            SessionStore session;
            try
            {
                 session = GetWorkPlanSessionFromSessionID(sessionID);
            }
            catch (Exception e)
            {
                return "Error: Invalid or missing work plan for input sessionID. " + e.Message;
            }

            if (session != null)
            {
                if (beginRow < 0) beginRow = 0;
                if (endRow >= session.EncodedActivities.Count) endRow = session.EncodedActivities.Count - 1;
                if (endRow < 0) return null;
                
                for(int i = beginRow; i <= endRow; i++)
                {
                    if (encodedActivities.Length > 0) encodedActivities += "|";
                    encodedActivities += session.EncodedActivities[i];
                }
            }
            return encodedActivities;
        }

        public static List<string> GetEncodedAppliedActivities(string sessionID, WorkPlanFilter filter)
        {
            List<string> encodedActivities = new List<string>();
            SessionStore session;
            try
            {
                session = GetWorkPlanSessionFromSessionID(sessionID);
            }
            catch (Exception e)
            {
                return null;
            }



            SelectScenario.GetActivityResults(session.SessionID,filter);
            int numberYear = session.Simulation.Years;
            int startYear = session.Simulation.StartDate.Year;
            for (int i = 0; i < session.OIDs.Count; i++)
            {
                string oid = session.OIDs[i];
                byte[] yearOCI = new byte[numberYear];
                for (int year = startYear; year < startYear + numberYear; year++)
                {
                    string overallConditionIndex = session.OIDResults[oid]["OverallConditionIndex"][year.ToString()];
                    int oci = (int)Math.Round(Convert.ToDouble(overallConditionIndex));
                    yearOCI[year - startYear] = (byte)oci;
                }
                string oci64 = Convert.ToBase64String(yearOCI);
                session.EncodedActivities[i] = oci64 + ":" + session.EncodedActivities[i];
            }





            return encodedActivities;
        }


        public static List<string> GetDecodedOCI(string encodedActivity64)
        {
            List<string> yearlyOCI = new List<string>();
            string[] split = encodedActivity64.Split(':');
            byte[] encodedOCI = Convert.FromBase64String(split[0]);
            for(int i = 0; i < encodedOCI.Length; i++)
            {
                yearlyOCI.Add(encodedOCI[i].ToString());
            }
            return yearlyOCI;
        }



        public static List<ActivityResultStore> GetDecodedAppliedActivities(string encodedActivity64, List<string> budgets, List<string> activities, int startYear)
        {
            string[] split = encodedActivity64.Split(':');
            List<ActivityResultStore> resultActivities = new List<ActivityResultStore>();
            byte[] encodedActivites = Convert.FromBase64String(split[1]);
 
            int index = 0;
            while(index < encodedActivites.Length)
            {
                ActivityResultStore activity = new ActivityResultStore(encodedActivites.Skip(index).Take(9).ToArray(),activities,budgets,startYear);
                index += 9;
                resultActivities.Add(activity);
            }
            return resultActivities;
        }

        public static List<AssetLocationStore> GetAssetLocations(string sessionID, int beginRow, int endRow)
        {
            SessionStore session;
            List<AssetLocationStore> rangeLocations = new List<AssetLocationStore>();
            try
            {
                session = GetWorkPlanSessionFromSessionID(sessionID);
            }
            catch
            {
                return null;
            }

            if (session != null)
            {
                if (session.Locations == null)
                {
                    session.Locations = SelectScenario.GetAssetLocation(session.SimulationID);
                    session.Locations.Sort(delegate(AssetLocationStore a, AssetLocationStore b) { return a.ID.CompareTo(b.ID); }); //After sort this should be in increasing ID order.
                }

                for (int i = beginRow; i <= endRow; i++)
                {
                    try
                    {
                        string oid = session.OIDs[i];
                        if (session.Locations[i].OID == oid) // This was in the expected order)
                        {
                            rangeLocations.Add(session.Locations[i]);
                        }
                        else // for some reason our link list is out of order.  Match on OID
                        {
                            AssetLocationStore location = session.Locations.Find(delegate(AssetLocationStore asl) { return asl.OID == oid; });
                            rangeLocations.Add(location);
                        }
                    }
                    catch
                    {
                        AssetLocationStore assetError = new AssetLocationStore();
                        assetError.ID = "NA";
                        assetError.Name = "Out of range";
                        rangeLocations.Add(assetError);
                    }
                }
            }
            return rangeLocations;
        }

        public static ErrorStore GetErrorStore(string errorNumber, string message)
        {
            ErrorStore error = new ErrorStore();
            error.ErrorNumber = errorNumber;
            error.Description = message;
            return GetElmahLog(error);
        }

        public static ErrorStore GetElmahLog(ErrorStore error)
        {
            //Get last error from Elmah
            error.LogNumber = "Not implemented";
            error.Verbose = "Stack trace not implemented";
            return error;
        }

        internal static bool GetPrimaryKeyForLookups(string omsOIDHierarchy, out string primaryKey, out string foreignKey, out string foreignKeyTable)
        {
            string[] oids = omsOIDHierarchy.Split('|');
            primaryKey = null;
            foreignKey = null;
            foreignKeyTable = null;

            if(oids.Length > 1)
            { 
                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    connection.Open();
                    string query = "SELECT COLUMNMAP FROM cgSysFields WHERE OID=(SELECT LOOKUPFIELD FROM cgSysFields WHERE OID=@omsHierarchyOID)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("omsHierarchyOID", oids[oids.Length - 2]));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if(dr.Read())
                    {
                        if (dr["COLUMNMAP"] != DBNull.Value) primaryKey = dr["COLUMNMAP"].ToString();
                    }
                }

                using (SqlConnection connection = new SqlConnection(DB.OMSConnectionString))
                {
                    connection.Open();
                    string query = "SELECT TABLENAME, COLUMNMAP FROM cgSysFields WHERE OID=@omsHierarchyOID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.Add(new SqlParameter("omsHierarchyOID", oids[oids.Length - 2]));
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr["COLUMNMAP"] != DBNull.Value) foreignKey = dr["COLUMNMAP"].ToString();
                        if (dr["TABLENAME"] != DBNull.Value) foreignKeyTable = dr["TABLENAME"].ToString();
                    }
                }
            
            
            
            }
            return true;
        }
    }
}
