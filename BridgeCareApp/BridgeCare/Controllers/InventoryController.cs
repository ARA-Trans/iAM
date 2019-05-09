using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
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
        /// Get: api/InventoryItemDetailByBMSId
        /// </summary>
        [Route("api/InventoryItemDetailByBMSId")]
        [ModelValidation("Given BMSId is not valid")]        
        [HttpGet]
        public InventoryItemDetailModel Get(string bmsId)
        {
            var inventoryModel = inventory.GetInventoryByBMSId(bmsId, db);
            var inventoryItemDetailModel = inventoryItemDetailModelGenerator.MakeInventoryItemDetailModel(inventoryModel);
            inventoryItemDetailModel.BMSId = bmsId;
            return inventoryItemDetailModel;
        }

        /// <summary>
        /// Get: api/InventoryItemDetailByBRKey
        /// </summary>
        [Route("api/InventoryItemDetailByBRKey")]
        [ModelValidation("Given BRKey is not valid")]
        [HttpGet]
        public InventoryItemDetailModel Get(int brKey)
        {
            var inventoryModel = inventory.GetInventoryByBRKey(brKey, db);
            var inventoryItemDetailModel = inventoryItemDetailModelGenerator.MakeInventoryItemDetailModel(inventoryModel);
            inventoryItemDetailModel.BRKey = brKey;
            return inventoryItemDetailModel;
        }

        /// <summary>
        /// Get: api/InventorySelectionModels
        /// </summary>
        [Route("api/InventorySelectionModels")]       
        [HttpGet]
        public List<InventorySelectionModel> Get()
        {
            var inventorySelectionModels = inventory.GetInventorySelectionModels(db);
            return inventorySelectionModels;
        }
    }
}