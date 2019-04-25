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
        /// Get: api/InventoryItemDetail
        /// </summary>
        [Route("api/InventoryItemDetail")]
        [ModelValidation("Given referenceKey is not valid")]
        [HttpGet]
        public InventoryItemDetailModel Get(SectionModel data)
        {
            var inventoryModel = inventory.GetInventory(data, db);
            var inventoryItemDetailModel = MakeInventoryItemDetailModel(inventoryModel);
            return inventoryItemDetailModel;
        }

        private InventoryItemDetailModel MakeInventoryItemDetailModel(InventoryModel inventoryModel)
        {
            throw new NotImplementedException();
        }
    }
}