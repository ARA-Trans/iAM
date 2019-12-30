using BridgeCare.Interfaces;
using BridgeCare.Security;
using log4net;
using System;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    public class InventoryController : ApiController
    {
        private readonly IInventoryItemDetailModelGenerator modelGenerator;
        private readonly IInventory repo;
        private readonly BridgeCareContext db;

        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(InventoryController));

        public InventoryController(IInventoryItemDetailModelGenerator modelGenerator, IInventory repo, BridgeCareContext db)
        {
            log.Debug("Debug message from Inventory controller");
            this.modelGenerator = modelGenerator ?? throw new ArgumentNullException(nameof(modelGenerator));
            this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// API endpoint for fetching inventory data
        /// </summary>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetInventory")]
        [RestrictAccess]
        public IHttpActionResult GetInventory() => Ok(repo.GetInventorySelectionModels(db));

        /// <summary>
        /// API endpoint for fetching inventory item detail data by bms id
        /// </summary>
        /// <param name="bmsId">BMS identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetInventoryItemDetailByBmsId")]
        [ModelValidation("The BMS id is not valid")]
        [RestrictAccess]
        public IHttpActionResult GetInventoryItemDetailByBmsId(string bmsId)
        {
            var inventoryItemDetailModel = modelGenerator
                .MakeInventoryItemDetailModel(repo.GetInventoryByBMSId(bmsId, db));
            inventoryItemDetailModel.BMSId = bmsId;

            return Ok(inventoryItemDetailModel);
        }

        /// <summary>
        /// API endpoint for fetching inventory item detail data by BR key
        /// </summary>
        /// <param name="brKey">BR key identifier</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("api/GetInventoryItemDetailByBrKey")]
        [ModelValidation("The BR key is not valid.")]
        [RestrictAccess]
        public IHttpActionResult GetInventoryItemDetailByBrKey(int brKey)
        {
            var inventoryItemDetailModel = modelGenerator
                .MakeInventoryItemDetailModel(repo.GetInventoryByBRKey(brKey, db));
            inventoryItemDetailModel.BRKey = brKey;

            return Ok(inventoryItemDetailModel);
        }
    }
}