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
        private readonly IInventory repo;
        private readonly IInventoryItemDetailModelGenerator modelGenerator;
        private readonly BridgeCareContext db;

        public InventoryController(IInventory repo, IInventoryItemDetailModelGenerator modelGenerator, BridgeCareContext db)
        {
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.modelGenerator = modelGenerator ?? throw new ArgumentNullException(nameof(modelGenerator));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Get: api/InventorySelectionModels
        /// </summary>
        [HttpGet]
        [Route("api/GetInventory")]
        public IHttpActionResult GetInventory() => Ok(repo.GetInventorySelectionModels(db));

        /// <summary>
        /// Get: api/InventoryItemDetailByBMSId
        /// </summary>
        [HttpGet]
        [Route("api/GetInventoryItemDetailByBmsId")]
        [ModelValidation("The BMS id is not valid")]
        public IHttpActionResult GetInventoryItemDetailByBmsId(string bmsId)
        {
            var inventoryItemDetailModel = modelGenerator.MakeInventoryItemDetailModel(repo.GetInventoryByBMSId(bmsId, db));
            inventoryItemDetailModel.BMSId = bmsId;
            return Ok(inventoryItemDetailModel);
        }

        /// <summary>
        /// Get: api/InventoryItemDetailByBRKey
        /// </summary>
        [HttpGet]
        [Route("api/GetInventoryItemDetailByBrKey")]
        [ModelValidation("The BR key is not valid.")]
        public IHttpActionResult GetInventoryItemDetailByBrKey(int brKey)
        {
            var inventoryItemDetailModel = modelGenerator.MakeInventoryItemDetailModel(repo.GetInventoryByBRKey(brKey, db));
            inventoryItemDetailModel.BRKey = brKey;
            return Ok(inventoryItemDetailModel);
        }
    }
}