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
        private readonly IInventoryItemDetailModelGenerator inventoryItemDetailModelGenerator;

        public InventoryController(IInventory inventoryInterface, IInventoryItemDetailModelGenerator inventoryItemDetailModelGenerator, BridgeCareContext context)
        {
            inventory = inventoryInterface ?? throw new ArgumentNullException(nameof(inventoryInterface));
            this.inventoryItemDetailModelGenerator = inventoryItemDetailModelGenerator ?? throw new ArgumentNullException(nameof(inventoryItemDetailModelGenerator));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }        

        /// <summary>
        /// Get: api/InventoryItemDetail
        /// </summary>
        [Route("api/InventoryItemDetail")]
        [ModelValidation("Given SectionModel is not valid")]        
        [HttpGet]
        public InventoryItemDetailModel Get(SectionModel data)
        {
            var inventoryModel = inventory.GetInventory(data, db);
            var inventoryItemDetailModel = inventoryItemDetailModelGenerator.MakeInventoryItemDetailModel(data, inventoryModel);
            return inventoryItemDetailModel;
        }        
    }
}