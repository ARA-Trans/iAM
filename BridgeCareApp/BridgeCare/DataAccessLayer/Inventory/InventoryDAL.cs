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
        public InventoryModel GetInventoryByBRKey(string brKey, BridgeCareContext db)
        {
            var selectStatement = $"{GetSelectColumnsForPennDotCrosswalk(PennDotCrosswalkDAL.InventoryItems)} FROM PennDot_Report_A WHERE BRKEY = @value";
           
            return GetInventoryModelData(db, selectStatement, Convert.ToInt32(brKey));
        }


        /// <summary>
        /// Get BRKey and BMSId pairs in form of InventorySelectionModels, if they match the provided user's criteria.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<InventorySelectionModel> GetInventorySelectionModels(BridgeCareContext db, UserInformationModel userInformation)
        {
            var userCriteriaEntity = db.UserCriteria.Where(criteria => criteria.USERNAME == userInformation.Name).First();
            var userCriteria = new UserCriteriaModel(userCriteriaEntity);

            if (!userCriteria.HasAccess)
            {
                throw new UnauthorizedAccessException($"User {userInformation.Name} has no inventory access.");
            }

            string query = "SELECT CAST(BRKEY AS VARCHAR) as BRKey, BRIDGE_ID as BMSId FROM PennDot_Report_A ORDER BY CONVERT(INT, BRKey) ASC";

            if (userCriteria.HasCriteria)
            {
                string networkId = db.NETWORKS.FirstOrDefault().NETWORKID.ToString();
                query = ConstructInventoryQuery(userCriteria.Criteria, networkId);
            }

            return db.Database
                .SqlQuery<InventorySelectionModel>(query)
                .AsQueryable()
                .ToList();
        }

        /// <summary>
        /// Constructs a SQL query for fetching keys and IDs of bridges matching the provided criteria string.
        /// </summary>
        /// <param name="userCriteriaString">User Criteria String</param>
        /// <param name="networkIDString">Network ID as String</param>
        /// <returns>SQL query string</returns>
        private string ConstructInventoryQuery(string userCriteriaString, string networkIDString)
        {
            var innerJoinColumns = new List<string> { "FEATURE_CARRIED", "FEATURE_INTERSECTED", "LOCATION", "DISTRICT", "OWNER_CODE", "MPO" };
            var innerJoinConditionList = innerJoinColumns.Select(column => $"PennDot_Report_A.{column} = SEGMENT_{networkIDString}_NS0.{column}").ToList();
            var innerJoinConditions = string.Join(" and ", innerJoinConditionList);

            var selectClause = "SELECT CAST(PennDot_Report_A.BRKEY AS VARCHAR) as BRKey, PennDot_Report_A.BRIDGE_ID as BMSId";
            // SEGMENT_#_NS0 contains the bridge data to be checked by the criteria string
            var fromClause = $"FROM SEGMENT_{networkIDString}_NS0";
            // PennDot_Report_A contains keys and IDs of bridges.
            var joinClause = $"INNER JOIN PennDot_Report_A on {innerJoinConditions}";
            var whereClause = $"WHERE ({userCriteriaString})";
            whereClause = whereClause.Replace("[", $"SEGMENT_{networkIDString}_NS0.[");
            var orderByClause = "ORDER BY CONVERT(INT, BRKey) ASC";

            return string.Join(" ", selectClause, fromClause, joinClause, whereClause, orderByClause);
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
