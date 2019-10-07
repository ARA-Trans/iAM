using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;

namespace BridgeCare.DataAccessLayer.Inventory
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
            
            return GetInventoryModelData(db, selectStatement, bmsId);
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
           
            return GetInventoryModelData(db, selectStatement, brKey);
        }

        /// <summary>
        /// Get BRKey and BMSId pairs in form of InventorySelectionModels
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<InventorySelectionModel> GetInventorySelectionModels(BridgeCareContext db)
        {
            var select = string.Format("SELECT BRKEY as BRKey, BRIDGE_ID as BMSId FROM PennDot_Report_A");

            return db.Database
                .SqlQuery<InventorySelectionModel>(select)
                .AsQueryable()
                .ToList();
        }

        /// <summary>
        /// Creates a select statement for the PennDot crosswalk table
        /// </summary>
        /// <param name="inventoryItems"></param>
        /// <returns>string</returns>
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

        /// <summary>
        /// Fetches inventory data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">BridgeCareContext</param>
        /// <param name="selectStatement">sql select statement</param>
        /// <param name="paramValue">sql command parameter value</param>
        /// <returns>InventoryModel</returns>
        private InventoryModel GetInventoryModelData<T>(BridgeCareContext db, string selectStatement, T paramValue)
        {
            var inventory = new InventoryModel();

            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["BridgeCareContext"].ConnectionString);
            connection.Open();

            var sqlCommand = new SqlCommand(selectStatement, connection);
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                ParameterName = "@value",
                Value = paramValue
            });

            var dataReader = sqlCommand.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    var columnIndex = 0;

                    foreach (InventoryItemModel inventoryItemModel in PennDotCrosswalkDAL.InventoryItems)
                    {
                        if (columnIndex == 0)
                        {
                            var brKey = Convert.ToInt32(dataReader["BRKEY"]);
                            AddNbiLoadRatingToModel(inventory, brKey, db);
                        }

                        inventory.AddModel(inventoryItemModel, dataReader.GetValue(columnIndex).ToString());

                        columnIndex++;
                    }
                }
            }

            dataReader.Close();
            connection.Close();

            return inventory;
        }

        /// <summary>
        /// Fetches nbi loading rating for an inventory item and adds it to the inventory model
        /// </summary>
        /// <param name="model">InventoryModel</param>
        /// <param name="brKey">BR key identifier</param>
        /// <param name="db">BridgeCareContext</param>
        private static void AddNbiLoadRatingToModel(InventoryModel model, int brKey, BridgeCareContext db)
        {
            var nbiLoadRatingQuery =
                $"{GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalkDAL.NbiLoadRatingInventoryItems)}" +
                $" FROM PENNDOT_NBI_LOAD_RATING WHERE BRKEY = {brKey}";

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

            model.InventoryNbiLoadRatings = inventoryNbiLoadRatings;
        }
    }
}