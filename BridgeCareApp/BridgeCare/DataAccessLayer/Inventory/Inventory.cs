using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class Inventory : IInventory
    {   
        /// <summary>
        /// Get Inventory details based on bmsId
        /// </summary>
        /// <param name="bmsId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public InventoryModel GetInventoryByBMSId(string bmsId, BridgeCareContext db)
        {
            var query = GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalk.InventoryItems);
            query += " FROM PennDot_Report_A WHERE BRIDGE_ID = '" + bmsId + "'";
            
            var BridgeData = GetInventoryModelData(db, query);
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
            var query = GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalk.InventoryItems);
            query += " FROM PennDot_Report_A WHERE BRKEY = " + brKey;
           
            var BridgeData = GetInventoryModelData(db, query);
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
            var query = "SELECT ";
            var separator = " ";
            foreach (var inventoryItem in inventoryItems)
            {
                query += separator + inventoryItem.ColumnName;
                separator = ",";
            }

            return query;
        }

        private InventoryModel GetInventoryModelData(BridgeCareContext db, string query)
        {
            InventoryModel BridgeData = new InventoryModel();
            try
            {
                DataTable dataTable = UtilityFunctions.NonEntitySQLQuery(query, db);
                foreach (InventoryItemModel inventoryItemModel in PennDotCrosswalk.InventoryItems)
                {
                    BridgeData.AddModel(inventoryItemModel, dataTable.Rows[0][inventoryItemModel.ColumnName].ToString());
                }

                var brKey = Convert.ToInt32(dataTable.Rows[0]["BRKEY"]);
                AddNbiLoadRaingToModel(BridgeData, brKey, db);
            }
            catch (SqlException ex)
            {
                HandleException.SqlError(ex, "Inventory Query");
            }
            catch (OutOfMemoryException ex)
            {
                HandleException.OutOfMemoryError(ex);
            }
            return BridgeData;
        }

        private static void AddNbiLoadRaingToModel(InventoryModel BridgeData, int brKey, BridgeCareContext db)
        {
            var nbiLoadRatingQuery = GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalk.NbiLoadRatingInventoryItems);
            nbiLoadRatingQuery += " FROM PENNDOT_NBI_LOAD_RATING WHERE BRKEY = " + brKey;
            var dataTable = UtilityFunctions.NonEntitySQLQuery(nbiLoadRatingQuery, db);
            var inventoryNbiLoadRatings = new List<InventoryNbiLoadRatingModel>();
            var nbiLoadRatingInventoryItems = PennDotCrosswalk.NbiLoadRatingInventoryItems;

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