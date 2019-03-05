using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

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

        /// Get: api/historicalAttributesByYear/
        [HttpGet]
        public List<AttributeByYearModel> Get(SectionModel sectionModel) => attributes.GetHistoricalAttributes(sectionModel, db);
    }
}