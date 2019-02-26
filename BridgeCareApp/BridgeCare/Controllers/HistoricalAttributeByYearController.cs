using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    //retrieves all attributes year and value pairsfor give sectionId
    public class HistoricalAttributesByYearController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributeByYear attributes;

        public HistoricalAttributesByYearController(IAttributeByYear attributeByYear, BridgeCareContext context)
        {
            attributes = attributeByYear ?? throw new ArgumentNullException(nameof(attributeByYear));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get: api/historicalattributesbyyear/
        public List<AttributeByYearModel> Get(SectionModel sectionModel)
        {
            return attributes.GetHistoricalAttributes(sectionModel, db);
        }
    }
}