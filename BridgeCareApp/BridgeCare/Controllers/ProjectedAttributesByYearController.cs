using BridgeCare.Interfaces;
using BridgeCare.Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace BridgeCare.Controllers
{
    public class ProjectedAttributesByYearController : ApiController
    {
        private readonly BridgeCareContext db;
        private readonly IAttributesByYear attributes;

        public ProjectedAttributesByYearController(IAttributesByYear attributeByYear, BridgeCareContext context)
        {
            attributes = attributeByYear ?? throw new ArgumentNullException(nameof(attributeByYear));
            db = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Get: api/projectedAttributesByYear/
        /// body (example)
        /// {
        ///"sectionId":1000012,
        ///"networkId":13,
        ///"simulationId":9
        /// }
        /// </summary>
        public List<AttributeByYearModel> Get(SimulatedSegmentIdsModel segmentAddressModel)
        {
            return attributes.GetProjectedAttributes(segmentAddressModel, db);
        }
    }
}