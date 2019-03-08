using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Filters;

namespace BridgeCare.Controllers
{
    /// <summary>
    /// Retrieves all attributes year and value pairsfor give sectionId
    /// </summary>
    public class HistoricalAttributesByYearController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributesByYear attributes;

        public HistoricalAttributesByYearController(IAttributesByYear attributeByYear, BridgeCareContext context)
        {
            attributes = attributeByYear ?? throw new ArgumentNullException(nameof(attributeByYear));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get: api/historicalAttributesByYear/ body (example) { "sectionId":
        /// 1000004, "bridgeId": "10001", "bridgeKey": "15003004520000",
        /// "networkId": 13 }
        /// </summary>
        [HttpGet]
        [ModelValidation]
        public List<AttributeByYearModel> Get([FromBody]SectionModel sectionModel) => attributes.GetHistoricalAttributes(sectionModel, db);
    }
}