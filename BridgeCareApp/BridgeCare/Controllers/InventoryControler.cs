using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class InventoryController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IInventory inventory;

        public InventoryController(IInventory inventoryInterface, BridgeCareContext context)
        {
            inventory = inventoryInterface ?? throw new ArgumentNullException(nameof(inventoryInterface));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get: api/Inventory
        /// </summary>
        [Route("api/Inventory")]
        [ModelValidation("Given simulation data is not valid")]
        [HttpGet]
        public InventoryModel Get(SectionModel data) => inventory.GetInventory(data, db);
    }
}