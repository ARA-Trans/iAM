using BridgeCare.ApplicationLog;
using BridgeCare.EntityClasses;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BridgeCare.DataAccessLayer
{
    public class Inventory : IInventory
    {
        private readonly BridgeCareContext db;

        public Inventory(BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        IQueryable<InventoryModel> GetInventory(SectionModel data, BridgeCareContext db)
        {

            return IQuerable<InventoryModel>;
        }


    }