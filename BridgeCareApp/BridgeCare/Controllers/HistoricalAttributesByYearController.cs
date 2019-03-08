using BridgeCare.Interfaces;
using BridgeCare.Models;
using BridgeCare.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Filters;
using System.ComponentModel.DataAnnotations;

namespace BridgeCare.Controllers
{
    ///retrieves all attributes year and value pairsfor give sectionId
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
        /// Get: api/historicalAttributesByYear/
        /// body (example)
        ///{
        ///"sectionId": 1000004,
        ///"bridgeId": "10001",
        ///"bridgeKey": "15003004520000",
        ///"networkId": 13
        ///}
        /// </summary>
        [HttpGet]
        [ModelValidation]
        public List<AttributeByYearModel> Get([FromBody]SectionModel sectionModel) => attributes.GetHistoricalAttributes(sectionModel, db);
        
    }
}