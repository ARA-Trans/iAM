using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Internal;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class InventoryDAL : IInventory
    {   
        /// <summary>
        /// Get Inventory details based on bmsId
        /// </summary>
        /// <param name="bmsId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public InventoryModel GetInventoryByBMSId(string bmsId, BridgeCareContext db)
        {
            var selectStatement = $"{GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalkDAL.InventoryItems)} FROM PennDot_Report_A WHERE BRIDGE_ID = @value";
            
            var BridgeData = GetInventoryModelData(db, selectStatement, bmsId);
            return BridgeData;
        }

        /// <summary>
        /// /// Get Inventory details based on brKey
        /// </summary>
        /// <param name="brKey"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public InventoryModel GetInventoryByBRKey(int brKey, BridgeCareContext db)
        {
            var selectStatement = $"{GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalkDAL.InventoryItems)} FROM PennDot_Report_A WHERE BRKEY = @value";
           
            var BridgeData = GetInventoryModelData(db, selectStatement, brKey);
            return BridgeData;
        }

        /// <summary>
        /// Get BRKey and BMSId pairs in form of InventorySelectionModels
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<InventorySelectionModel> GetInventorySelectionModels(BridgeCareContext db)
        {
            IQueryable<InventorySelectionModel> rawQueryForData = null;

            var select = string.Format("SELECT BRKEY as BRKey, BRIDGE_ID as BMSId FROM PennDot_Report_A");

            try
            {
                rawQueryForData = db.Database.SqlQuery<InventorySelectionModel>(select).AsQueryable();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "PennDot_Report_A");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return rawQueryForData.ToList();          
        }

        private static string GetSelectColumnsForPennDotCrosswalk(List<InventoryItemModel> inventoryItems)
        {
            var query = "SELECT";
            var separator = " ";
            foreach (var inventoryItem in inventoryItems)
            {
                query += separator + inventoryItem.ColumnName;
                separator = ",";
            }

            return query;
        }

        private InventoryModel GetInventoryModelData<T>(BridgeCareContext db, String selectStatement, T paramValue)
        {
            InventoryModel BridgeData = new InventoryModel();
            try
            {
                // create and open a connection to the database
                var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString);
                connection.Open();
                // create a sql command and add the value parameter
                var sqlCommand = new SqlCommand(selectStatement, connection);
                sqlCommand.Parameters.Add(new SqlParameter()
                {
                    ParameterName = "@value",
                    Value = paramValue
                });
                // get the data
                var dataReader = sqlCommand.ExecuteReader();
                // check for data returned
                if (dataReader.HasRows)
                {
                    // read the data
                    while (dataReader.Read())
                    {
                        var columnIndex = 0;
                        // create inventory item models from data
                        foreach (InventoryItemModel inventoryItemModel in PennDotCrosswalkDAL.InventoryItems)
                        {
                            if (columnIndex == 0)
                            {
                                // get the brkey value to add nbi load rating to the model
                                var brKey = Convert.ToInt32(dataReader["BRKEY"]);
                                AddNbiLoadRatingToModel(BridgeData, brKey, db);
                            }
                            BridgeData.AddModel(inventoryItemModel, dataReader.GetValue(columnIndex).ToString());
                            columnIndex++;
                        }
                    }
                }
                // close the dataReader
                dataReader.Close();
                // close the connection
                connection.Close();
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Inventory Query");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            catch (Exception ex)
            {
                HandleException.GeneralError(ex);
            }
            return BridgeData;
        }

        private static void AddNbiLoadRatingToModel(InventoryModel BridgeData, int brKey, BridgeCareContext db)
        {
            var nbiLoadRatingQuery = GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalkDAL.NbiLoadRatingInventoryItems);
            nbiLoadRatingQuery += " FROM PENNDOT_NBI_LOAD_RATING WHERE BRKEY = " + brKey;
            var dataTable = UtilityFunctions.NonEntitySQLQuery(nbiLoadRatingQuery, db);
            var inventoryNbiLoadRatings = new List<InventoryNbiLoadRatingModel>();
            var nbiLoadRatingInventoryItems = PennDotCrosswalkDAL.NbiLoadRatingInventoryItems;

            foreach (DataRow row in dataTable.Rows)
            {
                var inventoryNbiLoadRatingModel = new InventoryNbiLoadRatingModel();
                foreach (var nbiLoadRatingInventoryItem in nbiLoadRatingInventoryItems)
                {
                    var itemToAdd = new InventoryItemModel
                    {
                        ColumnName = nbiLoadRatingInventoryItem.ColumnName,
                        DisplayValue = nbiLoadRatingInventoryItem.DisplayValue,
                        Id = nbiLoadRatingInventoryItem.Id,
                        ViewName = nbiLoadRatingInventoryItem.ViewName
                    };
                    itemToAdd.DisplayValue = row[nbiLoadRatingInventoryItem.ColumnName].ToString();
                    inventoryNbiLoadRatingModel.NbiLoadRatingItems.Add(itemToAdd);
                }
                inventoryNbiLoadRatings.Add(inventoryNbiLoadRatingModel);
            }
            BridgeData.InventoryNbiLoadRatings = inventoryNbiLoadRatings;
        }
    }
}