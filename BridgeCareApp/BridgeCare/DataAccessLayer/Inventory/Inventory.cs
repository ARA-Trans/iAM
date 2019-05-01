using BridgeCare.ApplicationLog;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.Utility;
using System;
using System.Data;
using System.Data.SqlClient;

namespace BridgeCare.DataAccessLayer
{
    public class Inventory : IInventory
    {   
        public InventoryModel GetInventoryByBMSId(string bmsId, BridgeCareContext db)
        {
            var query = "SELECT ";
            var separator = " ";
            foreach (InventoryItemModel p in PennDotCrosswalk.InventoryItems)
            {
                query += separator + p.ColumnName;
                separator = ",";
            }
            query += " FROM PennDot_Report_A WHERE BRIDGE_ID = " + bmsId;
            var BridgeData = GetInventoryModelData(db, query);
            return BridgeData;
        }       

        public InventoryModel GetInventoryByBRKey(int brKey, BridgeCareContext db)
        {
            var query = "SELECT ";
            var separator = " ";
            foreach (InventoryItemModel p in PennDotCrosswalk.InventoryItems)
            {
                query += separator + p.ColumnName;
                separator = ",";
            }
            query += " FROM PennDot_Report_A WHERE BRKEY = " + brKey;
            var BridgeData = GetInventoryModelData(db, query);
            return BridgeData;
        }

        private InventoryModel GetInventoryModelData(BridgeCareContext db, string query)
        {
            InventoryModel BridgeData = new InventoryModel();
            try
            {
                DataTable dt = UtilityFunctions.NonEntitySQLQuery(query, db);
                foreach (InventoryItemModel p in PennDotCrosswalk.InventoryItems)
                {
                    BridgeData.AddModel(p, dt.Rows[0][p.ColumnName].ToString());
                }
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
    }
}