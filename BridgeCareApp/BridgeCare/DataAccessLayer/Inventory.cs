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
        private readonly BridgeCareContext db;

        public Inventory(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        public InventoryModel GetInventory(SectionModel data, BridgeCareContext db)
        {
            InventoryModel BridgeData = new InventoryModel();

            String query = "SELECT ";
            String separator = " ";
            foreach (InventoryItemModel p in BridgeData.InventoryItems)
            {
                query += separator + p.ColumnName;
                separator = ",";
            }
            query += " FROM PennDot_Report_A WHERE BRKEY = " + data.ReferenceKey;

            try
            {
                DataTable dt = UtilityFunctions.NonEntitySQLQuery(query, db);
                foreach (InventoryItemModel p in BridgeData.InventoryItems)
                {
                    p.Value = dt.Rows[0][p.ColumnName];
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